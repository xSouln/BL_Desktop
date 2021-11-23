using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop.UI
{
    public class UI_AppFlashInfo
    {
        public UI_Property<uint> StartAddress = new UI_Property<uint> { Name = nameof(StartAddress) };
        public UI_Property<uint> EndAdress = new UI_Property<uint> { Name = nameof(EndAdress) };
        public UI_Property<ushort> PageSize = new UI_Property<ushort> { Name = nameof(PageSize) };
        public UI_Property<ushort> Crc = new UI_Property<ushort> { Name = nameof(Crc) };

        public AppFlashInfoT Value
        {
            get => new AppFlashInfoT
            {
                StartAddress = StartAddress.Value,
                EndAdress = EndAdress.Value,
                PageSize = PageSize.Value,
                Crc = Crc.Value
            };
            set
            {
                StartAddress.Value = value.StartAddress;
                EndAdress.Value = value.EndAdress;
                PageSize.Value = value.PageSize;
                Crc.Value = value.Crc;
            }
        }
    }
}
