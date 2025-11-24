using System.Collections.Generic;
using Apis;
using UI;
using Directing;
using Save.Schema;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class DirectingManager
    {
        private readonly Vector2 letterBoxRatio = new Vector2(16, 7);
        private Dictionary<int, DialogueDirector> _dialogueDirectors;

        // Director에서 접근하기 위해 추가해둠
        public Dictionary<int, DialogueDirector> DialogueDirectors => _dialogueDirectors;


        public void Init()
        {
            _dialogueDirectors = new();

            GameManager.Scene.WhenSceneLoaded.AddListener(_ => { _dialogueDirectors.Clear(); });
        }

        public void RegisterDialogueDirector(DialogueDirector obj)
        {
            _dialogueDirectors.TryAdd(obj.speakerId, obj);
        }


        public void ShowVideoCutScene(int cutsceneIndex, UnityAction endEvent = null)
        {
            DataAccess.Codex.UnLock(CodexData.CodexType.CutScene, cutsceneIndex);

            UI_CutScenePopup video = GameManager.UI.CreateUI("UI_CutScenePopup", UIType.Popup) as UI_CutScenePopup;

            if (CutSceneDatabase.TryGetCutSceneData(cutsceneIndex, out var data) && video != null)
            {
                video.OnVideoEnd.AddListener(endEvent);
                video.Init(data.video);
            }
        }


        public void CameraMoving(Vector2 offset, bool isWorldPos = false, float time = 1f)
        {
        }


        
    }
}