using Default;
using EventData;
using UnityEngine;

namespace Apis
{
    public class CollisionEventHandler : MonoBehaviour, IEventChild
    {
        private IEventUser _user;

        private void OnCollisionEnter2D(Collision2D other)
        {
            EventParameters parameters = new(_user)
            {
                collideData = new CollideEventData { collider = other.collider }
            };

            var target = Utils.GetComponentInParentAndChild<IOnHit>(other.gameObject);
            parameters.target = target;

            _user.EventManager.ExecuteEvent(EventType.OnCollide, parameters);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                EventParameters parameters = new(_user)
                {
                    collideData = new CollideEventData { collider = collision }
                };
                parameters.target = collision.transform.GetComponentInParentAndChild<IOnHit>();
                _user.EventManager.ExecuteEvent(EventType.OnTriggerEnter, parameters);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision != null)
            {
                EventParameters parameters = new(_user)
                {
                    collideData = new CollideEventData { collider = collision }
                };

                var target = Utils.GetComponentInParentAndChild<IOnHit>(collision.gameObject);

                parameters.target = target;

                _user.EventManager.ExecuteEvent(EventType.OnTriggerExit, parameters);
            }
        }

        public void Init(IEventUser user)
        {
            _user = user;
        }
    }
}