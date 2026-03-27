using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public class InvenManager : SingletonPersistent<InvenManager>
    {
        
        private ItemStorage _storage;

        public ItemStorage Storage
        {
            get
            {
                if (_storage == null) _storage = new ItemStorage("ItemStorage");
                return _storage;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GameManager.instance.OnPlayerDestroy.AddListener(_ => HardReset());
        }

        public void HardReset()
        {
        }
    }
}