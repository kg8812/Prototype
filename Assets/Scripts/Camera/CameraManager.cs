using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class CameraManager : SingletonPersistent<CameraManager>
    {
        [HideInInspector] public GameObject fakePlayerTarget;

        // 자동 init
        private Camera _mainCam;

        private Player _player;
        private Camera _uiCam;

        
        protected override void Awake()
        {
            base.Awake();
            
            MainCamCullingMask = MainCam.cullingMask;


            GameManager.instance.playerRegistered.AddListener(p => _player = p);
            fakePlayerTarget = new GameObject("FakePlayerTarget");
            fakePlayerTarget.transform.SetParent(transform);
        }

        private void Start()
        {
        }

        public void ToggleMainCamCullingMask(bool isOn)
        {
            MainCam.cullingMask = isOn ? MainCamCullingMask : 0;
        }


        #region 인스펙터

        [Title("기획 세팅")] [LabelText("기본 groupFramingSize")] [Range(0.1f, 1f)] [SerializeField]
        private float groupFramingSize = 0.8f;

        [LabelText("최소 카메라 거리")] [SerializeField]
        private float minDistance = 10;

        [LabelText("최대 카메라 거리")] [SerializeField]
        private float maxDistance = 20;

        [InfoBox("최소, 최대거리 이내로 설정해주세요.")] [LabelText("카메라 초기 거리")] [SerializeField]
        private float camDistance;

        #endregion

        #region Getter

        // camera들
        public Camera MainCam
        {
            get
            {
                if (_mainCam == null) _mainCam = transform.GetChild(0).GetComponent<Camera>();
                return _mainCam;
            }
        }

        public Camera UICam
        {
            get
            {
                if (_uiCam == null) _uiCam = transform.GetChild(1).GetComponent<Camera>();
                return _uiCam;
            }
        }

        public int MainCamCullingMask { get; private set; }


        #endregion

        #region 플레이어 카메라 조작

        [HideInInspector] public float playerCamX;
        [HideInInspector] public float playerCamY;

        [LabelText("카메라 조작거리")] public float lookAheadDistance;

        private Vector3 _playerCamOffset;

        private void LateUpdate()
        {
            if (!ReferenceEquals(null, GameManager.instance.Player))
            {
                _playerCamOffset = new Vector3(playerCamX, playerCamY, 0).normalized * lookAheadDistance;
                
                fakePlayerTarget.transform.position =
                    GameManager.instance.Player.transForCamGroup.position + _playerCamOffset;
            }
        }

        #endregion
    }
}