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
using System.Windows.Controls.Primitives;
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
            _onkyoController.StateUpdated += OnkyoController_StateUpdated;
            PopulateInputSelections();
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
                VolumeSlider.Value = state.MasterVolume;
                InputSelectionComboBox.SelectedValue = e.UpdatedState.CurrentInput;

                MutedImage.Source = state.Muted
                    ? new BitmapImage(new Uri("Resources/Mute.png", UriKind.Relative))
                    : new BitmapImage(new Uri("Resources/Speaker.png", UriKind.Relative));
            });
        }

        private void TogglePowerBtn_Click(object sender, RoutedEventArgs e) => _onkyoController.TogglePower();
        private void muteBtn_Click(object sender, RoutedEventArgs e) => _onkyoController.MuteVolume();

        private void InputSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems;
            if (items.Count == 1)
            {
                _onkyoController.SwitchInput(items[0].ToString());
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VolumeLabel == null) return;

            var val = (int) e.NewValue;
            VolumeLabel.Content = val;
        }

        private void VolumeSlider_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _onkyoController.SetVolume((int) ((Slider) sender).Value);
        }

        private void VolumeSlider_KeyUp(object sender, KeyEventArgs e)
        {
            _onkyoController.SetVolume((int) ((Slider) sender).Value);
        }

        private void VolumeSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _onkyoController.SetVolume((int) ((Slider) sender).Value);
        }
    }
}