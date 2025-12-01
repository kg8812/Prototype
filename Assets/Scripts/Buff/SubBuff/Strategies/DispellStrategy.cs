namespace Apis
{
    public class BuffDispellStrategy
    {
        public interface IDispell
        {
            void OnAdd(Buff buff);
            void OnRemove(Buff buff);
        }

        public class Nothing : IDispell
        {
            public void OnAdd(Buff buff)
            {
            }

            public void OnRemove(Buff buff)
            {
            }
        }

        public class OnHit : IDispell
        {
            private IEventUser actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffUser.gameObject.GetComponent<IEventUser>();

                actor?.EventManager.AddEvent(EventType.OnAfterHit, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                actor?.EventManager.RemoveEvent(EventType.OnAfterHit, buff.RemoveBuff);
            }
        }

        public class OnAttackSuccess : IDispell
        {
            private IEventUser actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffUser.gameObject.GetComponent<IEventUser>();


                actor?.EventManager.AddEvent(EventType.OnAttackSuccess, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                actor?.EventManager.RemoveEvent(EventType.OnAttackSuccess, buff.RemoveBuff);
            }
        }

        public class OnDeath : IDispell
        {
            private IEventUser actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffUser.gameObject.GetComponent<IEventUser>();

                actor?.EventManager.AddEvent(EventType.OnDeath, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                actor?.EventManager.RemoveEvent(EventType.OnDeath, buff.RemoveBuff);
            }
        }

        public class OnMasterHit : IDispell
        {
            private Actor master;

            public void OnAdd(Buff buff)
            {
                if (buff.buffUser.gameObject.TryGetComponent(out Summon summon))
                {
                    master = summon.Master;
                    if (master == null) return;

                    master.AddEvent(EventType.OnAfterHit, buff.RemoveBuff);
                }
            }

            public void OnRemove(Buff buff)
            {
                if (master != null) master.RemoveEvent(EventType.OnAfterHit, buff.RemoveBuff);
            }
        }

        public class OnAttackEnd : IDispell
        {
            private IEventUser actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffUser.gameObject.GetComponent<IEventUser>();


                actor?.EventManager.AddEvent(EventType.OnAttackEnd, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                actor?.EventManager.RemoveEvent(EventType.OnAttackEnd, buff.RemoveBuff);
            }
        }

        public class OnSubBuffRemove : IDispell
        {
            private IEventUser actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffUser.gameObject.GetComponent<IEventUser>();

                actor?.EventManager.AddEvent(EventType.OnSubBuffRemove, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                actor?.EventManager.RemoveEvent(EventType.OnSubBuffRemove, buff.RemoveBuff);
            }
        }
    }
}