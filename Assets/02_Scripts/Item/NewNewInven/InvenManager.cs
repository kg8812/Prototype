using System.Collections.Generic;
using NewNewInvenSpace;
using UnityEngine;
using InvenType = NewNewInvenSpace.InvenType;

namespace Apis
{
    public class InvenManager : SingletonPersistent<InvenManager>
    {
        [SerializeField] private int wpEquipCnt;
        [SerializeField] private int wpEquipMaxCnt;
        [SerializeField] private int wpInvenCnt;
        [SerializeField] private int accEquipCnt;
        [SerializeField] private int accEquipMaxCnt;
        [SerializeField] private int accInvenCnt;
        [SerializeField] private int guitarInvenCnt;
        [SerializeField] private int guitarInvenMaxCnt;

        private AccInventoryGroup _acc;
        private AtkItemInventoryGroup _attackItem;
        private GuitarInventoryGroup _guitarInven;
        private PresetManager _presetManager;

        private ItemStorage _storage;

        public ItemStorage Storage
        {
            get
            {
                if (_storage == null) _storage = new ItemStorage("ItemStorage");
                return _storage;
            }
        }

        public PresetManager PresetManager
        {
            get
            {
                if (_presetManager == null)
                    _presetManager = new PresetManager(
                        new Dictionary<InvenGroupType, InventoryGroup>
                        {
                            { InvenGroupType.Acc, Acc },
                            { InvenGroupType.AtkItem, AttackItem }
                        }
                    );

                return _presetManager;
            }
        }

        public AccInventoryGroup Acc
        {
            get
            {
                if (_acc == null) _acc = new AccInventoryGroup(accEquipMaxCnt, accEquipCnt, accInvenCnt, accInvenCnt);

                return _acc;
            }
        }

        public AtkItemInventoryGroup AttackItem
        {
            get
            {
                if (_attackItem == null)
                    _attackItem = new AtkItemInventoryGroup(wpEquipMaxCnt, wpEquipCnt, wpInvenCnt, wpInvenCnt);

                return _attackItem;
            }
        }

        public GuitarInventoryGroup GuitarInven
        {
            get
            {
                if (_guitarInven == null) _guitarInven = new GuitarInventoryGroup(guitarInvenMaxCnt, guitarInvenCnt);

                return _guitarInven;
            }
        }


        protected override void Awake()
        {
            base.Awake();
            GameManager.instance.OnPlayerDestroy.AddListener(_ => HardReset());
        }

        public void HardReset()
        {
            Acc.Invens[InvenType.Equipment].Clear();
            Acc.Invens[InvenType.Storage].Clear();
            AttackItem.Invens[InvenType.Equipment].Clear();
            AttackItem.Invens[InvenType.Hidden].Clear();
            AttackItem.Invens[InvenType.Storage].Clear();
            GuitarInven.AllClear();
        }
    }
}