using UnityEngine;

namespace Apis.Destroyable
{
    public class CreateGermDestroyable: DestroyableObject
    {
        public int germId;
        public int amount;
        public int germCount;

        protected override void DestroyObj(EventParameters parameters)
        {
            base.DestroyObj(parameters);
            Germ[] germs = GameManager.Item.Germ.CreateNews(germId, germCount);
            foreach (var germ in germs)
            {
                if (germ == null)
                {
                    Debug.Log("Germ 설정 오류");
                    continue;
                }
                germ.transform.position = transform.position;
                germ.Amount = amount;
                germ.Dropping();
            }
            gameObject.SetActive(false);
        }
    }
}