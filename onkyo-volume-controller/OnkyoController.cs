using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using EvtSource;
using RestSharp;

namespace onkyo_volume_controller
{
    internal class OnkyoController
    {
        private const string UserAgent = "OnkyoVolumeController";
        private const string BaseUrl = "http://luna.edholm.it/eiscp-rest";
        private const string SsePath = "http://luna.edholm.it/eiscp-rest/state/updates";

        private readonly IRestClient _restClient = new RestClient(BaseUrl);
        private readonly EventSourceReader _sseClient = new EventSourceReader(new Uri(SsePath));

        public event EventHandler<StateUpdateEventArgs> StateUpdated;

        public OnkyoController()
        {
            _restClient.UserAgent = UserAgent;
            _sseClient.MessageReceived += _sseClient_MessageReceived;
            StartSseClient();
        }

        private void StartSseClient()
        {
            Task.Run(() => { _sseClient.Start(); });
        }

        private void _sseClient_MessageReceived(object sender, EventSourceMessageEventArgs e)
        {
            CurrentState = new JavaScriptSerializer().Deserialize<State>(e.Message);
            OnStateUpdated(new StateUpdateEventArgs(CurrentState));
        }

        private static RestRequest ToPath(string path) => new RestRequest(path);

        private IRestResponse Post(IRestRequest request)
        {
            var restResponse = _restClient.Post(request);
            if (!restResponse.IsSuccessful)
            {
                // TODO log
            }
            return restResponse;
        }

        private IRestResponse<T> Get<T>(IRestRequest request) where T : new()
        {
            var restResponse = _restClient.Get<T>(request);
            if (!restResponse.IsSuccessful)
            {
                // TODO log
            }
            return restResponse;
        }

        protected virtual void OnStateUpdated(StateUpdateEventArgs e) => StateUpdated?.Invoke(this, e);

        public void TogglePower() => Post(ToPath("/power/toggle"));

        public void IncreaseVolume() => Post(ToPath("/volume/increase"));

        public void DecreaseVolume() => Post(ToPath("/volume/decrease"));

        public void MuteVolume() => Post(ToPath("/volume/toggle-mute"));

        public void SetVolume(int newVolume) => Post(ToPath($"/volume/{newVolume}"));

        public List<string> AvailableInputs() => Get<List<string>>(ToPath("/input/available")).Data;

        public void SwitchInput(string newInput) => Post(ToPath($"/input/{newInput}"));

        public State CurrentState { get; private set; }
    }
}