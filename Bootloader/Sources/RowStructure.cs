using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.UI_Propertys;

namespace BootloaderDesktop
{
    public class RowStructure : NotifyPropertyChanged
    {
        public const int CHECKSUM_SIZE = 2;
        public const string END_ROW = "\r\n";
        public const int PREFIX_SIZE = 9;

        private int address;
        private HexWriteType write_type;

        private string content = null;

        private byte[] data = new byte[0];
        private int crc;

        public int DataLength
        {
            get => data.Length;
            private set
            {
                data = new byte[value];
                for (int i = 0; i < data.Length; i++) { data[i] = 0xff; }
                OnPropertyChanged(nameof(DataLength));
            }
        }

        public string StringAddress => address.ToString("X2");

        public string StringWriteType => ((int)write_type).ToString("X2");

        public string StringCRC
        {
            get => crc.ToString("X2");
            private set
            {
                if (value != null && value.Length == 2) { crc = Convert.ToByte(value, 16); }
                OnPropertyChanged(nameof(StringCRC));
                OnPropertyChanged(nameof(CRC));
            }
        }

        public int Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(StringAddress));
            }
        }

        public HexWriteType WriteType
        {
            get => write_type;
            set
            {
                write_type = value;
                OnPropertyChanged(nameof(WriteType));
                OnPropertyChanged(nameof(StringWriteType));
            }
        }

        public string Content
        {
            get => content;
            set
            {
                if (value != null && value.Length >= PREFIX_SIZE + CHECKSUM_SIZE && value[0] == ':')
                {
                    int offset = 0;
                    int data_size;

                    content = value;

                    DataLength = Convert.ToInt32(xConverter.GetRange(value, 1, 2), 16);
                    Address = Convert.ToInt32(xConverter.GetRange(value, 3, 4), 16);
                    WriteType = (HexWriteType)Convert.ToInt32(xConverter.GetRange(value, 7, 2), 16);

                    offset += PREFIX_SIZE;
                    data_size = value.Length - offset - CHECKSUM_SIZE;

                    if (data_size > 0)
                    {
                        StringData = xConverter.GetRange(value, offset, data_size);
                        offset += data_size;
                    }

                    StringCRC = xConverter.GetRange(value, offset, 2);

                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public void ToListAdd(List<byte> list, int row_size)
        {
            int i = 0;

            if (list != null && data.Length > 0)
            {
                while (i < data.Length)
                {
                    list.Add(data[i]);
                    i++;
                }

                if (i > row_size) { row_size = (i / row_size * row_size) + row_size; }

                while (i < row_size)
                {
                    list.Add(0xff);
                    i++;
                }
            }
        }

        public unsafe int ToArrayAdd(byte* ptr, int row_size)
        {
            int size = 0;

            if (ptr != null && data.Length > 0)
            {
                while (size < data.Length)
                {
                    ptr[size] = data[size];
                    size++;
                }

                if (size > row_size) { row_size = (size / row_size * row_size) + row_size; }

                while (size < row_size)
                {
                    ptr[size] = 0xff;
                    size++;
                }
            }

            return size;
        }

        public unsafe int ToArrayAdd(byte[] array, int offset, int row_size)
        {
            int size = 0;

            if (array != null && data.Length > 0)
            {
                while (size < data.Length && offset < array.Length)
                {
                    array[offset] = data[size];
                    offset++;
                    size++;
                }

                if (size > row_size) { row_size = (size / row_size * row_size) + row_size; }

                while (size < row_size)
                {
                    array[offset] = 0xff;
                    offset++;
                    size++;
                }
            }

            return size;
        }

        public string StringData
        {
            get
            {
                string str = "";
                foreach (byte element in data) { str += element.ToString("X2") + " "; }
                return str;
            }
            private set
            {
                if (value != null)
                {
                    int i = 0;
                    int j = 0;
                    while (j < value.Length && i < data.Length)
                    {
                        data[i] = Convert.ToByte(xConverter.GetRange(value, j, 2), 16);
                        i++;
                        j += 2;
                    }
                    OnPropertyChanged(nameof(StringData));
                }
            }
        }

        public int CRC
        {
            get => crc;
            private set
            {
                crc = value;
                OnPropertyChanged(nameof(CRC));
                OnPropertyChanged(nameof(StringCRC));
            }
        }

        public byte[] Data
        {
            get => data;
            private set
            {
                int length = 0;
                int i = 0;

                if (value == null) { length = 0; }
                else if (value.Length > 16) { return; }
                else { length = value.Length; }

                while (i < length)
                {
                    data[i] = value[i];
                    i++;
                }

                while (i < 16)
                {
                    data[i] = 0xff;
                    i++;
                }
                OnPropertyChanged(nameof(StringData));
            }
        }
    }
}
