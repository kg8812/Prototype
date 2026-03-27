using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.UI.Focus;
using Apis.Util;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Default
{
    public static class Utils
    {
        static EventType[] _eventTypes;

        public static IEnumerable<EventType> EventTypes
        {
            get
            {
                _eventTypes ??= (EventType[])Enum.GetValues(typeof(EventType));

                return _eventTypes;
            }
        }

        static SubBuffType[] _subBuffTypes;

        public static IEnumerable<SubBuffType> SubBuffTypes
        {
            get
            {
                _subBuffTypes ??= (SubBuffType[])Enum.GetValues(typeof(SubBuffType));

                return _subBuffTypes;
            }
        }

        static ActorStatType[] _statTypes;

        public static ActorStatType[] StatTypes
        {
            get
            {
                _statTypes ??= (ActorStatType[])Enum.GetValues(typeof(ActorStatType));

                return _statTypes;
            }
        }

        public static string GetStatText(ActorStatType type)
        {
            return type switch
            {
                ActorStatType.Atk => "공격력",
                ActorStatType.MoveSpeed => "이동속도",
                ActorStatType.AtkSpeed => "공격속도",
                ActorStatType.Def => "방어력",
                ActorStatType.MaxHp => "체력",
                ActorStatType.CritProb => "치명타 확률",
                ActorStatType.CritDmg => "치명타 데미지",
                _ => ""
            };
        }

        public static bool CheckLayer(LayerMask layers, int layer)
        {
            return ((1 << layer) & layers) != 0;
        }

        public static int GetFirstLayerIndex(LayerMask mask)
        {
            int layerValue = mask.value;
            for (int i = 0; i < 32; i++) // Unity는 32개의 Layer를 지원
            {
                if ((layerValue & (1 << i)) != 0)
                {
                    return i; // 첫 번째로 발견된 Layer의 인덱스 반환
                }
            }

            return -1; // Layer가 없으면 -1 반환
        }

        public static IEnumerator Disappear(GameObject obj, float duration)
        {
            var render = GetComponentInParentAndChild<MeshRenderer>(obj);
            if (render != null)
            {
                render.enabled = false;
                yield return new WaitForSeconds(duration);
                render.enabled = true;
            }
        }

        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>() ?? go.AddComponent<T>();
            return component;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);
            return transform == null ? null : transform.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false)
            where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        if (transform.TryGetComponent<T>(out var component))
                            return component;
                    }
                }
            }
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>(true))
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }


        public static List<T> GetChanceList<T>(T[] tData) where T : HasChance
        {
            List<T> tList = new List<T>();
            foreach (var t in tData)
            {
                for (int j = 0; j < t.chance; j++)
                {
                    tList.Add(t);
                }
            }

            return tList;
        }

        public static List<T> GetChanceList<T>(List<T> tData) where T : HasChance
        {
            List<T> tList = new List<T>();
            foreach (var t in tData)
            {
                for (int j = 0; j < t.chance; j++)
                {
                    tList.Add(t);
                }
            }

            return tList;
        }

        public static TValue GetOrAddDictionaryValue<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dict)
            where TValue : new()
        {
            if (dict.TryGetValue(key, out TValue value))
                return value;
            else
            {
                var val = new TValue();
                dict.Add(key, val);
                return val;
            }
        }

        public static T GetComponentInParentAndChild<T>(GameObject obj, bool isSearchParent = true) where T : class
        {
            if (ReferenceEquals(obj, null)) return null;

            if (obj.TryGetComponent(out T component)) return component;

            if (obj.transform.parent != null && isSearchParent)
            {
                return obj.transform.parent.GetComponentInChildren<T>();
            }
            else
            {
                return obj.GetComponentInChildren<T>();
            }
        }

        public static List<T> GetRandomElements<T>(List<T> originalList, int count)
        {
            List<T> shuffledList = new List<T>(originalList);
            shuffledList.Shuffle(); // 확장 메서드로 리스트를 섞음

            // 무작위로 섞인 리스트에서 처음 count개의 요소를 반환
            return shuffledList.GetRange(0, Mathf.Min(count, shuffledList.Count));
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static float CalculateDurationWithAtkSpeed(IStatUser actor, float value)
        {
            return value / (1 + actor.StatManager.GetFinalStat(ActorStatType.AtkSpeed));
        }

        public static bool CheckLayer(LayerMask layer, GameObject obj)
        {
            return (layer.value & (1 << obj.layer)) > 0;
        }


        public static void ActionAfterFrame(UnityAction action)
        {
            GameManager.instance.StartCoroutine(FrameCoroutine(action));
        }

        public static void ActionAfterTime(UnityAction action, float time)
        {
            GameManager.instance.StartCoroutine(TimeCoroutine(action, time));
        }

        static IEnumerator FrameCoroutine(UnityAction action)
        {
            yield return new WaitForEndOfFrame();
            action.Invoke();
        }

        static IEnumerator TimeCoroutine(UnityAction action, float time)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }

        public static bool CheckAngle(Vector2 myDir, Vector2 toDir, float angle)
        {
            return Vector2.Angle(myDir, toDir) <= angle * 0.5f;
        }

        public static Sequence DoInTime(TweenCallback action, float time)
        {
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(time);
            seq.AppendCallback(action);
            return seq;
        }

        public static bool GetLowestPointByRay(Vector2 origin, LayerMask layerMask, out Vector2 pos)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Mathf.Infinity, layerMask);
            if (hit.collider != null)
            {
                pos = hit.point;
                return true;
            }

            pos = Vector2.zero;
            return false;
        }

        public static void SetProjectilesAngle(List<Projectile> projectiles, float angle)
        {
            int half = Mathf.FloorToInt(projectiles.Count / 2f);
            for (int i = 0; i < projectiles.Count; i++)
            {
                float currAngle = (i - half) * angle;
                projectiles[i].Rotate(currAngle);
            }
        }

        public static void ActionOnPlayerReady(UnityAction<Player> action)
        {
            if (GameManager.instance.Player != null && GameManager.instance.Player.isStarted)
            {
                action.Invoke(GameManager.instance.Player);
            }
            else
            {
                void EquipUntil(Player player)
                {
                    action.Invoke(player);
                    GameManager.instance.afterPlayerStart.RemoveListener(EquipUntil);
                }

                GameManager.instance.afterPlayerStart.AddListener(EquipUntil);
            }
        }

        /// <summary>
        /// Dictinoary 깊은 복사 함수
        /// </summary>
        /// <param name="original">base dictionary</param>
        /// <param name="cloneFunc">값이 참조값일 수 있으므로, 참조값이면 복사해서 반환하도록 넣어주세요.</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> DeepCopyDictionary<TKey, TValue>(
            Dictionary<TKey, TValue> original,
            Func<TValue, TValue> cloneFunc)
        {
            return original.ToDictionary(entry => entry.Key, entry => cloneFunc(entry.Value));
        }

        public static HashSet<T> DeepCopyHashSet<T>(HashSet<T> original)
        {
            return new HashSet<T>(original);
        }

        public static List<T> DeepCopyList<T>(List<T> original)
        {
            return new List<T>(original);
        }
    }

    public static class ExtensionUtils // this 익스텐션 추가한 기능들
    {
        public static bool IsDontDestroy(this GameObject go)
        {
            return go.gameObject.scene.name is "DontDestroyOnLoad" or null;
        }

        public static T GetComponentInParentAndChild<T>(this Transform obj, bool isSearchParent = true)
            where T : class
        {
            if (ReferenceEquals(obj, null)) return null;

            if (obj.TryGetComponent(out T component)) return component;

            if (obj.parent != null && isSearchParent)
            {
                return obj.parent.GetComponentInChildren<T>();
            }
            else
            {
                return obj.GetComponentInChildren<T>();
            }
        }

        public static T[] GetComponentsInParentAndChild<T>(this Transform obj, bool isSearchParent = true)
            where T : class
        {
            if (ReferenceEquals(obj, null)) return null;

            if (obj.parent != null && isSearchParent)
            {
                return obj.parent.GetComponentsInChildren<T>();
            }
            else
            {
                return obj.GetComponentsInChildren<T>();
            }
        }

        public static Vector2 GetFloorPos(this IMonoBehaviour obj)
        {
            if (Utils.GetLowestPointByRay(obj.Position, LayerMasks.GroundAndPlatform, out var value))
            {
                return value;
            }

            return obj.transform.position;
        }

        // 자식들 가져오기
        public static Transform[] GetChildren(this Transform t)
        {
            int count = t.childCount;
            Transform[] children = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                children[i] = t.GetChild(i);
            }

            return children;
        }

        // 콜라이더 배열에서 타겟 목록 가져오기
        public static List<IOnHit> DistinctTargets(this IEnumerable<Collider2D> list)
        {
            return list.Select(x => x.transform.GetComponentInParentAndChild<IOnHit>())
                .Where(x => x is { IsDead: false })
                .Distinct().ToList();
        }

        // 타겟 목록 거리순으로 정렬
        public static List<IOnHit> OrderByDistance(this IEnumerable<IOnHit> list, Vector2 from)
        {
            return list.OrderBy(x => Vector2.Distance(x.Position, from)).ToList();
        }

        // 반경 내 타겟 가져오기
        public static List<IOnHit> GetTargetsInCircle(this GameObject obj, float radius, LayerMask mask)
        {
            var colliders = Physics2D.OverlapCircleAll(obj.transform.position, radius, mask);

            return colliders.DistinctTargets();
        }

        // 박스 범위 내 타겟 가져오기
        public static List<IOnHit> GetTargetsInBox(this GameObject obj, Vector2 size, LayerMask mask,
            Vector3? offset = null)
        {
            var colliders = Physics2D.OverlapBoxAll(obj.transform.position + (offset ?? Vector3.zero), size, 0, mask);
            return colliders.DistinctTargets();
        }

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();

            int count = enumerable1.Count();
            int rand = Random.Range(0, count);
            return enumerable1.ElementAt(rand);
        }

        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            return result;
        }

        public static void UpdateSelectedChildToScrollView(this ScrollRect instance, RectTransform child, float margin = 30, float duration = 0.5f)
        {
            if (instance == null || instance.content == null || child == null)
            {
                Debug.LogError("ScrollRect, ContentPanel, 또는 TargetItem이 할당되지 않았습니다!");
                return;
            }
            
            // 해당 ScrollRect 인스턴스를 ID로 사용하여 기존 트윈을 중지시킵니다.
            DOTween.Kill(instance); // 이 ScrollRect 인스턴스와 연결된 모든 트윈을 중지
    
            Canvas.ForceUpdateCanvases(); // UI 요소들의 현재 상태를 정확히 반영
    
            RectTransform content = instance.content;
            RectTransform viewport = instance.viewport; // viewport RectTransform 가져오기
    
            if (viewport == null) viewport = instance.GetComponent<RectTransform>(); // viewport가 명시적으로 설정 안된 경우 ScrollRect 자신의 RectTransform 사용
            
            // 1. Target의 월드 좌표 상의 4개 코너를 가져옵니다.
            Vector3[] targetCorners = new Vector3[4];
            child.GetWorldCorners(targetCorners);
            
            // 2. Viewport 입장에서 Target의 상대적인 위치(y값)를 계산합니다.
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (Vector3 worldCorner in targetCorners)
            {
                // 월드 좌표를 Viewport의 로컬 좌표로 변환
                float localY = viewport.InverseTransformPoint(worldCorner).y;
                if (localY < minY) minY = localY;
                if (localY > maxY) maxY = localY;
            }
            
            // 3. Viewport의 경계선 정의 (로컬 좌표계 기준)
            // Viewport의 Pivot이 중앙(0.5, 0.5)인 경우와 상단(0.5, 1)인 경우를 모두 대응
            float viewTop = viewport.rect.yMax;     // 뷰포트의 상단 끝 y
            float viewBottom = viewport.rect.yMin;  // 뷰포트의 하단 끝 y

            // 4. 스크롤 이동량 계산 (픽셀 단위)
            float scrollOffset = 0;

            if (maxY > viewTop)
            {
                // Target이 뷰포트 상단 위로 벗어남 -> 아래로 밀어내기
                scrollOffset = maxY - viewTop;
            }
            else if (minY < viewBottom)
            {
                // Target이 뷰포트 하단 아래로 벗어남 -> 위로 끌어올리기
                scrollOffset = minY - viewBottom;
            }
            else
            {
                // 이미 뷰포트 안에 다 들어와 있음 -> 아무것도 안 함
                return;
            }
            
            // 5. 픽셀 단위를 Normalized Position(0~1)으로 변환하여 적용
            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;
            float scrollableHeight = contentHeight - viewportHeight;

            float endValue = instance.verticalNormalizedPosition;
            
            if (scrollableHeight > 0)
            {
                // VerticalNormalizedPosition은 1(상단) ~ 0(하단)
                // scrollOffset이 양수면 위로 초과된 것이므로 값을 더해서 컨텐츠를 내림
                float normalizedDelta = scrollOffset / scrollableHeight;
                endValue = instance.verticalNormalizedPosition + normalizedDelta;
                endValue = Mathf.Clamp01(endValue);
            }
            

            DOTween.To(() => instance.verticalNormalizedPosition, v => instance.verticalNormalizedPosition = v, endValue, duration)
                .SetUpdate(true)
                .SetEase(Ease.OutQuad)
                .SetId(instance);
        }

        public static void UpdateFocusParentToScrollView(this ScrollRect instance, FocusParent focusParent, float margin = 30, float duration = 0.5f)
        {
            focusParent.FocusChanged.AddListener(ind =>
            {
                instance.UpdateSelectedChildToScrollView(focusParent.focusList[ind].rectTransform, margin, duration);
            });
        }

        // 컴포넌트를 추가하고 가져옴
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>() ?? go.AddComponent<T>();
            return component;
        }

        public static T GetOrAddComponent<T>(this Component go) where T : Component
        {
            T component = go.GetComponent<T>() ?? go.gameObject.AddComponent<T>();
            return component;
        }

        public static T GetOrAddComponent<T>(this GameObject go, bool allowDuplication) where T : Component
        {
            if (!allowDuplication && go.TryGetComponent(out T value))
            {
                return value;
            }

            T component = go.GetComponent<T>() ?? go.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// 컴포넌트 추가 함수
        /// </summary>
        /// <param name="allowDuplication">중복 허용 여부</param>
        public static void AddComponent<T>(this GameObject go, bool allowDuplication = true) where T : Component
        {
            if (!allowDuplication && go.TryGetComponent(out T value))
            {
                return;
            }

            go.AddComponent<T>();
        }

        public static void SetRadius(this Component component, float radius)
        {
            component.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }

        public static void SetRadius(this GameObject go, float radius)
        {
            go.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }

        public static void SetRadius(this GameObject go, float radius, EActorDirection dir)
        {
            go.transform.localScale = new Vector3((int)dir * radius * 2, radius * 2, 1);
        }

        public static TweenerCore<Vector2, Vector2, VectorOptions> KillWhenBoxCast(this TweenerCore<Vector2, Vector2, VectorOptions> t2,
            Rigidbody2D rb,
            Vector2 boxSize,
            LayerMask mask)
        {
            bool killRequested = false;

            
            var originalSetter = t2.setter;   // 기존 setter 저장

            t2.setter = newPos =>
            {
                if (killRequested)
                    return;

                Vector2 current = rb.position;
                Vector2 delta = newPos - current;
                float dist = delta.magnitude;

                if (dist > 0)
                {
                    var hit = Physics2D.BoxCast(current, boxSize, 0, delta.normalized, dist, mask);
                    if (hit)
                    {
                        killRequested = true;
                        return;
                    }
                }

                // 안전할 때만 기존 setter 호출
                originalSetter(newPos);
            };

            t2.OnUpdate(() =>
            {
                if (killRequested)
                    t2.Kill();
            });

            return t2;
        }
        
        static float CalcApexJumpY(float t, float startY, float endY, float jumpHeight)
        {
            float apexY = Mathf.Max(startY, endY) + jumpHeight;

            if (t < 0.5f)
            {
                float upT = t / 0.5f;
                return Mathf.Lerp(startY, apexY, DOVirtual.EasedValue(0, 1, upT, Ease.OutQuad));
            }
            else
            {
                float downT = (t - 0.5f) / 0.5f;
                return Mathf.Lerp(apexY, endY, DOVirtual.EasedValue(0, 1, downT, Ease.InQuad));
            }
        }
        
        public static (Tween xTween, Tween yTween) DoJumpApex(
            this Rigidbody2D rb,
            Vector2 endPos,
            float jumpHeight,
            float duration,
            Vector2 boxSize,
            LayerMask wallMask,Ease xEase = Ease.Linear)
        {
            Vector2 startPos = rb.position;

            float xT = 0f;
            float yT = 0f;

            bool blockX = false;

            // X 진행도 Tween
            Tween xTween = DOTween.To(() => xT, v => xT = v, 1f, duration)
                .SetEase(xEase)
                .SetUpdate(UpdateType.Fixed);

            // Y 진행도 Tween
            Tween yTween = DOTween.To(() => yT, v => yT = v, 1f, duration)
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed);

            // === 최종 이동 Setter (KillWhenBoxCast 철학 유지) ===
            xTween.OnUpdate(() =>
            {
                float easedX = DOVirtual.EasedValue(0, 1, xT, xEase);

                float targetX = Mathf.Lerp(startPos.x, endPos.x, easedX);
                float targetY = CalcApexJumpY(yT, startPos.y, endPos.y, jumpHeight);

                Vector2 current = rb.position;

                // X만 벽 체크
                if (!blockX)
                {
                    Vector2 xOnlyTarget = new Vector2(targetX, current.y);
                    Vector2 delta = xOnlyTarget - current;

                    if (delta.sqrMagnitude > 0.0002f)
                    {
                        if (Physics2D.BoxCast(
                                current,
                                boxSize,
                                0,
                                delta.normalized,
                                delta.magnitude,
                                wallMask))
                        {
                            blockX = true;
                            targetX = current.x;
                        }
                    }
                }
                else
                {
                    targetX = current.x;
                }

                Vector2 nextPos = new Vector2(targetX, targetY);
                rb.MovePosition(nextPos);
            });

            return (xTween, yTween);
        }

        public static Tween LaunchTileSprite(this SpriteRenderer render, Vector2 endPos, float fireTime,
            Ease ease)
        {
            Vector2 dir = endPos - (Vector2)render.transform.position;
            dir.Normalize();

            render.transform.right = -dir;

            float distance = Vector2.Distance(render.transform.position, endPos);

            Tween tweener = DOTween.To(() => render.size, x => render.size = x,
                new Vector2(distance / render.transform.localScale.x, render.size.y),
                fireTime).SetUpdate(UpdateType.Fixed).SetEase(ease).SetAutoKill(true);

            return tweener;
        }

        public static Tween ReturnTileSize(this SpriteRenderer render, float time, Ease ease)
        {
            Tween tweener = DOTween.To(() => render.size, x => render.size = x,
                new Vector2(0, render.size.y),
                time).SetUpdate(UpdateType.Fixed).SetEase(ease).SetAutoKill(true);

            return tweener;
        }

        #region one time

        public static void RegisterOneTime(Action<Action> subscribe, Action<Action> unsubscribe, Action handler)
        {
            Action wrapper = null;
            wrapper = () =>
            {
                handler?.Invoke();
                unsubscribe(wrapper);
            };
            subscribe(wrapper);
        }

        public static void RegisterOneTime<T>(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe,
            Action<T> handler)
        {
            Action<T> wrapper = null;
            wrapper = (arg) =>
            {
                handler?.Invoke(arg);
                unsubscribe(wrapper);
            };
            subscribe(wrapper);
        }

        public static void RegisterOneTime(UnityEvent unityEvent, UnityAction handler)
        {
            UnityAction wrapper = null;
            wrapper = () =>
            {
                handler?.Invoke();
                unityEvent.RemoveListener(wrapper);
            };
            unityEvent.AddListener(wrapper);
        }

        public static void RegisterOneTime<T>(UnityEvent<T> unityEvent, UnityAction<T> handler)
        {
            UnityAction<T> wrapper = null;
            wrapper = (arg) =>
            {
                handler?.Invoke(arg);
                unityEvent.RemoveListener(wrapper);
            };
            unityEvent.AddListener(wrapper);
        }

        #endregion
    }

    public static class FormatUtils
    {
        public static long CurrentTimeToId(string formatter = "yyMMddHHmmss")
        {
            DateTime currentTime = DateTime.Now;
            Debug.Log(currentTime.ToString(formatter));
            return long.Parse(currentTime.ToString(formatter));
        }

        public static string TimeDisplay(float second)
        {
            int days = (int)(second / (24 * 3600)); // 일 수 계산
            int hours = (int)((second % (24 * 3600)) / 3600); // 시간 계산
            int minutes = (int)((second % 3600) / 60); // 분 계산
            int seconds = (int)(second % 60); // 초 계산

            if (days > 0)
            {
                return $"{days}{(days == 1 ? "day" : "days")} {hours:D2}:{minutes:D2}:{seconds:D2}";
            }
            else
            {
                return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            }
        }

        public static int GetRatioIntByInt(int value, int from, int to)
        {
            int realInt = Mathf.RoundToInt((float)value / (from - 1) * (to - 1));
            return Mathf.Clamp(realInt, 0, to - 1);
        }
    }

    public static class Calc
    {
        public static int GetDigits(int n)
        {
            return n == 0 ? 1 : (int)Math.Floor(Math.Log10(n) + 1);
        }

        public static int ConcatInts(int a, int b)
        {
            return a * (int)Math.Pow(10, GetDigits(b)) + b;
        }
    }
}