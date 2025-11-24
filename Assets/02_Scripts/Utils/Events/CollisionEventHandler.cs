using Default;
using UnityEngine;

namespace Apis
{
    public class CollisionEventHandler : MonoBehaviour ,IEventChild
    {
        private IEventUser _user;

        public void Init(IEventUser user)
        {
            _user = user;
        }
        
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                EventParameters parameters = new(_user)
                {
                    collideData = new(){collider = collision},
                };
                parameters.target = collision.transform.GetComponentInParentAndChild<IOnHit>();
                _user.EventManager.ExecuteEvent(EventType.OnTriggerEnter, parameters);
            }
        }
        
        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision != null)
            {
                EventParameters parameters = new(_user)
                {
                    collideData = new(){collider = collision},
                };

                IOnHit target = Utils.GetComponentInParentAndChild<IOnHit>(collision.gameObject);
                
                parameters.target = target;
                
                _user.EventManager.ExecuteEvent(EventType.OnTriggerExit, parameters);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            EventParameters parameters = new(_user)
            {
                collideData = new(){collider = other.collider},
            };
            
            IOnHit target = Utils.GetComponentInParentAndChild<IOnHit>(other.gameObject);
            parameters.target = target;
            
            _user.EventManager.ExecuteEvent(EventType.OnCollide,parameters);
        }
    }
}