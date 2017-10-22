using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace onkyo_volume_controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRestClient _restClient = new RestClient("http://luna.edholm.it:9191");
        private State _lastState;

        public MainWindow()
        {
            InitializeComponent();
            var requestNewState = new RestRequest("/state", Method.POST);
            _restClient.Post(requestNewState);
            RefreshState();
        }

        private void RefreshState()
        {
            var getCurrentState = new RestRequest("/state", Method.GET);
            var response = _restClient.Execute<State>(getCurrentState);
            _lastState = response.Data;
            volumeLabel.Content = _lastState.MasterVolume;
            volumeSlider.Value = _lastState.MasterVolume;
        }

        private void discoverButton_Click(object sender1, RoutedEventArgs rea)
        {
            RefreshState();
        }

        private void pwrButton_Click(object sender, RoutedEventArgs e)
        {
            var request = _lastState.IsPowered
                ? new RestRequest("/power/on", Method.POST)
                : new RestRequest("/power/off", Method.POST);

            _restClient.Post(request);
            RefreshState();
        }
    }
}