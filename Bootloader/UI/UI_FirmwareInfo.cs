using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop.UI
{
    public class UI_FirmwareInfo
    {
        public UI_Property<uint> StartAddress = new UI_Property<uint> { Name = nameof(StartAddress) };
        public UI_Property<uint> EndAdress = new UI_Property<uint> { Name = nameof(EndAdress) };
        public UI_Property<ushort> Crc = new UI_Property<ushort> { Name = nameof(Crc) };
        public UI_Property<ushort> Handler = new UI_Property<ushort> { Name = nameof(Handler) };

        public FirmwareInfoT Value
        {
            get => new FirmwareInfoT
            {
                StartAddress = StartAddress.Value,
                EndAdress = EndAdress.Value,
                Crc = Crc.Value,
                //Handler = Handler.Value
            };
            set
            {
                StartAddress.Value = value.StartAddress;
                EndAdress.Value = value.EndAdress;
                Crc.Value = value.Crc;
                //Handler.Value = value.Handler;
            }
        }
    }
}
