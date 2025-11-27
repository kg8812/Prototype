using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using DG.Tweening;
using UnityEngine;

public class SpringJump : MonoBehaviour
{
    public float jumpPower;
    public float cd;
    public float jumpTime;
    public LayerMask layers;
    [SerializeField] private BoxCollider2D collision;

    private float curCd;
    private bool isJumping;
    private MovingObj movingObj;

    private SpriteRenderer render;
    private List<Rigidbody2D> targets = new();

    private void Awake()
    {
        curCd = 0;
        targets ??= new List<Rigidbody2D>();
        isJumping = false;
        render = GetComponent<SpriteRenderer>();
        movingObj = collision.GetComponent<MovingObj>();
    }

    private void Update()
    {
        if (!isJumping) curCd -= Time.deltaTime;
        if (curCd <= 0 && targets.Count > 0) StartCoroutine(DoJump());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            var target = other.transform.GetComponentInParentAndChild<Rigidbody2D>();

            if (!targets.Contains(target)) targets.Add(target);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            var target = other.transform.GetComponentInParentAndChild<Rigidbody2D>();

            if (targets.Contains(target)) targets.Remove(target);
        }
    }

    private IEnumerator DoJump()
    {
        if (isJumping) yield break;
        curCd = cd;
        isJumping = true;
        render.color = Color.yellow;
        var ySize = collision.size.y;

        DOTween.To(() => collision.size.y, y => collision.size = new Vector2(collision.size.x, y), ySize / 2f,
                jumpTime / 2f)
            .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        DOTween.To(() => collision.offset.y, y => collision.offset = new Vector2(0, y), -ySize / 4f, jumpTime / 2f)
            .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        collision.transform.DOLocalMoveY(0, jumpTime / 2f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        yield return new WaitForSeconds(jumpTime / 2f);
        DOTween.To(() => collision.size.y, y => collision.size = new Vector2(collision.size.x, y), ySize, jumpTime / 2f)
            .SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
        DOTween.To(() => collision.offset.y, y => collision.offset = new Vector2(0, y), -ySize / 2f, jumpTime / 2f)
            .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        collision.transform.DOLocalMoveY(ySize / 2f, jumpTime / 2f).SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
        targets.ForEach(x => { x.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); });
        render.color = Color.white;
        targets.Clear();

        yield return new WaitForSeconds(jumpTime / 2f);

        isJumping = false;
    }
}