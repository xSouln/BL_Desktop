using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;

namespace BootloaderDesktop
{
    public enum HexWriteType
    {
        Write,
        EndFile,
        Segment,
        ApplicationStartAddress,
        WriteExtendedAddress,
        ApplicationStartLinearAddress
    }

    public class HexReader
    {
        public const int CHECKSUM_SIZE = 2;
        public const string END_ROW = "\r\n";
        public const int PREFIX_SIZE = 9;
        public const byte START_CHARECTAR = (byte)':';

        public const string COMMAND_WRITE = "00";
        public const int COMMAND_KEY_START_INDEX_0 = 7;
        public const int DATA_START_INDEX_0 = 9;

        public static RowStructure[] GetFileStructure(string content)
        {
            List<RowStructure> file_structure = new List<RowStructure>();

            string[] rows = xConverter.Split(content, END_ROW);

            foreach (string row in rows)
            {
                if (row.Length < PREFIX_SIZE + CHECKSUM_SIZE) { return null; }

                file_structure.Add(new RowStructure { Content = row });
            }

            return file_structure.ToArray();
        }

        public static byte[] GetFlash(RowStructure[] structures)
        {
            List<byte> flash = new List<byte>();

            if (structures != null)
            {
                foreach (RowStructure structure in structures)
                {
                    if (structure.WriteType == HexWriteType.Write)
                    {
                        structure.ToListAdd(flash, 0x10);
                    }
                }
            }
            return flash.ToArray();
        }

        public static unsafe int ToFlashAdd(RowStructure[] structures, byte[] flash)
        {
            int size = 0;
            if (structures != null)
            {
                foreach (RowStructure structure in structures)
                {
                    if (structure.WriteType == HexWriteType.Write)
                    {
                        size += structure.ToArrayAdd(flash, size, 0x10);
                    }
                }
            }
            return size;
        }

        public static RowStructure[] Read(string path)
        {
            if (path == null) { return null; }
            FileInfo file_info;
            byte[] data;
            string content;
            try
            {
                file_info = new FileInfo(path);
                if (file_info.Length > 0)
                {
                    data = new byte[file_info.Length];
                    using (FileStream Stream = new FileStream(path, FileMode.Open))
                    {
                        Stream.Read(data, 0, (int)file_info.Length);
                        content = Encoding.UTF8.GetString(data);
                        return GetFileStructure(content);
                    }
                }
            }
            catch (Exception ex)
            {
                xTracer.Message("" + ex);
            }
            return null;
        }

        private static unsafe ushort calculate_crc(void* obj, int obj_size)
        {
            ushort crc_value = 0;
            byte* data = (byte*)obj;
            for (int i = obj_size; i > 0; i--)
            {
                crc_value ^= *data++;
                for (byte j = 0; j < 8; j++)
                {
                    if ((crc_value & 0x01) > 0) crc_value = (ushort)((crc_value >> 1) ^ 0xA001);
                    else crc_value >>= 1;
                }
            }
            return crc_value;
        }
    }
}
