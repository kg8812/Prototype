using Apis;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager
{
    public float playTime;
    
    public UnityEvent OnGameReset = new();

    public void ResetGame()
    {
        OnGameReset.Invoke();
    } 
}
