using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootloaderDesktop
{
    public partial class Bootloader
    {
        public class Types
        {
            public enum EActions : ushort
            {
                REQUESTS_START = 0,

                GET_X = 100,
                GET_FIRMWARE,
                GET_INFO,
                GET_APP_INFO,
                GET_STATUS,
                GET_HANDLER,

                SET_X = 200,
                SET_LOCK_STATE,
                SET_OPTIONS,
                SET_HANDLER,

                TRY_X = 300,
                TRY_ERASE,
                TRY_WRITE,
                TRY_READ,
                TRY_STOP,
                TRY_JUMP_TO_BOOT,
                TRY_JUMP_TO_MAIN,
                TRY_UPDATE_INFO,
                TRY_READ_CRC,

                EVT_X = 400,
                EVT_WRITE_COMPLITE,
                EVT_READ_COMPLITE,

                REQUESTS_END = 500
            }

            public enum EErrors : short
            {
                Aceept,
                ErrorData,
                ErrorContentSize,
                ErrorRequest,
                ErrorResolution,
                UnknownCommand,
                Timeout,
                Busy,
                Outside,
                ErrorAction,
                Locked
            }

            public enum ELockState : byte
            {
                Unlocked,
                Locked,
            }

            public enum EStatus : uint
            {
                Write = 1 << 0,
                Read = 1 << 1,
                Erase = 1 << 2,
                FlashUnlocked = 1 << 3,
            }

            public enum EOperation : ushort
            {
                Free,
                Erase,
                Write,
                Read,
                Lock
            }

            public struct StatusT
            {
                public EStatus Status;

                public uint WriteAddress;
                public uint EraseAddress;
                public uint ReadAddress;

                public EOperation Operation;
                public ushort OperationTime;
                public EErrors OperationResult;
            }

            public struct FlashInfoT
            {
                public uint StartAddress;
                public uint EndAdress;
                public uint Requests;
                public ushort PageSize;
                public ushort Crc;
            }

            public struct AppFlashInfoT
            {
                public uint StartAddress;
                public uint EndAdress;
                public ushort PageSize;
                public ushort Crc;
            }

            public struct RequestWriteT
            {
                public uint StartAddress;
                public ushort DataSize;
                public ushort Action;
            }

            public struct RequestReadT
            {
                public uint StartAddress;
                public ushort DataSize;
                public ushort Action;
            }

            public struct RequestEraseT
            {
                public uint StartAddress;
                public ushort PagesCount;
                public ushort Action;
            }

            public struct RequestCrcT
            {
                public uint StartAddress;
                public uint EndAddress;
                public uint Action;
            }
        }
    }
}
