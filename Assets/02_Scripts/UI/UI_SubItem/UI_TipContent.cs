using System.Collections;
using Apis;
using Apis.DataType;
using Apis.Managers;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace _02_Scripts.UI.UI_SubItem
{
    public class UI_TipContent: UI_Base
    {
        enum Imgs
        {
            CoverImg
        }
        enum Texts
        {
            TipTitle,
            ContentText
        }

        enum VideoPlayers
        {
            VideoPlayer
        }
        
        private TipDataType _tipData = null;
        private VideoPlayer _vPlayer;
        
        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<VideoPlayer>(typeof(VideoPlayers));
            Bind<Image>(typeof(Imgs));
            _vPlayer = Get<VideoPlayer>((int)VideoPlayers.VideoPlayer);
        }
        
        public void SetInfo(TipDataType tipData)
        {
            _tipData = tipData;
            if (tipData == null)
            {
                GetText((int)Texts.TipTitle).text = "???";
                GetText((int)Texts.ContentText).text = "???";
                _vPlayer.Stop();
                GetImage((int)Imgs.CoverImg).enabled = true;
            }
            else
            {
                
                GetImage((int)Imgs.CoverImg).enabled = false;
                UpdateText(LanguageManager.LanguageType);
                string str = tipData.helpVideo;
                if (!tipData.helpVideo.Contains("Videos/"))
                {
                    str = "Videos/" + tipData.helpVideo;
                }

                ResourceUtil.LoadAsync<VideoClip>(str, videoClip =>
                {
                    _vPlayer.clip = videoClip;
                    PlayVideo();
                }, () => Debug.LogError($"{str} video 읽기 실패"));
            }
        }
        
        public void UpdateText(LanguageType lType)
        {
            // 원칙적으로 string table에 tip을 기재하는 것이 맞지만
            // 예외적으로 tip dataTable에서 kor, eng 관리.
            // 너무 예외적이다라면 추후 string table로 옮기는 거 고려
            
            if (_tipData == null) return;
            GetText((int)Texts.TipTitle).text = lType == LanguageType.Korean
                ? _tipData.tipNameKor
                : _tipData.tipNameEng;
            GetText((int)Texts.ContentText).text = lType == LanguageType.Korean
                ? _tipData.tipDescKor
                : _tipData.tipDescEng;
        }
        
        private void PlayVideo()
        {
            _vPlayer.Stop();
            _vPlayer.Prepare();
            _vPlayer.Play();
        }
        
        
        protected override void Deactivated()
        {
            base.Deactivated();
            _tipData = null;
        }
    }
}