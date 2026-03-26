using GameStateSpace;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager
{
    public float playTime;
    private bool _isCountPlayTime;

    public UnityEvent OnGameReset = new();

    public void ResetGame()
    {
        OnGameReset.Invoke();
    }

    private void UpdatePlayTime()
    {
        if (_isCountPlayTime) playTime += Time.deltaTime;
    }

    private void ToggleCountPlayTime(GameStateType sType)
    {
        switch (sType)
        {
            case GameStateType.BattleState:
            case GameStateType.NonBattleState:
                _isCountPlayTime = true;
                break;
            case GameStateType.DefaultState:
            case GameStateType.InteractionState:
                _isCountPlayTime = false;
                break;
        }
    }
}