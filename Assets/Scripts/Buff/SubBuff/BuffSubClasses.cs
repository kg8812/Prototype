using UnityEngine;

namespace Apis
{
    public class SubBuffLifeCycleHandler
    {
        public void AfterSubBuffAdded(SubBuff sub)
        {
            sub?.OnAdd();
        }

        public void AfterSubBuffRemoved(SubBuff sub)
        {
            sub?.OnRemove();
        }

        public void AfterSubBuffMaxStackReached(SubBuff sub)
        {
            sub?.OnMaxStack();
        }
    }

    public class BuffUIHandler
    {
        
    }
}