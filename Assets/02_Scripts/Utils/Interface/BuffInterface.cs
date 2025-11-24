using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public interface IImmunity
    {
        public ImmunityController ImmunityController { get; }
    }

    public interface IBuffUser : IImmunity,IMonoBehaviour
    {
        public SubBuffManager SubBuffManager { get; }
    }
}