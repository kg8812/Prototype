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
using UnityEngine.Serialization;

public partial class GameManager
{
    public float originTimeScale = 1f;

    [FormerlySerializedAs("WhenConvenienceUnlock")] [HideInInspector]
    public UnityEvent WhenUnlock = new();

    private UnityEvent _whenReturnedToTitle;
    private bool isCountPlayTime;

    private bool isInit;

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
    }

    private void Update()
    {
        SUpdate();

        // TODO: 나중에 빌드때 없애기
        if (InputManager.GetKeyDown(KeyCode.LeftBracket)) UpdateTime(Mathf.Max(0.01f, originTimeScale - 0.1f));
        if (InputManager.GetKeyDown(KeyCode.RightBracket)) UpdateTime(Mathf.Min(3f, originTimeScale + 0.1f));

        if (Time.timeScale > 0 && InputManager.GetKeyDown(KeyCode.F10)) UI.CreateUI("NewCheatUI", UIType.Scene);

        if (isCountPlayTime) playTime += Time.deltaTime;
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

    public void GameOver()
    {
        Sound.StopArenaBGM(0.5f);
        Sound.StopSceneBGM();
        FadeManager.instance.Fading(() => { instance.Player.ResetPlayerStatus(); });
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
    
    public void SaveSlot()
    {
        if (Scene.CurSceneData.isPlayerMustExist && Player != null)
        {
            Slot.SaveCurrentSlot();
        }
    }
    

    #region 일시정지 관리

    private readonly HashSet<Guid> _pauseGuids = new();

    public Guid RegisterPause()
    {
        var guid = Guid.NewGuid();
        _pauseGuids.Add(guid);
        Pause();
        return guid;
    }

    public bool RemovePause(Guid guid)
    {
        if (_pauseGuids.Remove(guid))
            if (_pauseGuids.Count == 0)
            {
                Resume();
                return true;
            }

        return false;
    }

    [Button]
    private void Pause()
    {
        Time.timeScale = 0;
        Physics2D.simulationMode = SimulationMode2D.Update;
    }

    [Button]
    private void Resume()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Time.timeScale = originTimeScale;
    }

    public void UpdateTime(float time)
    {
        originTimeScale = time;
        if (_pauseGuids.Count == 0)
            Time.timeScale = originTimeScale;
    }

    #endregion
}