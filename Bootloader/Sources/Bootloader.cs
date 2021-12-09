using BootloaderDesktop.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLib;
using xLib.Sources;
using xLib.Transceiver;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop
{
    public partial class Bootloader
    {
        public static class UpdateList
        {
            public static xRequest GetFirmwareInfo = Requests.Get.FirmwareInfo.Prepare();
            public static xRequest GetAppInfo = Requests.Get.AppInfo.Prepare();
            public static xRequest GetStatus = Requests.Get.Status.Prepare();

            public static List<xRequest> List = new List<xRequest>
            {
                GetAppInfo,
                GetStatus
            };
        }

        public static UI_FirmwareInfo FirmwareInfo = new UI_FirmwareInfo();
        public static UI_AppFlashInfo AppFlashInfo = new UI_AppFlashInfo();
        public static UI_Status Status = new UI_Status();

        public static byte[] Firmware;

        public static event xAction<string> Tracer;
        public static xAction<bool, byte[]> Transmitter;

        private static xTcp tcp;
        private static xSerialPort serial_port;
        private static xRequestLine request_line;
        private static Thread thread_load;

        public static xCommunicationControl Communication = new xCommunicationControl(2000);

        private static void trace(string note) { Tracer?.Invoke(note); xTracer.Message(note); }

        static Bootloader()
        {
            new Requests.Get();
        }

        public static void Init()
        {
            request_line = new xRequestLine
            {
                Requests = UpdateList.List,
                RequstTransmitter = GetTransmitter,
                TryCount = 1,
                ResponseTimeOut = 300,
                Tracer = xTracer.Message
            };

            tcp = new xTcp() { Receiver = new xReceiver(50000, new byte[] { (byte)'\r' }) { EventPacketReceive = EventPacketReceive } };
            serial_port = new xSerialPort() { Receiver = new xReceiver(50000, new byte[] { (byte)'\r' }) { EventPacketReceive = EventPacketReceive } };

            request_line.StartUpdate(900);
        }

        public static xSerialPort SerialPort => serial_port;

        public static xTcp Tcp => tcp;

        public static xAction<bool, byte[]> GetTransmitter()
        {
            if (Transmitter != null) { return Transmitter; }
            else if (tcp != null && tcp.IsConnected) { return tcp.Send; }
            else if (serial_port != null && serial_port.IsConnected) { return serial_port.Send; }
            return null;
        }

        public static void Dispose()
        {
            tcp?.Disconnect();
            serial_port?.Disconnect();
            request_line?.Dispose();
            thread_load?.Abort();
        }

        public static async void Erase(uint start_address, uint end_address)
        {
            var response = await Requests.Try.Erase.Prepare(new RequestEraseT { StartAddress = start_address, EndAdress = end_address }).TransmitionAsync(GetTransmitter(), 1, 5000);
            xTracer.Message("Erase result: " + response.Result?.Error);
        }
        public static void StopLoad()
        {
            thread_load?.Abort();
            thread_load = null;
        }

        public static async void SetLock(ELockState request)
        {
            var response = await Requests.Set.LockState.Prepare(request).TransmitionAsync(GetTransmitter(), 1, 300);
            xTracer.Message("Erase result: " + response.Result?.Error);
        }

        public static async void JumpToMain()
        {
            var response = await Requests.Try.JumpToMain.Prepare().TransmitionAsync(GetTransmitter(), 1, 300);
            xTracer.Message("Erase result: " + response.Result?.Error);
        }

        public static async void JumpToBoot()
        {
            var response = await Requests.Try.JumpToBoot.Prepare().TransmitionAsync(GetTransmitter(), 1, 1000);
            xTracer.Message("Erase result: " + response.Result?.Error);
            //await Requests.Try.Erase.Prepare(new RequestEraseT { StartAddress = 0x08004000, EndAdress = 0x08008000 }).TransmitionAsync(RequstTransmitter(), 1, 1000);
        }

        public static async void Reset()
        {
            var response = await Requests.Try.Reset.Prepare().TransmitionAsync(GetTransmitter(), 1, 300);
            xTracer.Message("Erase result: " + response.Result?.Error);
        }

        public static async void Read()
        {
            var response = await Requests.Try.Reset.Prepare().TransmitionAsync(GetTransmitter(), 1, 300);
            xTracer.Message("Erase result: " + response.Result?.Error);
        }

        public static async void LoadAppInfo(uint info_address, FirmwareInfoT info)
        {
            //var result1 = await Requests.Try.Erase.Prepare(new RequestEraseT { StartAddress = info_address, EndAdress = info_address + info.PageSize }).TransmitionAsync(RequstTransmitter(), 1, 1000);
            var result1 = await Requests.Try.Erase.Prepare(new RequestEraseT { StartAddress = 0x08004000, EndAdress = 0x08008000 }).TransmitionAsync(GetTransmitter(), 1, 1000);

            Status.Erase.WaitValue(false, 1000);

            ushort info_size;
            unsafe { info_size = (ushort)sizeof(FirmwareInfoT); }

            var result2 = await Requests.Try.Write.Prepare(new RequestWriteT
            {
                StartAddress = info_address,
                DataSize = info_size,
            },
            info
            ).TransmitionAsync(GetTransmitter(), 1, 1000);

            Status.Write.WaitValue(false, 1000);

            var result3 = await Requests.Try.UpdateInfo.Prepare().TransmitionAsync(GetTransmitter(), 1, 1000);
        }

        public static void StartLoad(uint start_address, byte[] firmaware, ushort packet_size)
        {
            int offset = 0;
            ushort crc = 0;
            uint address = start_address;
            List<byte> data = new List<byte>();

            if (firmaware == null || firmaware.Length == 0 || thread_load != null) { return; }

            thread_load = new Thread(async () =>
            {
                while (true)
                {
                    int i = 0;
                    data.Clear();
                    while (offset < firmaware.Length && i < packet_size)
                    {
                        data.Add(firmaware[offset]);
                        crc += firmaware[offset];
                        offset++;
                        i++;
                    }

                    if (data.Count == 0) { break; }

                    var result = await Requests.Try.Write.Prepare(new RequestWriteT
                    {
                        StartAddress = address,
                        DataSize = (ushort)data.Count,
                    },
                    data
                    ).TransmitionAsync(GetTransmitter(), 1, 1000);

                    xTracer.Message("Write result: " + result.Result?.Error + ", response time: " + result.ResponseTime);

                    address += (uint)data.Count;
                }

                LoadAppInfo(0x08004000, new FirmwareInfoT
                {
                    StartAddress = start_address,
                    EndAdress = start_address + (uint)firmaware.Length,
                    Content = 0x400,
                    Crc = crc
                });

                thread_load = null;
            });
            thread_load.Start();
        }

        public static void EventPacketReceive(xReceiverData ReceiverData)
        {
            string convert = "";
            string temp = "";
            ResponseT response;
            unsafe { response = *(ResponseT*)ReceiverData.Content.Obj; }
            int obj_size;

            if (response.Prefix.Start == xResponse.START_CHARECTER)
            {
                xContent content;
                unsafe
                {
                    if (ReceiverData.Content.Size < sizeof(ResponseT)) { ReceiverData.xRx.Response = xRxResponse.Storage; return; }
                    obj_size = ReceiverData.Content.Size - sizeof(ResponseT);

                    if (obj_size < response.Info.Size) { ReceiverData.xRx.Response = xRxResponse.Storage; return; }
                    if (obj_size > response.Info.Size) { trace("error content size"); goto error; }

                    content = ReceiverData.Content;
                    content.Obj += sizeof(ResponseT);
                    content.Size -= sizeof(ResponseT);
                }

                if (Requests.Handler.Identification(ReceiverData.Content)) { Communication.Update(); goto end; }
            }
        error:
            unsafe
            {
                convert = xConverter.GetString(ReceiverData.Content.Obj, ReceiverData.Content.Size);
                try { temp = xConverter.GetString(ReceiverData.Content.Obj, ReceiverData.Content.Size); }
                catch { temp = "Encoding error"; }
                trace("Trace: " + temp + " {" + convert + "}");
            }

        end: ReceiverData.xRx.Response = xRxResponse.Accept;
        }
    }
}
