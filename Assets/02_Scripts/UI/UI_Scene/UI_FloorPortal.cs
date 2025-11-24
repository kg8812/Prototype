using Default;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_FloorPortal : UI_Scene
{
    GameObject black;

    enum Buttons
    {
        Button1,Button2,Button3
    }

    readonly Button[] buttons = new Button[3];
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        buttons[0] = Get<Button>((int)Buttons.Button1);
        buttons[1] = Get<Button>((int)Buttons.Button2);
        buttons[2] = Get<Button>((int)Buttons.Button3);
        buttons[0].interactable = false;

    }
    
    public void Move(int floor)
    {
        FadeManager.instance.Fading(() => { MoveFloor(floor); });
        for(int i = 0;i < buttons.Length;i++)
        {
            buttons[i].interactable = true;
        }
        buttons[floor].interactable = false;
    }

    private void MoveFloor(int floor)
    {
        GameManager.instance.ControllingEntity.transform.position = LobbyManager.instance.spawnPos[floor].position;
        Vector3 pos = GameManager.instance.ControllingEntity.transform.position;
        pos.z = 0;
        GameManager.instance.ControllingEntity.transform.position = pos;
    }

    public void CloseUI()
    {
        GameManager.UI.CloseUI(this);
    }
    
    /*
    void ShowBlackScreen()
    {
        if (black == null)
        {
            black = ResourceUtil.Instantiate("Black Screen", Camera.main.transform);
        }

        black.SetActive(true);

        Invoke(nameof(DisableBlackScreen), 1f);
    }

    void DisableBlackScreen()
    {
        black.SetActive(false);
    }
    */
}