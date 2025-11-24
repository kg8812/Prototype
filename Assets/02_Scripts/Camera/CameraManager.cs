using System;
using System.Collections;
using System.Collections.Generic;
using Directing;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class CameraManager : SingletonPersistent<CameraManager>
    {
        

        #region 인스펙터

        [Title("기획 세팅")] 
        [LabelText("기본 groupFramingSize")] [Range(0.1f, 1f)] [SerializeField] private float groupFramingSize = 0.8f;

        [LabelText("최소 카메라 거리")] [SerializeField]
        private float minDistance = 10;
        [LabelText("최대 카메라 거리")] [SerializeField] private float maxDistance = 20;

        [InfoBox("최소, 최대거리 이내로 설정해주세요.")]
        [LabelText("카메라 초기 거리")] [SerializeField]
        private float camDistance;

        #endregion

        #region Getter

        // 세팅
        public float Size
        {
            get => groupFramingSize;
            set
            {
                groupFramingSize = value;
                transposer.m_GroupFramingSize = value;
            }
        }
        
        public int MainCamCullingMask { get; private set; }
        
        
        // camera들
        public Camera MainCam
        {
            get
            {
                if (_mainCam == null)
                {
                    _mainCam = transform.GetChild(0).GetComponent<Camera>();
                }
                return _mainCam;
            }
        }

        public Camera UICam
        {
            get
            {
                if (_uiCam == null)
                {
                    _uiCam = transform.GetChild(1).GetComponent<Camera>();
                    
                }
                return _uiCam;
            }
        }

        public CinemachineVirtualCamera PlayerCam
        {
            get
            {
                if (_playerCam == null)
                {
                    _playerCam = transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();
                }
                return _playerCam;
            }
        }
        
        
        public CinemachineTargetGroup TargetGroup
        {
            get
            {
                if (_targetGroup == null)
                {
                    _targetGroup = transform.GetComponentInChildren<CinemachineTargetGroup>();
                }
                return _targetGroup;
            }
        }

        public CinemachineBrain CineBrain => _cineBrain;

        #endregion
        
        
        // 자동 init
        private Camera _mainCam;
        private Camera _uiCam;
        private CinemachineVirtualCamera _playerCam;
        private CinemachineTargetGroup _targetGroup;
        
        private CinemachineBrain _cineBrain;
        private Dictionary<int, CinemachineVirtualCamera> subCams;
        private Dictionary<int, CinemachineVirtualCamera[]> CutsceneCams;
        private PolygonCollider2D _confinerCol;
        
        private Player _player;
        private CinemachineConfiner2D playerCamConfiner;
        private CinemachineFramingTransposer transposer;
        
        private int CurrentSubCamId;
        
        [HideInInspector] public GameObject fakePlayerTarget;
        
        protected override void Awake()
        {
            base.Awake();
            _cineBrain = MainCam.GetComponent<CinemachineBrain>();
            _confinerCol = transform.GetComponentInChildren<PolygonCollider2D>();
            playerCamConfiner = PlayerCam.gameObject.GetComponent<CinemachineConfiner2D>();
            transposer = PlayerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_MinimumDistance = minDistance;
            transposer.m_CameraDistance = camDistance;
            transposer.m_GroupFramingSize = Size;
            transposer.m_MaximumDistance = maxDistance;
            
            MainCamCullingMask = MainCam.cullingMask;

            
            subCams = new();
            CutsceneCams = new();

            CurrentSubCamId = -1;
            
            
            GameManager.instance.playerRegistered.AddListener(p => _player = p);
            fakePlayerTarget = new GameObject("FakePlayerTarget");
            fakePlayerTarget.transform.SetParent(transform);
            isShake = false;
        }
        
        private void Start()
        {
            GameManager.Scene.WhenSceneLoadBegin.AddListener(_ => CloseDirecting());
            GameManager.Scene.WhenSceneLoaded.AddListener((s) => { ResisterSubCams(); });
        }

        public void MovePlayerCamToPlayer()
        {
            _playerCam.transform.position = _player.transForCamGroup.position;
        }
        public void ToggleMainCamCullingMask(bool isOn)
        {
            MainCam.cullingMask = isOn ? MainCamCullingMask : 0;
        }

        



        #region 서브캠

        private CinemachineVirtualCamera GetSubCam(int subCamId)
        {
            return subCams.GetValueOrDefault(subCamId);
        }

        private void ResisterSubCams()
        {
            subCams.Clear();
            foreach (var subCam in FindObjectsOfType<CinemachineVirtualCamera>())
            {
                if (subCam.gameObject.name.StartsWith("SubCam_"))
                {
                    int subCamId = int.Parse(subCam.gameObject.name.Substring(7));
                    subCams.Add(subCamId, subCam);
                    subCam.enabled = false;
                }
            }
        }
        
        public void DirectingBySubCam(int subCamId)
        {
            if (subCamId == 0)
            {
                CloseDirecting();
                return;
            }
            if (subCams.TryGetValue(subCamId, out var value))
            {
                CurrentSubCamId = subCamId;
                value.enabled = true;
                PlayerCam.enabled = false;
            }
        }

        public void CloseDirecting()
        {
            if (subCams.TryGetValue(CurrentSubCamId, out var value))
            {
                PlayerCam.enabled = true;
                value.enabled = false;
                CurrentSubCamId = -1;
            }
        }

        #endregion

        #region 컷씬캠

        public void AddCutsceneCams(int cutsceneID, CinemachineVirtualCamera[] cams)
        {
            if (CutsceneCams.ContainsKey(cutsceneID))
            {
                CutsceneCams[cutsceneID] = cams;
            }
            else
            {
                CutsceneCams.Add(cutsceneID, cams);
            }
           
            return;
        }

        public CinemachineVirtualCamera[] GetCutsceneCams(int cutsceneID){
            if(!CutsceneCams.TryGetValue(cutsceneID, out var cams)){
                Debug.Log($"No camera registered for cutscene {cutsceneID}");
                return null;
            }
            return cams;
        }

        public void ResetCutseneCams()
        {
            CutsceneCams.Clear();
        }

        #endregion

        #region 컨파이너

        public void SetPlayerCamConfinerBox2D(BoxCollider2D col)
        {
            
            if (col == null)
            {
                playerCamConfiner.BoundingShape2D = null;
                playerCamConfiner.enabled = false;
                UpdateConfinerMaxDistance(false);
                // Debug.Log("Box on");
            }
            else
            {
                Vector2 size = col.size;
                Vector2 center = col.offset;

                // BoxCollider2D의 속성을 기반으로 PolygonCollider2D의 path 수정
                Vector2[] points = new Vector2[4];

                // BoxCollider2D의 모서리 점 계산
                points[0] = new Vector2(-size.x / 2f, -size.y / 2f) + center;
                points[1] = new Vector2(-size.x / 2f, size.y / 2f) + center;
                points[2] = new Vector2(size.x / 2f, size.y / 2f) + center;
                points[3] = new Vector2(size.x / 2f, -size.y / 2f) + center;
                
                _confinerCol.enabled = true;
                // PolygonCollider2D의 path 수정
                _confinerCol.SetPath(0, points);
                _confinerCol.transform.position = col.transform.position;
                
                
                playerCamConfiner.BoundingShape2D = _confinerCol;
                playerCamConfiner.InvalidateCache();
                playerCamConfiner.enabled = true;
                UpdateConfinerMaxDistance();
                // Debug.Log("Box on2");
            }
        }

        // 제한자 설정
        public void SetPlayerCamConfiner2D(PolygonCollider2D col)
        {
            // Debug.Log("Poly on");
            if (object.ReferenceEquals(col, null))
            {
                playerCamConfiner.BoundingShape2D = null;
                playerCamConfiner.enabled = false;
                UpdateConfinerMaxDistance(false);
            }
            else
            {
                playerCamConfiner.BoundingShape2D = col;
                playerCamConfiner.enabled = true;
                UpdateConfinerMaxDistance();
            }
        }

        public void ConfinerForceUpdate()
        {
            float temp = playerCamConfiner.Damping;
            playerCamConfiner.Damping = 0;
            CineBrain.ManualUpdate();
            playerCamConfiner.Damping = temp;
        }
        
        public void UpdateConfinerMaxDistance(bool isOn = true)
        {
            if (isOn && playerCamConfiner.BoundingShape2D != null)
            {
                Bounds bounds = playerCamConfiner.BoundingShape2D.bounds;

                // y에 의한 최대 z 거리 계산
                float confinerMaxDistY = bounds.extents.y / Mathf.Tan(MainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                
                // x축에 의한 최대 Z 거리 계산
                float confinerMaxDistX = bounds.extents.x / Mathf.Tan(MainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                
                float confinerMaxDist = Mathf.Min(confinerMaxDistY, confinerMaxDistX);

                // 타겟 그룹 계산
                TargetGroup.DoUpdate();
                Bounds tbounds = TargetGroup.BoundingBox;
                float zExtent = tbounds.extents.z; // z축 두께의 절반
                Debug.Log($"check confiner {tbounds} {zExtent}");
                confinerMaxDist -= zExtent;
            
                // 값 하고 계산.
                transposer.m_MaximumDistance = Mathf.Min(confinerMaxDist, maxDistance);
            }
            else
            {
                transposer.m_MaximumDistance = maxDistance;
            }
        }

        #endregion

        public void InitPlayerCamPosition()
        {
            _playerCam.transform.position = TargetGroupCamera.instance.transform.position;
        }
        
        

        public void ResetPlayerCamToggle(bool isReset)
        {
            if (isReset)
            {
                PlayerCam.Follow = null;
                PlayerCam.transform.position = Vector3.back * camDistance;
                MainCam.transform.position = Vector3.back * camDistance;
                PlayerCam.enabled = false;
            }
            else if (GameManager.instance.Player != null)
            {
                PlayerCam.Follow = TargetGroup.transform;
                PlayerCam.enabled = true;
            }
        }

        public void ToggleCameraFix(bool isFix)
        {
            if (isFix)
            {
                PlayerCam.Follow = null;
            }
            else
            {
                PlayerCam.Follow = TargetGroup.transform;
                PlayerCam.transform.position = TargetGroup.transform.position;
            }
        }

        #region 화면흔들림

        private bool isShake;
        
        public IEnumerator ShakePlayerCam(float amplitude, float frequency , float duration)
        {
            if (isShake) yield break;

            isShake = true;
            var pli = PlayerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            pli.AmplitudeGain = amplitude;
            pli.FrequencyGain = frequency;
            yield return new WaitForSeconds(duration);
            pli.AmplitudeGain = 0;
            pli.FrequencyGain = 0;
            isShake = false;
        }

        #endregion


        #region 플레이어 카메라 조작

        [HideInInspector]public float playerCamX;
        [HideInInspector]public float playerCamY;

        [LabelText("카메라 조작거리")] public float lookAheadDistance;

        private Vector3 _playerCamOffset;
        private void LateUpdate()
        {
            if (!ReferenceEquals(null, GameManager.instance.Player) )
            {
                _playerCamOffset = new Vector3(playerCamX, playerCamY, 0).normalized * lookAheadDistance;
               
                if (TargetGroup.m_Targets.Length <= 1)
                {
                    fakePlayerTarget.transform.position =
                        GameManager.instance.Player.transForCamGroup.position + _playerCamOffset;
                }
                else
                {
                    fakePlayerTarget.transform.position = GameManager.instance.Player.transForCamGroup.position;
                }
            }
        }

        #endregion
    }
}