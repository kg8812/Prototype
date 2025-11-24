using System;
using GameStateSpace;
using UnityEngine;

namespace Apis.Model
{
    [RequireComponent(typeof(SectorObject))]
    public class SectorFoundObject: MonoBehaviour, IMarkerObject
    {
        private const string PlayerString = "Player";

        private string _key;
        public string Key
        {
            get
            {
                _so ??= GetComponent<SectorObject>();
                _key = _so.guid.ToString();
                return _key;
            }
        }
        
        private SectorObject _so;
        private Marker _marker;
        
        private bool _isVisible;
        private bool founded;
        
        

        private void Awake()
        {
            _so = GetComponent<SectorObject>();
            _marker = GetComponent<Marker>();
            if (GameManager.Save.currentSlotData.MapSaveData.objectFound.ContainsKey(_so.guid.ToString()))
            {
                founded = true;
                _marker?.Activate();
            }
        }

        private void Update()
        {
            if (_isVisible && !founded && GameManager.instance.Player != null && GameManager.instance.CurGameStateType != GameStateType.DefaultState)
            {
                if (ShotRayToPlayer() >= 0)
                {
                    Found();
                }
            }
        }

        private void Found()
        {
            founded = true;
            if (_so.guid != Guid.Empty)
            {
                
                GameManager.Save.currentSlotData.MapSaveData.objectFound.Add(_so.guid.ToString(), new MarkerSaveData(){position = transform.position, markerType = _marker.markerType});
                _marker?.Activate();
            }
        }
        
        private float ShotRayToPlayer()
        {
            Vector3 myRayPoint = transform.position;
            Vector2 dir = GameManager.instance.ControllingEntity.Position - myRayPoint;
            RaycastHit2D hit = Physics2D.Raycast(myRayPoint, dir, Mathf.Infinity, LayerMasks.Player | LayerMasks.GroundWall);
#if UNITY_EDITOR
            Debug.DrawRay(myRayPoint, dir,Color.green);
#endif
            if (hit && hit.collider.gameObject.CompareTag(PlayerString))
            {
                return hit.distance;
            }
            return -1;
        }

        private void OnBecameVisible()
        {
            _isVisible = true;
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }
    }
}