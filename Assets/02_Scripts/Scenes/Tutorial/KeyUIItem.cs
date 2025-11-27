using UnityEngine;
using UnityEngine.Events;

namespace Scenes.Tutorial
{
    public class KeyUIItem : MonoBehaviour
    {
        public UnityEvent whenTriggerEntered = new();

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player")) whenTriggerEntered.Invoke();
        }
    }
}