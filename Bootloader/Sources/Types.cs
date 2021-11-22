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
                GET_INFO,
                GET_HANDLER,

                SET_X = 200,
                SET_OPTIONS,
                SET_HANDLER,

                TRY_X = 300,
                TRY_ERASE,
                TRY_WRITE,
                TRY_READ,
                TRY_STOP,
                TRY_JUMP_TO_MAIN,
                TRY_JUMP_TO_BOOT,

                EVT_X = 400,
                EVT_WRITE_COMPLITE,
                EVT_READ_COMPLITE,

                REQUESTS_END = 500
            }

            public struct FlashInfoT
            {
                public uint StartAddress;
                public uint EndAdress;
                public ushort Crc;
                public ushort Handler;
            }
        }
    }
}
