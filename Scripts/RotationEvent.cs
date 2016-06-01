using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.BeMoBI.Scripts
{
    [Serializable]
    public class RotationEvent : UnityEvent<RotationEventArgs> { }
    
    [Serializable]
    public class RotationEventArgs
    {
        public enum State { Begin, End }

        public State state;
    }
}
