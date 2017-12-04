using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onkyo_volume_controller
{
    internal class StateUpdateEventArgs : EventArgs
    {
        public State UpdatedState { get; }

        public StateUpdateEventArgs(State updatedState)
        {
            this.UpdatedState = updatedState;
        }
    }
}