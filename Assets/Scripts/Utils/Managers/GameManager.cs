using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Managers;
using Sirenix.OdinInspector;
using Apis.UI;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager
{
    public float originTimeScale = 1f;

    private UnityEvent _whenReturnedToTitle;
    private bool isInit;

    /// <summary>Init/Loading을 제외한 마지막 씬 타입. 타이틀 "복귀"인지 첫 진입인지 가른다.</summary>
    private SceneType _lastSceneType = SceneType.Init;

    public UnityEvent WhenReturnedToTitle => _whenReturnedToTitle ??= new UnityEvent();
    public static bool IsQuitting { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        IsQuitting = false;
        playerInit = new UnityEvent<Player>();
        playerRegistered = new UnityEvent<Player>();
        isInit = false;
        // player = FindObjectOfType<Player>();
        Data.Load();
        DontDestroyOnLoad(this);

        DefaultController = new DefaultController();

        SAwake();

        // 게임 데이터 저장하려면 프로그레싱이 선행되어야 하기 때문에 ProgressManager자체에서 이벤트 연결
        // Scene.WhenSceneLoaded.AddListener(_ => SaveSlot());

        Scene.WhenSceneLoaded.AddListener(PlayerToggleWhenSceneChanged);
        Scene.WhenSceneLoaded.AddListener(DetectReturnToTitle);

        // 씬이 바뀌면 일시정지 guid 보유자(UI 등)가 통째로 사라지므로 남은 일시정지를 먼저 푼다.
        // WhenSceneLoaded가 아니라 Begin에 거는 이유: 새 씬의 UI가 등록한 일시정지까지 지우면 안 되기 때문.
        Scene.WhenSceneLoadBegin.AddListener(_ => ClearAllPauses());

        // 게임 특화 초기화. GameManager.Sample.cs가 없으면 이 호출은 컴파일 단계에서 사라진다.
        OnSampleAwake();
    }

    /// <summary>게임 특화 초기화 훅. 구현은 GameManager.Sample.cs에 있다.</summary>
    partial void OnSampleAwake();

    /// <summary>게임 특화 매 프레임 훅. 구현은 GameManager.Sample.cs에 있다.</summary>
    partial void OnSampleUpdate();

    private void Update()
    {
        SUpdate();
        OnSampleUpdate();
    }


    private void OnDisable()
    {
        // Scene.RemoveSceneLoaded();
    }

    private void OnApplicationQuit()
    {
        IsQuitting = true;
    }

    public Coroutine StartCoroutineWrapper(IEnumerator aEnumerator)
    {
        return StartCoroutine(aEnumerator);
    }

    public void StopCoroutineWrapper(Coroutine coroutine)
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    public static void DontDestroyObject(GameObject obj)
    {
        DontDestroyOnLoad(obj);
    }

    /**
     * 예외처리
     */
    private void PlayerToggleWhenSceneChanged(SceneData sceneData)
    {
        if (!sceneData.isPlayerMustExist)
        {
            UI.ToggleMainUI(false);
            if (Player != null) Player.gameObject.SetActive(false);
        }
        else
        {
            UI.ToggleMainUI(true);
        }
    }
    
    /// <summary>
    ///     게임 씬에서 타이틀로 돌아왔을 때 <see cref="WhenReturnedToTitle" />를 발화한다.
    ///     Init/Loading은 경유 씬이라 "어디서 왔는지"를 판단할 때 무시한다.
    /// </summary>
    private void DetectReturnToTitle(SceneData sceneData)
    {
        if (sceneData.sceneType is SceneType.Init or SceneType.Loading) return;

        var returned = sceneData.sceneType == SceneType.Title && _lastSceneType == SceneType.Other;
        _lastSceneType = sceneData.sceneType;

        if (returned) WhenReturnedToTitle.Invoke();
    }

    public void SaveSlot()
    {
        if (Scene.CurSceneData.isPlayerMustExist && Player != null)
        {
            Slot.SaveCurrentSlot();
        }
    }
    

    #region 일시정지 관리

    [Tooltip("일시정지 중 2D 물리 시뮬레이션 모드를 Update로 전환한다. 2D 프로젝트에서만 의미가 있다.")]
    public bool switchPhysics2DModeOnPause;

    private readonly HashSet<Guid> _pauseGuids = new();

    public bool IsPaused => _pauseGuids.Count > 0;

    public Guid RegisterPause()
    {
        var guid = Guid.NewGuid();
        _pauseGuids.Add(guid);
        if (_pauseGuids.Count == 1) Pause();
        return guid;
    }

    /// <returns>마지막 일시정지가 풀려 실제로 Resume된 경우에만 true</returns>
    public bool RemovePause(Guid guid)
    {
        if (!_pauseGuids.Remove(guid)) return false;
        if (_pauseGuids.Count > 0) return false;

        Resume();
        return true;
    }

    /// <summary>
    ///     남아있는 일시정지를 전부 푼다.
    ///     guid 보유자가 해제하지 못한 채 사라지면(씬 전환 등) guid가 영구히 남아 게임이 영원히 멈추므로 필요하다.
    /// </summary>
    public void ClearAllPauses()
    {
        if (_pauseGuids.Count == 0) return;

        _pauseGuids.Clear();
        Resume();
    }

    [Button]
    private void Pause()
    {
        Time.timeScale = 0;
        if (switchPhysics2DModeOnPause) Physics2D.simulationMode = SimulationMode2D.Update;
    }

    [Button]
    private void Resume()
    {
        if (switchPhysics2DModeOnPause) Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Time.timeScale = originTimeScale;
    }

    #endregion
}