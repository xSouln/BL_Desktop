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
        public UI_Property<uint> BootStartAddress = new UI_Property<uint> { Name = nameof(BootStartAddress) };
        public UI_Property<uint> BootEndAddress = new UI_Property<uint> { Name = nameof(BootEndAddress) };

        public UI_Property<uint> AppStartAddress = new UI_Property<uint> { Name = nameof(AppStartAddress) };
        public UI_Property<uint> AppEndAddress = new UI_Property<uint> { Name = nameof(AppEndAddress) };

        public UI_Property<ushort> Crc = new UI_Property<ushort> { Name = nameof(Crc) };

        public UI_Property<bool, EAppInfoStatus> BootIsEnable = new UI_Property<bool, EAppInfoStatus> { Name = nameof(BootIsEnable), Request = EAppInfoStatus.BootIsEnable };
        public UI_Property<bool, EAppInfoStatus> Reset = new UI_Property<bool, EAppInfoStatus> { Name = nameof(Reset), Request = EAppInfoStatus.Reset };
        public UI_Property<bool, EAppInfoStatus> JumpToMain = new UI_Property<bool, EAppInfoStatus> { Name = nameof(JumpToMain), Request = EAppInfoStatus.JumpToMain };
        public UI_Property<bool, EAppInfoStatus> JumpToBoot = new UI_Property<bool, EAppInfoStatus> { Name = nameof(JumpToBoot), Request = EAppInfoStatus.JumpToBoot };
        public UI_Property<bool, EAppInfoStatus> AppCrcError = new UI_Property<bool, EAppInfoStatus> { Name = nameof(AppCrcError), Request = EAppInfoStatus.AppCrcError };

        public static bool IsEnable(EAppInfoStatus state, EAppInfoStatus request) => (state & request) == request;
        public static EAppInfoStatus GetStatus(UI_Property<bool, EAppInfoStatus> property) => property.Value ? property.Request : 0;

        public AppFlashInfoT Value
        {
            get => new AppFlashInfoT
            {
                BootStartAddress = BootStartAddress.Value,
                BootEndAddress = BootEndAddress.Value,

                AppStartAddress = AppStartAddress.Value,
                AppEndAddress = AppEndAddress.Value,

                Crc = Crc.Value
            };
            set
            {
                BootStartAddress.Value = value.BootStartAddress;
                BootEndAddress.Value = value.BootEndAddress;

                AppStartAddress.Value = value.AppStartAddress;
                AppEndAddress.Value = value.AppEndAddress;

                BootIsEnable.Value = IsEnable(value.Status, BootIsEnable.Request);
                Reset.Value = IsEnable(value.Status, Reset.Request);
                JumpToMain.Value = IsEnable(value.Status, JumpToMain.Request);
                JumpToBoot.Value = IsEnable(value.Status, JumpToBoot.Request);
                AppCrcError.Value = IsEnable(value.Status, AppCrcError.Request);

                Crc.Value = value.Crc;
            }
        }
    }
}
