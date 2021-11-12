using System;
using System.Collections.Generic;
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

namespace BootloaderSEX1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            xSupport.Context = this;

            InitializeComponent();

            MenuTcp.Click += WindowTcpConnection.OpenClick;
            MenuTerminal.Click += WindowTerminal.OpenClick;

            Closed += MainWindowClosed;
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            WindowTcpConnection.Dispose();
            WindowTerminal.Dispose();
        }
    }
}
