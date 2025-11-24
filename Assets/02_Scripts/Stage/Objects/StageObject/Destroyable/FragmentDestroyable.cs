using System.Collections.Generic;
using Apis;
using DG.Tweening;
using UnityEngine;

public class FragmentDestroyable : DestroyableObject
{
    public int dropIndex;
    public float gravityScale;
    public float force = 3;
    public float delay = 2f;   // n초 후 페이드 시작
    public float fadeTime = 1f; // 페이드 아웃 지속 시간
    

    [SerializeField] private ItemDropper _dropper;
    public List<Rigidbody2D> fragments;
    public List<Vector2> localPositions;

    protected override void Awake()
    {
        if (fragments.Count == 0)
        {
            fragments = new();
            for (int i = 0; i < (transform.childCount - (hasBlock?1:0)); i++)
            {
                fragments.Add(transform.GetChild(i+(hasBlock?1:0)).gameObject.GetComponent<Rigidbody2D>());
                fragments[i].bodyType = RigidbodyType2D.Static;
            }
        }

        localPositions = new();
        for (int i = 0; i < fragments.Count; i++)
        {
            localPositions.Add(fragments[i].transform.localPosition);
        }
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
        _dropper.DropperId = dropIndex;
        for (int i = 0; i < localPositions.Count; i++)
        {
            fragments[i].bodyType = RigidbodyType2D.Static;
            fragments[i].transform.localPosition = localPositions[i];
        }
    }

    protected override void DestroyObj(EventParameters parameters)
    {
        base.DestroyObj(parameters);
        _dropper.Drop();
        Explosion(parameters);
    }

    private void Explosion(EventParameters parameters)
    {
        if (fragments.Count != 0)
        {
            sr.enabled = false;
            for (int i = 0; i < fragments.Count; i++)
            {
                fragments[i].bodyType = RigidbodyType2D.Dynamic;
                fragments[i].velocity = Vector2.zero;
                fragments[i].gravityScale = gravityScale;
                Vector2 dir = transform.position - parameters.user.transform.position;
                fragments[i].AddForceAtPosition(dir.normalized * force, parameters.user.transform.position, ForceMode2D.Impulse);
                Disappear(fragments[i].transform);
            }
        }
    }

    private void Disappear(Transform trans)
    {
        SpriteRenderer dsr = trans.GetComponent<SpriteRenderer>();
        if (dsr != null)
        {
            dsr.DOFade(0f, fadeTime)
                .SetDelay(delay)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
