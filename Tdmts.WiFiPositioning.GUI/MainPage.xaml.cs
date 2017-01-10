using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Tdmts.WiFiPositioning.BL;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tdmts.WiFiPositioning.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WiFiScanner _scanner = new WiFiScanner();

        public MainPage()
        {
            this.InitializeComponent();
            _scanner.PositionUpdated += _scanner_PositionUpdated;
        }

        private void _scanner_PositionUpdated(object sender, Dictionary<string, double> e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var position in e)
            {
                double distance = Math.Round(_scanner.GetDistanceFromRssiAndTxPowerOn1m(position.Value, -29),2);

                string s = string.Format("{0} : {1} : approximately {2}m away from you.", position.Key, position.Value, distance);
                sb.AppendLine(s);
            }

            txtAccessPoints.Text = sb.ToString();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
