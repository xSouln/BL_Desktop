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
using xLib.UI_Propertys;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WindowBootloader : Window
    {
        public WindowBootloader()
        {
            xSupport.Context = this;

            InitializeComponent();

            Bootloader.Init();

            Bootloader.SerialPort.SerialPortOptions = Properties.Settings.Default.SerialPortOptions;
            Bootloader.Tcp.LastAddress = Properties.Settings.Default.TcpAddress;

            MenuTcp.Click += WindowTcpConnection.OpenClick;
            MenuTerminal.Click += WindowTerminal.OpenClick;
            MenuSerialPort.Click += WindowSerialPort.OpenClick;

            WindowSerialPort.SerialPort = Bootloader.SerialPort;
            WindowTcpConnection.Tcp = Bootloader.Tcp;

            ListViewStructures.ItemsSource = Bootloader.Colections.Structures;
            ListViewPropertys.ItemsSource = Bootloader.Colections.Propertys;

            Bootloader.SerialPort.ConnectionStateChanged += SerialPortConnectionStateChanged;
            Bootloader.Tcp.EventConnected += TcpEventConnected;

            MenuFileOpen.Click += FileOpenClick;

            Closed += MainWindowClosed;
        }

        private void TcpEventConnected(xTcp arg)
        {
            Properties.Settings.Default.TcpAddress = Bootloader.Tcp.LastAddress;
            Properties.Settings.Default.Save();
        }

        private void SerialPortConnectionStateChanged(xSerialPort obj, bool state)
        {
            if (state)
            {
                Properties.Settings.Default.SerialPortOptions = Bootloader.SerialPort.SerialPortOptions;
                Properties.Settings.Default.Save();
            }
        }

        private unsafe void FileOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();

            if (OPF.ShowDialog() == true)
            {
                RowStructure[] structures = HexReader.Read(OPF.FileName);

                Bootloader.Colections.Structures.Clear();
                foreach (RowStructure row in structures)
                {
                    Bootloader.Colections.Structures.Add(row);
                }

                //Bootloader.Firmware = HexReader.GetFlash(structures);
                //Bootloader.Firmware = new byte[0x10000 - 0x8000];
                //int firmware_data_size = HexReader.ToFlashAdd(structures, Bootloader.Firmware);

                //int i = firmware_data_size;
                /*
                while (i < Bootloader.Firmware.Length - sizeof(FlashInfoT))
                {
                    Bootloader.Firmware[i] = 0xff;
                    i++;
                }

                fixed (byte* ptr = Bootloader.Firmware)
                {
                    FlashInfoT* info = (FlashInfoT*)(ptr + Bootloader.Firmware.Length - sizeof(FlashInfoT));
                    info->StartAddress = 0x8000;
                    info->EndAdress = 0x10000;
                    info->Crc = 12345;
                }
                */
                Bootloader.Firmware = HexReader.GetFlash(structures);
            }
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            Bootloader.Dispose();

            WindowTcpConnection.Dispose();
            WindowTerminal.Dispose();
            WindowSerialPort.Dispose();
        }

        private void ButLoadStart_Click(object sender, RoutedEventArgs e)
        {
            Bootloader.StartLoad(0x08008000, Bootloader.Firmware, 256);
        }

        private void ListViewPropertys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewPropertys.SelectedValue != null && ListViewPropertys.SelectedValue is UI_Property property) { property.Select(); }
            ListViewPropertys.UnselectAll();
        }

        private void ButErase_Click(object sender, RoutedEventArgs e)
        {
            Bootloader.Erase(0x08008000, 0x400, (0x0801FFFF - 0x08008000) / 0x400);
        }

        private void ButJumpToMain_Click(object sender, RoutedEventArgs e)
        {
            Bootloader.JumpToMain();
        }

        private void ButLoadStop_Click(object sender, RoutedEventArgs e)
        {
            Bootloader.StopLoad();
        }
    }
}
