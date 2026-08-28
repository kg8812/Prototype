using GameStateSpace;
using Managers;
using TMPro;
using UnityEngine;

namespace UtilSpace
{
    public class Debugger : SingletonPersistent<Debugger>
    {
        [SerializeField] private TextMeshProUGUI gameStateText;
        [SerializeField] private TextMeshProUGUI timeScaleText;
        [SerializeField] private Canvas canv;

        protected override void Awake()
        {
            base.Awake();
            SetStateText(GameManager.instance.CurState);
            GameManager.instance.GameStateChangedTo.AddListener(SetStateText);
            canv.worldCamera = CameraManager.instance.UICam;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) canv.enabled = !canv.enabled;

            timeScaleText.text = $"speed: {Time.timeScale}";
        }

        private void SetStateText(GameState state)
        {
            gameStateText.text = state == null ? "-" : state.GetType().Name;
        }
    }
}