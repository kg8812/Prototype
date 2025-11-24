using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerState {
    public interface IAutoEscape
    {
        public bool EscapeCondition();
        public EPlayerState NextState { get; set; }
    }
}
