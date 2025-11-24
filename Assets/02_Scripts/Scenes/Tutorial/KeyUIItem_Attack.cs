using Apis;
using NewNewInvenSpace;
using Scenes.Tutorial;
using UnityEngine;

namespace _02_Scripts.Scenes.Tutorial
{
    public class KeyUIItem_Attack: KeyUIItem
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !InvenManager.instance.AttackItem.Invens[InvenType.Equipment].IsEmpty())
            {
                whenTriggerEntered.Invoke();
            }
        }
    }
}