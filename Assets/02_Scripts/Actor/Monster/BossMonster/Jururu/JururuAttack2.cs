using Apis;
using DG.Tweening;
using System.Collections;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public class JururuAttack2 : BossAttackCollider
{
    Player target;

    private SpriteRenderer _renderer;

    public GameObject col;

    protected override void AttackInvoke(EventParameters parameters)
    {
        base.AttackInvoke(parameters);

        if (isPull) return;
        target = parameters.target as Player;
        if (target != null)
        {
            hitPos.position = target.transform.position;
            AddStun();
            isAtked = true;
            dist = hitPos.localPosition + new Vector3(_renderer.size.x, 0);
        }

        tweener?.Kill();
        StartCoroutine(PullAnimation());
        col.SetActive(false);
        boss.animator.SetBool("IsAttackEnd",false);
    }

    private void Start()
    {
        Init(boss, new TargetCurHpRatio(hpRatio));
    }
    

    [Header("채찍 사출")]
    [LabelText("이동시간")] public float moveTime1;
    [LabelText("이동거리")] public float distance;
    [LabelText("사출 속도 그래프")] public Ease ease1;
    [Header("데미지")]
    [LabelText("현재 체력 %")] public float hpRatio = 10;
    [LabelText("채찍 대기 시간")] public float duration2;
    [Header("채찍 회수")]
    [LabelText("이동시간")]public float moveTime2;
    [LabelText("채찍 소환거리 (보스 기준)")] public float stopDistance;
    [LabelText("회수 속도 그래프")] public Ease ease2;
    Tweener tweener;


    private bool isPull;

    public Transform hitPos;
    protected override void Awake()
    {
        base.Awake();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private Vector2 dist;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        Init(boss, new TargetCurHpRatio(hpRatio));
        StopAllCoroutines();
        CancelInvoke();
        _renderer.size = new Vector2(0, _renderer.size.y);
        transform.position = boss.Position + Vector3.right * (int)boss.Direction * stopDistance;
        col.SetActive(true);
        tweener = DOTween.To(() => _renderer.size, x =>
            {
                _renderer.size = x;
                col.transform.localPosition = new Vector3(-x.x, col.transform.localPosition.y);
            }, new Vector2(distance, _renderer.size.y),
            moveTime1).SetEase(ease1).SetAutoKill(true);
        tweener.onKill += () =>
        {
            CancelInvoke();
            Invoke(nameof(InvokePull), duration2);
            col.SetActive(false);
        };
        target = null;
        isPull = false;
        isAtked = false;
    }   
    
    void InvokePull()
    {
        if(!isPull && gameObject.activeSelf)
        {
            StartCoroutine(PullAnimation());
        }
    }
    IEnumerator PullAnimation()
    {
        if (isPull) yield break;
        
        isPull = true;

        yield return new WaitForEndOfFrame();
        boss.animator.SetTrigger("AttackEnd");
    }
    protected override void OnDisable()
    {
        tweener?.Kill();
        CancelInvoke();
        StopCoroutine(nameof(PullAnimation));
        RemoveStun();
        isAtked = false;
        col.SetActive(false);
    }

    public void Pull()
    {
        tweener?.Kill();
        tweener = DOTween.To(() => _renderer.size, x => _renderer.size = x, new Vector2(0, _renderer.size.y),
            moveTime2).SetUpdate(UpdateType.Fixed).SetEase(ease2).SetAutoKill(true);

        tweener.onUpdate += () =>
        {
            hitPos.localPosition = new Vector2(-_renderer.size.x + dist.x, dist.y);
        };
        tweener.onKill += () =>
        {
            CancelInvoke();
            Invoke(nameof(InvokePull), duration2);
        };
        
        col.SetActive(false);
    }

    private bool isAtked;
    void Update()
    {
        
        if (isAtked)
        {
            target.transform.position = hitPos.position;
            target.Rb.velocity = Vector2.zero;
        }
    }

    void AddStun()
    {
        target?.ControlOff();
    }
    void RemoveStun()
    {
        target?.ControlOn();
    }
}
