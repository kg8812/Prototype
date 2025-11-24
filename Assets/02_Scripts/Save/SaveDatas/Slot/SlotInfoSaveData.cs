using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Apis;
using Default;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save.Schema
{
    public enum PlayerType
    {
        Ine,
        Jingburger,
        Lilpa,
        Jururu,
        Gosegu,
        Viichan
    }

    public class SlotInfoSaveData : SlotSaveData
    {
        public PlayerType PlayerType;
        public int Lv;
        public DateTime LastPlayTime;
        public int PlaceNameId;
        public float PlayTime = 0;
        public float Progress;
        

        protected override void OnBeforeSave()
        {
            PlayerType = GameManager.instance.Player.playerType;
            Lv = GameManager.instance.Level;
            LastPlayTime = DateTime.Now;
            PlaceNameId = Calc.ConcatInts(StrUtil.MapBoxNameCategory, Map.instance.CurMapBox);
            PlayTime = Mathf.RoundToInt(GameManager.instance.playTime);
            Progress = Map.instance.Progress;
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
            PlaceNameId = 1670001;
            PlayTime = 0;
            Progress = 0;
            
            GameManager.instance.playTime = PlayTime;
        }
    }
}