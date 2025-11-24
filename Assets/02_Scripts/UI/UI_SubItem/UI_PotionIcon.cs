using Default;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UI_PotionIcon: UI_Base
    {
        enum GameObjects
        {
            Layout
        }
        GameObject parent;

        public override void Init()
        {
            base.Init();
            GameManager.instance.afterPlayerStart.AddListener(x => x.OnPotionChange.AddListener(ChangeText));
            Bind<GameObject>(typeof(GameObjects));
            parent = Get<GameObject>((int)GameObjects.Layout);
            UI_MainHud.Instance.afterSet.AddListener(x => ChangeText(x.CurrentPotionCapacity));
        }

        public void ChangeText(int potionCount)
        {
            int subCount = subItems.Count;
            
            for (int i = subCount; i < potionCount; i++)
            {
                subItems.Add(GameManager.UI.MakeSubItem("PotionCount", parent.transform));
            }

            List<UI_Base> removes = new();
            
            for (int i = potionCount; i < subCount; i++)
            {
                removes.Add(subItems[i]);
            }
            
            removes.ForEach(RemoveSubItem);
        }
    }
}