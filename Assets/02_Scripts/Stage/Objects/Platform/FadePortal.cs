using Directing;
using UI;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Scenes.Tutorial
{
    public class FadePortal: MonoBehaviour
    {
        [SerializeField] private Transform toPos;
        [SerializeField] private UnityEvent otherevent;

        private bool portaled = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (portaled) return;
            Debug.Log(other.gameObject.name);
            if (other.gameObject.CompareTag("Player") && !other.isTrigger)
            {
                Portaled();
            }
        }

        public void Portaled()
        {
            portaled = true;
            FadeManager.instance.Fading(() =>
            {
                GameManager.instance.ControllingEntity.transform.position = toPos.position;
                GameManager.instance.ControllingEntity.MoveToFloor();
                TargetGroupCamera.instance.DoUpdate();
                CameraManager.instance.SetPlayerCamConfinerBox2D(null);
                CameraManager.instance.ToggleCameraFix(false);
                CameraManager.instance.InitPlayerCamPosition();
                otherevent.Invoke();
            },null,0.2f);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !other.isTrigger)
            {
                portaled = false;
            }
        }
    }
}