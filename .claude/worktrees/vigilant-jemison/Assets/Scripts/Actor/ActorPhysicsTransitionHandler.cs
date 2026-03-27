using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.Components;
using UnityEngine;

public class ActorPhysicsTransitionHandler : MonoBehaviour
{
    private HashSet<Trigger> activatingList = new();
    private Collider2D currentPhysicsCollider;

    private HashSet<Trigger> ignoredList = new();


    private bool isTransitioning;

    private Coroutine transitionCoroutine;

    public HashSet<Trigger> ActivatingList
    {
        get { return activatingList ??= new HashSet<Trigger>(); }
    }

    public HashSet<Trigger> IgnoredList => ignoredList ??= new HashSet<Trigger>();

    private void Awake()
    {
        currentPhysicsCollider = GetComponent<Actor>().Collider;
        isTransitioning = false;
    }

    private void FixedUpdate()
    {
        if (!isTransitioning && ignoredList.Count > 0 && currentPhysicsCollider != null)
        {
            var copy = new HashSet<Trigger>(ignoredList);
            var exitedIgnoredColliders = new List<Trigger>();

            var contactFilter = new ContactFilter2D();
            contactFilter.useLayerMask = true;
            contactFilter.layerMask = LayerMasks.Trigger;
            contactFilter.useTriggers = true;
            List<Collider2D> targets = new();
            Physics2D.OverlapCollider(currentPhysicsCollider, contactFilter, targets);

            foreach (var trigger in copy)
            {
                if (trigger == null || trigger._col.enabled == false || !trigger.gameObject.activeSelf)
                {
                    exitedIgnoredColliders.Add(trigger);
                    continue;
                }

                if (!targets.Contains(trigger._col)) exitedIgnoredColliders.Add(trigger);
            }

            foreach (var trigger in exitedIgnoredColliders)
                if (trigger != null)
                {
                    Physics2D.IgnoreCollision(currentPhysicsCollider, trigger._col, false);
                    ignoredList.Remove(trigger);
                    trigger.OnTriggerExit2D(currentPhysicsCollider);
                }
                else
                {
                    ActivatingList.Remove(trigger);
                    ignoredList.Remove(trigger);
                }
        }
    }

    public Collider2D GetCurrentCollider()
    {
        return currentPhysicsCollider;
    }

    private void RefreshActivatingList()
    {
        activatingList = ActivatingList.Where(x => x != null && x.gameObject.activeSelf && x._col.enabled).ToHashSet();
    }

    public void StartColliderTransition(Collider2D colliderToDisable, Collider2D colliderToEnable)
    {
        if (isTransitioning)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 전환 중입니다!", gameObject);

            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        }

        if (colliderToDisable == colliderToEnable || colliderToDisable == null || colliderToEnable == null)
        {
            Debug.LogError($"[{gameObject.name}] 콜라이더 전환 시작 오류: 콜라이더 정보가 올바르지 않습니다.", gameObject);
            return;
        }

        if (colliderToDisable != currentPhysicsCollider)
            Debug.LogWarning(
                $"[{gameObject.name}] 콜라이더 전환 시작 경고: 비활성화하려는 콜라이더({colliderToDisable.name})가 현재 활성 콜라이더({currentPhysicsCollider?.name})와 다릅니다.",
                gameObject);


        isTransitioning = true;

        RefreshActivatingList();

        var initialActiveTriggersSnapshot = new List<Trigger>(ActivatingList);

        transitionCoroutine =
            StartCoroutine(TransitionColliderCoroutine(colliderToDisable, colliderToEnable,
                initialActiveTriggersSnapshot));
    }


    // --- 내부 전환 코루틴 ---
    private IEnumerator TransitionColliderCoroutine(
        Collider2D colliderBeingDisabled, // 비활성화할 콜라이더
        Collider2D colliderBeingEnabled, // 활성화할 콜라이더
        List<Trigger> initialActiveTriggersSnapshot // 전환 시작 시 게임 로직 상 활성 트리거 목록 스냅샷
    )
    {
        // --- Phase 1: 이전 콜라이더 정리 및 IgnoreCollision 해제 ---
        if (colliderBeingDisabled != null)
        {
            foreach (var ignoredTriggerCol in IgnoredList.ToList())
                if (ignoredTriggerCol != null && ignoredTriggerCol.enabled)
                    Physics2D.IgnoreCollision(colliderBeingDisabled, ignoredTriggerCol._col, false);

            IgnoredList.Clear();

            // 현재 전환(탑승/하차)을 위해 colliderBeingDisabled와 스냅샷 오버랩 트리거들 간 상호작용 무시 설정
            foreach (var trigger in initialActiveTriggersSnapshot)
                if (trigger != null && trigger._col != null && trigger._col.enabled)
                {
                    Physics2D.IgnoreCollision(colliderBeingDisabled, trigger._col, true);
                    IgnoredList.Add(trigger);
                }

            // 이전 콜라이더 비활성화 (IgnoreCollision 설정 덕분에 Exit 이벤트 방지 기대)
            if (colliderBeingDisabled.enabled) colliderBeingDisabled.enabled = false;
            // Debug.Log($"{colliderBeingDisabled.name} 비활성화 완료 (코루틴)");
        }

        yield return new WaitForFixedUpdate();

        IgnoredList.Clear();

        // --- Phase 2: 새 콜라이더 활성화 및 일시적 무시 설정 ---
        if (colliderBeingEnabled != null)
        {
            // TODO: 새 콜라이더의 위치/회전/부모 설정 등 물리적 상태 설정 (이것은 외부에서 관리)
            // Debug.Log($"{colliderBeingEnabled.name} 활성화 (코루틴)");

            // 새 콜라이더와 스냅샷의 오버랩 트리거들 간 상호작용 무시 (OnTriggerEnter 방지 기대)
            foreach (var trigger in initialActiveTriggersSnapshot)
                if (trigger != null && trigger._col != null && trigger._col.enabled)
                {
                    Physics2D.IgnoreCollision(colliderBeingEnabled, trigger._col, true);
                    IgnoredList.Add(trigger);
                }

            colliderBeingEnabled.enabled = true;
            // 새 콜라이더가 이제부터 무시할 트리거 목록으로 업데이트
        }

        yield return new WaitForFixedUpdate();

        currentPhysicsCollider = colliderBeingEnabled; // 전환 완료. 현재 물리 주체 콜라이더 업데이트
        isTransitioning = false;
        transitionCoroutine = null;
    }

    public void RemoveAll()
    {
        if (IgnoredList.Count > 0)
            foreach (var trigger in IgnoredList)
                Physics2D.IgnoreCollision(currentPhysicsCollider, trigger._col, false);

        IgnoredList.Clear();
        ActivatingList.Clear();
    }
}