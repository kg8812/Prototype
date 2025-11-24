using System.Collections;
using System.Collections.Generic;
using Default;
using UnityEngine;

public class UI_TestScene : UI_Scene
{
    public void MoveToScene(string sceneName)
    {
        GameManager.Scene.SceneLoad(sceneName);
    }
}
