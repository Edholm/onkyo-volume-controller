using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EvtSource;
using RestSharp.Deserializers;
using IRestClient = RestSharp.IRestClient;

namespace onkyo_volume_controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRestClient _restClient = new RestClient("http://luna.edholm.it/eiscp-rest");

        private readonly EventSourceReader _sseClient =
            new EventSourceReader(new Uri("http://luna.edholm.it/eiscp-rest/state/updates"));

        private State _lastState;

        public MainWindow()
        {
            InitializeComponent();
            _restClient.UserAgent = "OnkyoVolumeController";
            _sseClient.MessageReceived += _sseClient_MessageReceived;
            StartSSETask();
        }

        private void StartSSETask()
        {
            Task.Run(() => { _sseClient.Start(); });
        }

        private void _sseClient_MessageReceived(object sender, EventSourceMessageEventArgs e)
        {
            var newState = new JavaScriptSerializer().Deserialize<State>(e.Message);
            RefreshState(newState);
        }

        private void RefreshState(State state)
        {
            this.Dispatcher.Invoke(() =>
            {
                _lastState = state;
                volumeLabel.Content = _lastState.MasterVolume;
                volumeSlider.Value = _lastState.MasterVolume;
                inputSelectionLabel.Content = _lastState.CurrentInput;
            });
        }

        private void pwrButton_Click(object sender, RoutedEventArgs e)
        {
            var toggle = new RestRequest("/power/toggle", Method.POST);
            var restResponse = _restClient.Post(toggle);
        }

        private void incVolumeBtn_Click(object sender, RoutedEventArgs e)
        {
            var inc = new RestRequest("/volume/increase", Method.POST);
            var restResponse = _restClient.Post(inc);
        }

        private void decVolumeBtn_Click(object sender, RoutedEventArgs e)
        {
            var dec = new RestRequest("/volume/decrease", Method.POST);
            var restResponse = _restClient.Post(dec);
        }

        private void muteBtn_Click(object sender, RoutedEventArgs e)
        {
            var dec = new RestRequest("/volume/toggle-mute", Method.POST);
            var restResponse = _restClient.Post(dec);
        }
    }
}