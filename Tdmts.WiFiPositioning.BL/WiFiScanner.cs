using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.UI.Xaml;

namespace Tdmts.WiFiPositioning.BL
{
    public class WiFiScanner
    {

        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        private Dictionary<string, WiFiAdapter> _wiFiAdapters = new Dictionary<string, WiFiAdapter>();
        private int _scanInterval = 0;

        private Dictionary<string, double> _position = new Dictionary<string, double>();

        public event EventHandler<Dictionary<string, double>> PositionUpdated;
        
        public WiFiScanner()
        {
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(ScanInterval);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();
        }

        private async void _dispatcherTimer_Tick(object sender, object e)
        {
            _dispatcherTimer.Stop();
            if(WiFiAdapters.Count==0)
            {
                await GetWiFiAdaptersAsync();
            }
            await UpdatePositions();
            _dispatcherTimer.Start();
        }

        public async Task<Dictionary<string, WiFiAdapter>> GetWiFiAdaptersAsync()
        {
            IReadOnlyList<WiFiAdapter> adapters = await WiFiAdapter.FindAllAdaptersAsync();
            if(adapters.Count>0)
            {
                foreach(WiFiAdapter adapter in adapters)
                {
                    if(!WiFiAdapters.ContainsKey(adapter.NetworkAdapter.NetworkAdapterId.ToString()))
                    {
                        //https://msdn.microsoft.com/en-us/library/windows.networking.connectivity.networkadapter.ianainterfacetype.aspx
                        if (adapter.NetworkAdapter.IanaInterfaceType == 71)
                        {
                            WiFiAdapters.Add(adapter.NetworkAdapter.NetworkAdapterId.ToString(), adapter);
                        }
                    }
                }
            }
            return WiFiAdapters;
        }
        
        public async Task<Dictionary<string, double>> UpdatePositions()
        {
            foreach (var wiFiAdapter in WiFiAdapters)
            {
                await wiFiAdapter.Value.ScanAsync();
                IReadOnlyList<WiFiAvailableNetwork> availableNetworks = wiFiAdapter.Value.NetworkReport.AvailableNetworks;

                foreach (WiFiAvailableNetwork availableNetwork in availableNetworks)
                {
                    if (Position.ContainsKey(availableNetwork.Ssid))
                    {
                        Position[availableNetwork.Ssid] = availableNetwork.NetworkRssiInDecibelMilliwatts;
                        PositionUpdated(this, Position);
                    }
                    else
                    {
                        Position.Add(availableNetwork.Ssid, availableNetwork.NetworkRssiInDecibelMilliwatts);
                    }
                }
            }
            return Position;
        }

        public double GetDistanceFromRssiAndTxPowerOn1m(double rssi, int txPower)
        {
            /*
             * RSSI = TxPower - 10 * n * lg(d)
             * n = 2 (in free space)
             * 
             * d = 10 ^ ((TxPower - RSSI) / (10 * n))
             */
            return Math.Pow(10, ((double)txPower - rssi) / (10 * 2));
        }

        public Dictionary<string, double> Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        public Dictionary<string, WiFiAdapter> WiFiAdapters
        {
            get
            {
                return _wiFiAdapters;
            }

            set
            {
                _wiFiAdapters = value;
            }
        }

        public int ScanInterval
        {
            get
            {
                return _scanInterval;
            }

            set
            {
                if(value<5000)
                {
                    value = 5000;
                }
                _scanInterval = value;
            }
        }
    }
}
