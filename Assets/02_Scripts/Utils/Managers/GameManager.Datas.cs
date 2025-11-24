using Apis;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager
{
    public float playTime;
    int soul = 0;
    int lobbySoul = 0;
    
    public int Soul
    {
        get => soul;
        set
        {
            if (soul != value)
            {
                int x = value - soul;
                soul = value;
                OnSoulChange.Invoke(x);
            }
        }
    }
    public int LobbySoul
    {
        get => lobbySoul;
        set
        {
            if (lobbySoul != value)
            {
                int x = value - lobbySoul;
                lobbySoul = value;
                OnLobbySoulChange.Invoke(x);
            }
        }
    }

    
    
    public UnityEvent<Monster> OnEnemyKill = new();
    public UnityEvent<int> OnSoulChange = new();
    public UnityEvent<int> OnLobbySoulChange = new();
    public UnityEvent OnGameReset = new();

    public void ResetGame()
    {
        soul = 0;
        OnGameReset.Invoke();
    } 
}
