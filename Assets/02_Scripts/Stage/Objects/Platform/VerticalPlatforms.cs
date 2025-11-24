using Apis;
using Default;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class VerticalPlatforms : PlatformCreator
{
    enum Direction
    {
        Up,Down
    }
    
    [LabelText("이동방향")] [SerializeField] Direction direction = Direction.Up;
    [LabelText("이동시간")] [SerializeField] private float moveTime;
    [LabelText("이동Ease")] [SerializeField] private Ease moveEase;
    [InfoBox("입력하면 씬창에서 해당위치에 빨간색(시작점),파란색(끝점)이 표시됩니다.")]
    [LabelText("시작점")][SerializeField] private float startPoint;
    [LabelText("끝점")] [SerializeField] private float endPoint;
    [LabelText("생성시간")][SerializeField] float creationTime;
    public LayerMask layers;

    private float curTime;
    
    public virtual void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0,startPoint), 0.2f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + new Vector3(0,endPoint), 0.2f);
    }
    public override GameObject CreatePlatform(Vector2 position)
    {
        GameObject platform = base.CreatePlatform(position);
        platform.GetOrAddComponent<MovingObj>().layers = layers;
        
        return platform;
    }
    
    
    public override void Return(GameObject platform)
    {
        void Func()
        {
            if (platform.TryGetComponent(out MovingObj movingObj))
            {
                Destroy(movingObj);
            }

            base.Return(platform);
        }
        
        if (platform.TryGetComponent(out SpriteRenderer render))
        {
            Color color = render.color;
            Tween tween = render.DOColor(new Color(render.color.r, render.color.g, render.color.b, 0), 0.5f);
            tween.onComplete += Func;
            tween.onComplete += () =>
            {
                render.color = color;
            };
        }
        else
        {
            Func();
        }
    }

    void Create()
    {
        GameObject platform = CreatePlatform(new Vector2(transform.position.x, transform.position.y + (direction == Direction.Down ? startPoint : endPoint)));
        curTime = creationTime;
        var tween = platform.transform.DOMoveY(transform.position.y + (direction == Direction.Down ? endPoint : startPoint), moveTime).SetEase(moveEase);
        tween.onComplete += () =>
        {
            Return(platform);
        };
    }
    private void Awake()
    {
        Create();
    }

    private void Update()
    {
        curTime -= Time.deltaTime;

        if (curTime <= 0)
        {
            Create();
            curTime = creationTime;
        }
    }
}
