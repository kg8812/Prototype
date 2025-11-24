using Apis;
using Apis.Destroyable;
using UnityEngine.Serialization;

namespace _02_Scripts.Stage.Objects.StageObject.Destroyable
{
    public class MultiCreateGoldDestroyable: DestroyableObject
    {
        public int defaultGoldId;
        public int amount;
        public int lastAmount;
        public int maxCnt = 4;

        private int _cnt;
        protected override void Awake()
        {
            base.Awake();
            _cnt = maxCnt;
        }

        protected override void DestroyObj(EventParameters parameters)
        {
            _cnt--;
            if (_cnt <= 0)
            {
                base.DestroyObj(parameters);
            }
            
            Gold_Drop[] golds = GameManager.Item.Gold.CreateNews(defaultGoldId, _cnt <= 0 ? lastAmount : amount);
            foreach (var gold in golds)
            {
                gold.transform.position = transform.position;
                gold.Dropping();
            }
            if (_cnt <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}