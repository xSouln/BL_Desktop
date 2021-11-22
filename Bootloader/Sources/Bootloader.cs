using BootloaderDesktop.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Sources;
using xLib.Transceiver;

namespace BootloaderDesktop
{
    public partial class Bootloader
    {
        public static class UpdateList
        {
            public static xRequest GetInfo = Requests.Get.Info.Prepare();

            public static List<xRequest> List = new List<xRequest>
            {
                GetInfo,
            };
        }

        public static UI_FlashInfo FlashInfo = new UI_FlashInfo();

        public static event xAction<string> Tracer;
        public static xAction<bool, byte[]> Transmitter;

        private static xTcp tcp;
        private static xSerialPort serial_port;
        private static xRequestLine request_line;

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
                RequstTransmitter = RequstTransmitter,
                TryCount = 2,
                ResponseTimeOut = 300,
                Tracer = xTracer.Message
            };

            tcp = new xTcp() { Receiver = new xReceiver(50000, new byte[] { (byte)'\r' }) { EventPacketReceive = EventPacketReceive } };
            serial_port = new xSerialPort() { Receiver = new xReceiver(50000, new byte[] { (byte)'\r' }) { EventPacketReceive = EventPacketReceive } };

            request_line.StartUpdate(900);
        }

        public static xSerialPort SerialPort => serial_port;

        public static xTcp Tcp => tcp;

        private static xAction<bool, byte[]> RequstTransmitter()
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
