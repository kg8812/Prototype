using Apis;
using Apis.Components;
using Apis.Managers;
using Default;
using Managers;
using UnityEngine;

public partial class GameManager : Singleton<GameManager>
{
    private static DatabaseManager _data;

    private static SceneLoadManager _scene;

    private static UIManager _ui;

    private static SoundManager _sound;


    private static TriggerManager _trigger;


    private static SaveManager _save;

    private static FactoryManager _factory;

    private static ItemFactoryManager _item;

    private static AttackObject _atkObj;

    public static DatabaseManager Data
    {
        get
        {
            if (_data == null)
            {
                _data = new DatabaseManager();
                _data.Init();
            }

            return _data;
        }
    }

    public static SceneLoadManager Scene
    {
        get
        {
            if (_scene == null)
            {
                _scene = new SceneLoadManager();
                _scene.Init();
            }

            return _scene;
        }
    }

    public static UIManager UI
    {
        get
        {
            if (_ui == null)
            {
                _ui = new UIManager();
                _ui.Init();
            }

            return _ui;
        }
    }

    public static SoundManager Sound
    {
        get
        {
            if (_sound == null)
            {
                _sound = new SoundManager();
                _sound.Init();
            }

            return _sound;
        }
    }

    public static TriggerManager Trigger
    {
        get
        {
            if (_trigger == null)
            {
                _trigger = new TriggerManager();
                _trigger.Init();
            }

            return _trigger;
        }
    }

    public static SaveManager Save
    {
        get
        {
            _save ??= new SaveManager();
            return _save;
        }
    }

    public static FactoryManager Factory
    {
        get
        {
            _factory ??= new FactoryManager();

            return _factory;
        }
    }

    public static ItemFactoryManager Item => _item ??= new ItemFactoryManager();

    public static AttackObject AtkObj
    {
        get
        {
            if (_atkObj == null || !_atkObj.gameObject.activeSelf)
            {
                _atkObj = Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject, "SquareAttackObject");
                _atkObj.transform.SetParent(instance.transform);
                _atkObj.Collider.enabled = false;
                _atkObj.transform.localPosition = Vector3.zero;
            }

            return _atkObj;
        }
    }
}