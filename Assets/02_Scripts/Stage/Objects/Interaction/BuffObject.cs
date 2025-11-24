using System;
using Apis;
using Default;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuffObject : MonoBehaviour,IOnInteract
{

    public int buffGroupIndex;

    public Func<bool> InteractCheckEvent { get; set; }

    public void OnInteract()
    {
        if (BuffDatabase.DataLoad.TryGetBuffList(buffGroupIndex, out var datas))
        {
            datas = Utils.GetChanceList(datas);

            int rand = Random.Range(0, datas.Count);

            if (BuffDatabase.DataLoad.TryGetBuff(datas[rand].buffIndex, out var buff))
            {
                SubBuff sub = SubBuffResources.Get(buff);
                GameManager.instance.Player.AddSubBuff(GameManager.instance.Player,buff,sub);
                Destroy(gameObject);
            }
        }
    }
}
