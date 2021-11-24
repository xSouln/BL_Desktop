using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace BootloaderDesktop
{
    public partial class Bootloader
    {
        public class Colections
        {
            public static ObservableCollection<RowStructure> Structures { get; set; } = new ObservableCollection<RowStructure>();

            public static ObservableCollection<UI_Property> Propertys { get; set; } = new ObservableCollection<UI_Property>()
            {
                new UI_Property { Name = "App flash info:" },
                AppFlashInfo.BootStartAddress,
                AppFlashInfo.BootEndAddress,
                AppFlashInfo.AppStartAddress,
                AppFlashInfo.AppEndAddress,
                AppFlashInfo.Crc,

                AppFlashInfo.BootIsEnable,
                AppFlashInfo.Reset,
                AppFlashInfo.JumpToMain,
                AppFlashInfo.JumpToBoot,
                AppFlashInfo.AppCrcError,

                new UI_Property { Name = "" },
                new UI_Property { Name = "Status:" },
                Status.WriteAddress,
                Status.ReadAddress,
                Status.EraseAddress,
                Status.Write,
                Status.Read,
                Status.Erase,
                Status.FlashUnlocked,
                Status.Operation,
                Status.OperationTime,
                Status.OperationResult
            };
        }
    }
}
