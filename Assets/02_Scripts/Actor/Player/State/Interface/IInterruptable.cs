using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState {
    public interface IInterruptable
    {
        public float InterruptTime { get; set; }
        public EPlayerState[] InteruptableStates { get;}
    }
}
