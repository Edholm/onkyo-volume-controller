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
        private readonly OnkyoController _onkyoController = new OnkyoController();

        public MainWindow()
        {
            InitializeComponent();
            PopulateInputSelections();
            _onkyoController.StateUpdated += OnkyoController_StateUpdated;
        }

        private void PopulateInputSelections()
        {
            _onkyoController.AvailableInputs().ForEach((input => { InputSelectionComboBox.Items.Add(input); }));
        }

        private void OnkyoController_StateUpdated(object sender, StateUpdateEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var state = e.UpdatedState;
                volumeLabel.Content = state.MasterVolume;
                volumeSlider.Value = state.MasterVolume;
                InputSelectionComboBox.SelectedValue = e.UpdatedState.CurrentInput;
            });
        }

        private void pwrButton_Click(object sender, RoutedEventArgs e) => _onkyoController.TogglePower();
        private void incVolumeBtn_Click(object sender, RoutedEventArgs e) => _onkyoController.IncreaseVolume();
        private void decVolumeBtn_Click(object sender, RoutedEventArgs e) => _onkyoController.DecreaseVolume();
        private void muteBtn_Click(object sender, RoutedEventArgs e) => _onkyoController.MuteVolume();

        private void InputSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems;
            if (items.Count == 1)
            {
                _onkyoController.SwitchInput(items[0].ToString());
            }
        }
    }
}