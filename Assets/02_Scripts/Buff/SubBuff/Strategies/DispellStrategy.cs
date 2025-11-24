
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
            Actor actor;
            public void OnAdd(Buff buff)
            {
                actor = buff.buffActor;

                if (actor == null) return;
                
                actor.AddEvent(EventType.OnAfterHit, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                if (actor == null) return;
                actor.RemoveEvent(EventType.OnAfterHit, buff.RemoveBuff);
            }
        }
        public class OnAttackSuccess : IDispell
        {
            Actor actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffActor;
                if (actor == null) return;

                actor.AddEvent(EventType.OnAttackSuccess, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                if (actor == null) return;

                actor.RemoveEvent(EventType.OnAttackSuccess, buff.RemoveBuff);
            }
        }
        public class OnDeath : IDispell
        {
            Actor actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffActor;
                if (actor == null) return;

                actor.AddEvent(EventType.OnDeath, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                if (actor == null) return;

                actor.RemoveEvent(EventType.OnDeath, buff.RemoveBuff);
            }
        }
        public class OnMasterHit : IDispell
        {
            Actor master;
            public void OnAdd(Buff buff)
            {
                if(buff.buffActor is Summon summon)
                {
                    master = summon.Master;
                    if (master == null) return;

                    master.AddEvent(EventType.OnAfterHit, buff.RemoveBuff);
                }
            }

            public void OnRemove(Buff buff)
            {
                if(master != null)
                {
                    master.RemoveEvent(EventType.OnAfterHit, buff.RemoveBuff);
                }
            }
        }

        public class OnAttackEnd : IDispell
        {
            Actor actor;

            public void OnAdd(Buff buff)
            {
                actor = buff.buffActor;
                if (actor == null) return;

                actor.AddEvent(EventType.OnAttackEnd, buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                if (actor == null) return;

                actor.RemoveEvent(EventType.OnAttackEnd, buff.RemoveBuff);
            }
        }

        public class OnSubBuffRemove : IDispell
        {
            private Actor actor;
            public void OnAdd(Buff buff)
            {
                actor = buff.buffActor;
                if (actor == null) return;
                
                actor.AddEvent(EventType.OnSubBuffRemove,buff.RemoveBuff);
            }

            public void OnRemove(Buff buff)
            {
                if (actor == null) return;
                
                actor.RemoveEvent(EventType.OnSubBuffRemove,buff.RemoveBuff);
            }
        }
    }
}