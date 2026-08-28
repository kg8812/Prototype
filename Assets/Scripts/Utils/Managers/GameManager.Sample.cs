using Apis.UI;
using GameStateSpace;
using Managers;
using UnityEngine;

/// <summary>
///     게임(장르) 특화 기능 모음. 베이스 템플릿에는 포함되지 않는 것들이다.
///     새 프로젝트에서 이 파일 하나만 지우면 관련 기능이 통째로 사라진다.
///     코어는 이 파일을 <c>partial void</c> 훅으로만 호출하므로, 파일이 없어도 컴파일이 깨지지 않는다.
/// </summary>
public partial class GameManager
{
    #region 진행도 (레벨 / 경험치)

    private static PlayerProgressManager _progress;
    public static PlayerProgressManager Progress => _progress ??= new PlayerProgressManager();

    #endregion

    #region 플레이 타임 집계

    public float playTime;
    private bool _isCountPlayTime;

    private void UpdatePlayTime()
    {
        if (_isCountPlayTime) playTime += Time.deltaTime;
    }

    /// <summary>전투/일반 플레이 중에만 플레이 타임을 센다. 메뉴나 대화 중에는 세지 않는다는 게임 규칙.</summary>
    private void ToggleCountPlayTime(GameState state)
    {
        _isCountPlayTime = state is BattleState or PlayState;
    }

    #endregion

    #region 게임 오버

    public void GameOver()
    {
        Sound.StopArenaBGM(0.5f);
        Sound.StopSceneBGM();
        FadeManager.instance.Fading(() => { instance.Player.ResetPlayerStatus(); });
    }

    #endregion

    #region 코어 훅 구현

    partial void OnSampleAwake()
    {
        // 이 게임에만 있는 상태를 등록한다. 템플릿 기본 상태는 SAwake가 이미 등록해 뒀다.
        RegisterState(new BattleState());

        // 플레이 타임 집계를 게임 상태에 연결한다.
        ToggleCountPlayTime(CurState);
        GameStateChangedTo.AddListener(ToggleCountPlayTime);

        // 처치 시 경험치 적립. 코어(GameManager.Player.cs)는 이 규칙을 몰라야 하므로 여기서 붙인다.
        playerRegistered.AddListener(p =>
            p.AddEvent(EventType.OnKill, info =>
            {
                if (info?.target is null or { IsDead: true }) return;

                Progress.Exp += info.target.Exp;
            }));
    }

    partial void OnSampleUpdate()
    {
        UpdatePlayTime();
    }

    #endregion
}
