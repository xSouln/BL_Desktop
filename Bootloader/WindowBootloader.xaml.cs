using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xLib;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WindowBootloader : Window
    {
        public ObservableCollection<RowStructure> Structures { get; set; } = new ObservableCollection<RowStructure>();
        public WindowBootloader()
        {
            xSupport.Context = this;

            InitializeComponent();

            Bootloader.Init();

            MenuTcp.Click += WindowTcpConnection.OpenClick;
            MenuTerminal.Click += WindowTerminal.OpenClick;
            MenuSerialPort.Click += WindowSerialPort.OpenClick;

            WindowSerialPort.SerialPort = Bootloader.SerialPort;
            WindowTcpConnection.Tcp = Bootloader.Tcp;

            ListViewStructures.ItemsSource = Structures;
            MenuFileOpen.Click += FileOpenClick;

            Closed += MainWindowClosed;
        }

        private unsafe void FileOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();

            if (OPF.ShowDialog() == true)
            {
                RowStructure[] structures = HexReader.Read(OPF.FileName);
                Structures.Clear();
                foreach (RowStructure row in structures)
                {
                    Structures.Add(row);
                }

                byte[] firmware_data = HexReader.GetFlash(structures);
                byte[] flash_data = new byte[0x10000 - 0x8000];
                int firmware_data_size = HexReader.ToFlashAdd(structures, flash_data);

                int i = firmware_data_size;

                while (i < flash_data.Length - sizeof(FlashInfoT))
                {
                    flash_data[i] = 0xff;
                    i++;
                }

                fixed (byte* ptr = flash_data)
                {
                    FlashInfoT* info = (FlashInfoT*)(ptr + flash_data.Length - sizeof(FlashInfoT));
                    info->StartAddress = 0x8000;
                    info->EndAdress = 0x10000;
                    info->Crc = 12345;
                }
            }
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            WindowTcpConnection.Dispose();
            WindowTerminal.Dispose();
            WindowSerialPort.Dispose();

            Bootloader.Dispose();
        }
    }
}
