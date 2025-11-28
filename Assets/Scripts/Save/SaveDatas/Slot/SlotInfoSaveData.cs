using System;
using UnityEngine;

namespace Save.Schema
{
    public class SlotInfoSaveData : SlotSaveData
    {
        public DateTime LastPlayTime;
        public int Lv;
        public float PlayTime;


        protected override void OnBeforeSave()
        {
            Lv = GameManager.instance.Level;
            LastPlayTime = DateTime.Now;
            PlayTime = Mathf.RoundToInt(GameManager.instance.playTime);
        }

        protected override void BeforeLoaded()
        {
            GameManager.instance.playTime = PlayTime;
        }

        protected override void OnReset()
        {
            Lv = 1;
            LastPlayTime = DateTime.Now;
            // 70001 맵박스 이름.
            PlayTime = 0;

            GameManager.instance.playTime = PlayTime;
        }
    }
}