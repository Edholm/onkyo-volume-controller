﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onkyo_volume_controller
{
    class State
    {
        public int MasterVolume { get; set; }
        public Boolean IsMuted { get; set; }
        public Boolean IsPowered { get; set; }
        public String CurrentInput { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}