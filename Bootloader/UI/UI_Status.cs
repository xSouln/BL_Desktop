using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop.UI
{
    public class UI_Status
    {
        public UI_Property<uint> WriteAddress = new UI_Property<uint> { Name = nameof(WriteAddress) };
        public UI_Property<uint> ReadAddress = new UI_Property<uint> { Name = nameof(ReadAddress) };
        public UI_Property<uint> EraseAddress = new UI_Property<uint> { Name = nameof(EraseAddress) };

        public UI_Property<EOperation> Operation = new UI_Property<EOperation> { Name = nameof(Operation) };
        public UI_Property<ushort> OperationTime = new UI_Property<ushort> { Name = nameof(OperationTime) };
        public UI_Property<EErrors> OperationResult = new UI_Property<EErrors> { Name = nameof(OperationResult) };

        public UI_Property<bool, EStatus> Write = new UI_Property<bool, EStatus> { Name = nameof(Write), Request = EStatus.Write };
        public UI_Property<bool, EStatus> Read = new UI_Property<bool, EStatus> { Name = nameof(Read), Request = EStatus.Read };
        public UI_Property<bool, EStatus> Erase = new UI_Property<bool, EStatus> { Name = nameof(Erase), Request = EStatus.Erase };
        public UI_Property<bool, EStatus> FlashUnlocked = new UI_Property<bool, EStatus> { Name = nameof(FlashUnlocked), Request = EStatus.FlashUnlocked, EventSelection = FlashUnlockedEventSelection };

        private static void FlashUnlockedEventSelection(UI_Property<bool, EStatus> arg)
        {
            if (arg.Value) { Bootloader.SetLock(ELockState.Locked); }
            else { Bootloader.SetLock(ELockState.Unlocked); }
        }

        public static bool IsEnable(EStatus state, EStatus request) => (state & request) == request;
        public static EStatus GetStatus(UI_Property<bool, EStatus> property) => property.Value ? property.Request : 0;

        public StatusT Value
        {
            get => new StatusT
            {
                WriteAddress = WriteAddress.Value,
                ReadAddress = ReadAddress.Value,
                EraseAddress = EraseAddress.Value,

                Operation = Operation.Value,
                OperationTime = OperationTime.Value,
                OperationResult = OperationResult.Value,

                Status = GetStatus(Write)
                | GetStatus(Read)
                | GetStatus(Erase)
                | GetStatus(FlashUnlocked)
            };
            set
            {
                WriteAddress.Value = value.WriteAddress;
                ReadAddress.Value = value.ReadAddress;
                EraseAddress.Value = value.EraseAddress;

                Operation.Value = value.Operation;
                OperationTime.Value = value.OperationTime;
                OperationResult.Value = value.OperationResult;

                Write.Value = IsEnable(value.Status, Write.Request);
                Read.Value = IsEnable(value.Status, Read.Request);
                Erase.Value = IsEnable(value.Status, Erase.Request);
                FlashUnlocked.Value = IsEnable(value.Status, FlashUnlocked.Request);
            }
        }
    }
}
