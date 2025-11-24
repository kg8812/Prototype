using GameStateSpace;
using Managers;
using TMPro;
using UnityEngine;

namespace UtilSpace
{
    public class Debugger: SingletonPersistent<Debugger>
    {
        [SerializeField] private TextMeshProUGUI gameStateText;
        [SerializeField] private TextMeshProUGUI timeScaleText;
        [SerializeField] private Canvas canv;

        protected override void Awake()
        {
            base.Awake();
            SetStateText(GameStateType.DefaultState);
            GameManager.instance.GameStateChangedTo.AddListener(SetStateText);
            canv.worldCamera = CameraManager.instance.UICam;
        }

        private void SetStateText(GameStateType gameStateType)
        {
            
            gameStateText.text = gameStateType.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                canv.enabled = !canv.enabled;
            }

            timeScaleText.text = $"speed: {Time.timeScale}";
        }
    }
}