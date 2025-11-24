using System;
using System.Collections.Generic;
using System.Linq;
using Apis.Components;
using UnityEngine;

namespace Apis
{
    [RequireComponent(typeof(Trigger))]
    public class IgnoreTimeScaleTrigger: MonoBehaviour
    {
        private static Action _forceUpdate;

        [SerializeField] private Trigger trigger;
        [SerializeField] private Collider2D col;
        
        
        private HashSet<Collider2D> _prevTriggerHits = new();
        private bool _use;
        private List<Collider2D> _results;
        private ContactFilter2D _filter;
        
        private void Awake()
        {
            col ??= GetComponent<Collider2D>();
            
            _use = col != null;
            if (_use)
            {
                trigger ??= GetComponent<Trigger>();
                _results = new List<Collider2D>();
                
                _filter = new ContactFilter2D();
                _filter.useTriggers = true;
                _filter.SetLayerMask(Physics2D.GetLayerCollisionMask(col.gameObject.layer));
            }
        }

        private void OnEnable()
        {
            if(_use)
                _forceUpdate += ForceUpdateTriggerAction;
        }

        private void OnDisable()
        {
            _forceUpdate -= ForceUpdateTriggerAction;
        }
        
        public static void ForceUpdateTrigger()
        {
            _forceUpdate?.Invoke();
        }
        
        private void ForceUpdateTriggerAction()
        {
            if (!_use) return;
            
            col.OverlapCollider(_filter, _results);

            foreach (var prev in _prevTriggerHits)
            {
                if (!_results.Contains(prev))
                {
                    trigger.OnTriggerExit2D(prev);
                }
            }

            foreach (var hit in _results)
            {
                if (_prevTriggerHits.Contains(hit))
                {
                    trigger.OnTriggerEnter2D(hit);
                }
            }

            _prevTriggerHits = _results.ToHashSet();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            _prevTriggerHits.Add(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _prevTriggerHits.Remove(other);
        }
    }
}