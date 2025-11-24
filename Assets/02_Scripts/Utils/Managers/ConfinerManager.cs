using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Apis.Managers
{
    [System.Serializable]
    public struct ConfinerEntity
    {
        public BoxCollider2D box;
        public PolygonCollider2D poly;
        public bool isPoly;
        public int priority;
    }
    public class ConfinerManager: SingletonPersistent<ConfinerManager>
    {
        public List<ConfinerEntity> colliders;
        HashSet<Collider2D> removeHash = new ();
        
        protected override void Awake()
        {
            base.Awake();
            colliders = new();
            GameManager.Scene.WhenSceneLoadBegin.AddListener(_ => RemoveAllConfiner());
        }


        public void RegisterConfiner(BoxCollider2D box, int priority)
        {
            if (removeHash.Contains(box))
            {
                removeHash.Remove(box);
                return;
            }
            ConfinerEntity ce = new ConfinerEntity();
            ce.priority = priority;
            ce.box = box;
            ce.isPoly = false;
            RegisterConfinerEntity(ce);
        }
        
        public void RegisterConfiner(PolygonCollider2D poly, int priority)
        {
            if (removeHash.Contains(poly))
            {
                removeHash.Remove(poly);
                return;
            }
            ConfinerEntity ce = new ConfinerEntity();
            ce.priority = priority;
            ce.poly = poly;
            ce.isPoly = true;
            RegisterConfinerEntity(ce);
        }

        public void RemoveConfiner(BoxCollider2D box)
        {
            bool isRemoved = false;
            
            for (int i = 0; i < colliders.Count; i++)
            {
                if (ReferenceEquals(colliders[i].box, box))
                {
                    RemoveConfiner(i);
                    isRemoved = true;
                }
            }

            if (!isRemoved)
            {
                removeHash.Add(box);
            }
        }
        
        public void RemoveConfiner(PolygonCollider2D poly)
        {
            bool isRemoved = false;

            for (int i = 0; i < colliders.Count; i++)
            {
                if (ReferenceEquals(colliders[i].poly, poly))
                {
                    RemoveConfiner(i);
                    isRemoved = true;
                }
            }
            if (!isRemoved)
            {
                removeHash.Add(poly);
            }
        }

        public void RemoveAllConfiner()
        {
            CameraManager.instance.SetPlayerCamConfiner2D(null);
            colliders.Clear();
        }

        private void RemoveConfiner(int i)
        {
            if (i + 1 == colliders.Count)
            {
                if (i == 0)
                {
                    CameraManager.instance.SetPlayerCamConfiner2D(null);
                }else if (colliders[i - 1].isPoly)
                {
                    CameraManager.instance.SetPlayerCamConfiner2D(colliders[i-1].poly);
                }
                else
                {
                    CameraManager.instance.SetPlayerCamConfinerBox2D(colliders[i-1].box);
                }
            }
            colliders.RemoveAt(i);
        }

        private void RegisterConfinerEntity(ConfinerEntity ce)
        {
            int ind = 0;
            for (ind = 0; ind < colliders.Count; ind++)
            {
                if (colliders[ind].priority > ce.priority)
                {
                    break;
                }
            }

            if (ind >= colliders.Count)
            {
                if (ce.isPoly)
                {
                    CameraManager.instance.SetPlayerCamConfiner2D(ce.poly);
                }
                else
                {
                    CameraManager.instance.SetPlayerCamConfinerBox2D(ce.box);
                }
            }
            colliders.Insert(ind, ce);
        }
        
        
    }
}