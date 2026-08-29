# Prototype

**Unity 2D 액션 게임을 위한 베이스 템플릿 프로젝트.**

새 프로젝트마다 반복해서 다시 만들게 되는 시스템 — 애셋 로딩, 게임 상태, 전투, 버프, 스킬, UI, 사운드, 세이브 — 을 하나의 재사용 가능한 구조로 정리했습니다.
실제 게임 프로젝트에서 쓰면서 문제가 드러난 부분을 계속 리팩토링하며 다듬고 있습니다.

<br/>

![Unity](https://img.shields.io/badge/Unity-6000.3-000000?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)
![Addressables](https://img.shields.io/badge/Addressables-2.9-1a73e8)
![Input System](https://img.shields.io/badge/Input%20System-1.18-6f42c1)

| | |
|---|---|
| **엔진** | Unity 6000.3 / URP (2D) |
| **핵심 패키지** | Addressables, Input System, Cinemachine, 2D Feature Set |
| **외부 라이브러리** | Odin Inspector, DOTween, Easy Save 3, Spine, Newtonsoft.Json |
| **규모** | 스크립트 490개 / 약 39,500줄 (외부 라이브러리 제외) |
| **에디터 툴** | Behaviour Tree 비주얼 에디터 (UI Toolkit / GraphView) |

<br/>

<!--
────────────────────────────────────────────────────────────────
[촬영 후 아래 주석을 풀고 이미지 URL을 채워주세요]

|  |  |
|:---:|:---:|
| ![Behaviour Tree 에디터](URL) | ![UI 포커스 네비게이션](URL) |
| **Behaviour Tree 에디터** — 노드 조립으로 AI 구성 | **UI 네비게이션** — 패드/키보드 그룹 포커스 이동 |
| ![스킬 사용 방식](URL) | ![투사체 확장](URL) |
| **스킬 5종** — 즉발/차지/캐스팅/토글/지속 | **투사체** — 유도 · 관통 · 반사 · 방사 |
────────────────────────────────────────────────────────────────
-->

<br/>

## 목차

- [이 프로젝트에서 봐주었으면 하는 것](#이-프로젝트에서-봐주었으면-하는-것)
  - [1. 애셋 수명 관리 — 개별 추적 대신 스코프 단위 해제](#1-애셋-수명-관리--개별-추적-대신-스코프-단위-해제)
  - [2. 게임 상태 — 플래그 대신 티켓](#2-게임-상태--플래그-대신-티켓)
  - [3. Actor — 기능을 상속이 아니라 이벤트로 붙인다](#3-actor--기능을-상속이-아니라-이벤트로-붙인다)
- [시스템별 상세](#시스템별-상세)
  - [Actor / 전투](#sys-actor)
  - [버프 시스템](#sys-buff)
  - [스킬 / 스킬트리](#sys-skill)
  - [Behaviour Tree (AI)](#sys-bt)
  - [AttackObject / Projectile](#sys-attackobject)
  - [UI](#sys-ui)
  - [Sound](#sys-sound)
  - [Save / Scene / Database](#sys-save)
  - [Stage / 레벨 오브젝트](#sys-stage)
- [폴더 구조](#폴더-구조)

<br/>

---

# 이 프로젝트에서 봐주었으면 하는 것

전체를 다 보실 필요는 없습니다. 아래 세 가지가 **설계 판단이 가장 많이 들어간 부분**입니다.

<br/>

## 1. 애셋 수명 관리 — 개별 추적 대신 스코프 단위 해제

> `Assets/Scripts/Utils/Resource/`

### 문제

초기 구조는 `Addressables.LoadAssetAsync`를 필요한 곳에서 그때그때 호출하고, 호출부가 각자 `Release`를 책임지는 방식이었습니다. 두 가지가 문제였습니다.

- **해제 책임이 흩어짐** — 누가 언제 해제해야 하는지 코드마다 달라서, 핸들 누수와 이중 해제가 동시에 생겼습니다.
- **런타임 동기 로드** — 전투 중 처음 등장하는 이펙트·사운드가 그 자리에서 로드되면서 프레임 스파이크와 GC 스파이크가 발생했습니다.

### 접근

애셋을 **개별로 추적하지 않고, 수명이 같은 것끼리 묶어 통째로 해제**하는 구조로 바꿨습니다.

```csharp
public enum AssetLifetime
{
    Global,  // 게임 종료까지 유지. 공용 UI 프리팹, 사운드 믹서, 공용 데이터
    Scene    // 씬을 벗어날 때 일괄 해제. 스테이지 전용 몬스터/배경/BGM
}
```

**`AssetScope`** — 수명 단위 하나. 로드한 `AsyncOperationHandle`의 소유권은 전적으로 이 클래스에 있고, 해제 경로는 `Dispose` 하나뿐입니다. 그래서 호출부는 `Addressables.Release`를 부를 일이 아예 없습니다.

**`AssetRegistry`** — 스코프 보관소. 씬 전환 시 `ReleaseScene()` 한 번으로 그 씬이 올린 애셋이 전부 반납됩니다.

```csharp
public static void ReleaseScene()
{
    Scene.Dispose();
    Scene = new AssetScope("Scene");
}
```

**프리로드를 데이터로 분리** — 무엇을 미리 올릴지는 코드가 아니라 ScriptableObject가 들고 있습니다.

```csharp
[CreateAssetMenu(menuName = "Config/Preload Manifest")]
public class PreloadManifest : ScriptableObject
{
    public string[] labels;      // Addressables 라벨 단위로 통째로 로드
    public string[] addresses;   // 개별 주소 지정
    public PrewarmEntry[] prewarm;  // 풀에 미리 생성해 둘 오브젝트
}
```

`SceneManifestTable`이 씬 이름 → 매니페스트를 매핑하고, 로딩 화면이 씬을 활성화하기 직전에 `PreloadSceneAsync(sceneName)`을 호출합니다. **오브젝트 풀 prewarm까지 이 단계에 포함**시켜서, 게임플레이 중에는 로드도 인스턴스 생성도 일어나지 않습니다.

### 이 구조에서 신경 쓴 지점

**① 프리로드 누락을 사람이 찾지 않게 했습니다.**
매니페스트에 빠진 애셋은 결국 런타임 동기 로드로 이어지는데, 이건 눈으로 찾을 수가 없습니다. 그래서 `AssetScope`가 동기 로드된 주소를 기록하고, 진단 API로 뽑아볼 수 있게 했습니다.

```csharp
/// <summary>프리로드되지 않아 동기 로드된 주소 목록을 출력한다. 매니페스트 작성용.</summary>
public static void LogPreloadReport() => AssetScope.LogSyncLoadReport();
```

한 바퀴 플레이하고 이 로그를 그대로 매니페스트에 옮기면 됩니다.

**② 프리팹 인스턴스가 핸들을 갖지 않게 했습니다.**
`Addressables.InstantiateAsync`는 인스턴스마다 핸들을 만들어서 풀링과 충돌합니다. 그래서 **프리팹은 스코프가 핸들 하나로 붙잡고, 인스턴스는 순수 `Object.Instantiate`로** 만듭니다. 인스턴스 파기는 `Destroy`만으로 충분해집니다.

**③ 도메인 리로드를 꺼도 깨지지 않게 했습니다.**
Enter Play Mode Options로 도메인 리로드를 끄면 static이 유지되어 이전 플레이 세션의 죽은 핸들이 남습니다.

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
private static void ResetStatics()
{
    Global = new AssetScope("Global");
    Scene  = new AssetScope("Scene");
}
```

**④ BGM은 라벨 통째로 올리지 않습니다.**
SFX는 짧고 전역에서 쓰이므로 라벨 단위로 다 올리지만, BGM은 클립 하나가 수 MB라 전부 올리면 메모리를 크게 먹습니다. 씬에서 쓰는 BGM만 씬 매니페스트에 등록해서 Scene 스코프와 함께 해제되도록 했습니다.

<!-- [촬영] 프로파일러 before/after 스크린샷 — 리팩토링 이전 커밋(44ade73)과 비교한 GC Alloc / 프레임 스파이크
| 리팩토링 전 | 리팩토링 후 |
|:---:|:---:|
| ![before](URL) | ![after](URL) |
-->

<br/>

## 2. 게임 상태 — 플래그 대신 티켓

> `Assets/Scripts/Utils/Managers/GameManager.State.cs`

### 문제

"지금 조작이 가능한가"를 bool 플래그로 관리하면, **여러 주체가 동시에 같은 상태를 요구할 때** 무너집니다.

컷신이 조작을 막고, 그 위에 팝업이 떠서 또 조작을 막고, 팝업이 먼저 닫히면서 플래그를 풀어버리면 — 컷신 중인데 플레이어가 움직입니다. 반대로 어느 한쪽이 해제를 빠뜨리면 게임이 영원히 멈춥니다.

### 접근

상태를 켠 사람마다 **Guid 티켓**을 발급하고, 티켓이 하나도 남지 않아야 실제로 해제되도록 했습니다.

```csharp
// 상태를 켠다 → 티켓을 받는다
var guid = GameManager.instance.TryOnGameState<InteractionState>();

// 자기가 받은 티켓으로만 끈다
GameManager.instance.TryOffGameState<InteractionState>(guid);
```

```csharp
/// <summary>
///     상태별로 "이 상태를 켜 둔 사람들"의 티켓. 개수가 0보다 크면 켜진 것이다.
///     on/off 플래그를 따로 두지 않는 이유: 장부가 둘이면 서로 어긋날 수 있기 때문.
/// </summary>
private readonly Dictionary<GameState, HashSet<Guid>> _stateGuids = new();
```

**우선순위로 활성 상태를 결정합니다.** 동시에 여러 상태가 켜져 있을 수 있고, 그중 `Priority`가 가장 작은 것이 현재 상태가 됩니다.

```csharp
public static class StatePriority
{
    public const int Default     = 0;    // UI만 조작 가능. 페이드 중, 플레이어 없는 씬
    public const int Interaction = 10;   // UI + 기본 조작. 상호작용 UI가 떠 있는 상태
    public const int Play        = 100;  // 일반 플레이. 가장 낮으므로 기본 상태가 된다
}
```

상태 전환 함수는 `private`입니다. **외부에서 상태를 직접 바꿀 수 없고, 오직 티켓을 통해서만** 바뀝니다. 이게 "누가 상태를 바꿨는지 모르겠는" 상황을 원천적으로 막습니다.

### 이 구조에서 신경 쓴 지점

**티켓 보유자가 사라지는 경우를 처리했습니다.**
씬이 전환되면 티켓을 들고 있던 UI가 통째로 사라집니다. 그러면 아무도 해제할 수 없는 티켓이 영구히 남아 게임이 멈춥니다.

```csharp
// 씬이 바뀌면 일시정지 guid 보유자(UI 등)가 통째로 사라지므로 남은 일시정지를 먼저 푼다.
// WhenSceneLoaded가 아니라 Begin에 거는 이유: 새 씬의 UI가 등록한 일시정지까지 지우면 안 되기 때문.
Scene.WhenSceneLoadBegin.AddListener(_ => ClearAllPauses());
```

일시정지(`RegisterPause` / `RemovePause`)도 같은 티켓 방식이며, 정리 시점을 `WhenSceneLoaded`가 아니라 `WhenSceneLoadBegin`으로 잡은 것이 핵심입니다.

### 템플릿과 게임 코드의 분리

이 프로젝트는 템플릿이므로, **게임별 코드가 템플릿을 오염시키지 않아야** 합니다. `partial void` 훅으로 처리했습니다.

```csharp
// GameManager.cs (템플릿)
partial void OnSampleAwake();   // 선언만

protected override void Awake()
{
    // ... 템플릿 초기화 ...
    OnSampleAwake();  // GameManager.Sample.cs가 없으면 이 호출은 컴파일 단계에서 사라진다
}
```

```csharp
// GameManager.Sample.cs (게임별 — 지워도 컴파일된다)
partial void OnSampleAwake()
{
    RegisterState(new BattleState());  // 게임 고유 상태 등록
}
```

게임별 상태는 `RegisterState<T>()`로 추가하고, `StatePriority` 사이 값을 골라 우선순위를 정합니다. `GameManager.Sample.cs`와 `GameState/Sample/` 폴더만 지우면 순수 템플릿이 됩니다.

<br/>

## 3. Actor — 기능을 상속이 아니라 이벤트로 붙인다

> `Assets/Scripts/Actor/`

### 문제

유닛에 기능을 추가할 때 상속으로 처리하면, "독 데미지를 주는 몬스터"와 "폭발하는 몬스터"와 "독 데미지를 주면서 폭발하는 몬스터"가 각각 클래스가 되면서 조합 폭발이 일어납니다.

### 접근

`Actor`를 **기능 구현체가 아니라 조합 컨테이너**로 두고, 기능은 이벤트 구독으로 붙입니다.

```csharp
actor.AddEvent(EventType.OnHit, OnHitHandler);
```

전투는 하나의 함수가 아니라 **62개의 이벤트 단계**로 쪼개져 있어서, 어느 지점에든 기능을 끼워 넣을 수 있습니다. 버프·스킬·아이템·AI가 전부 이 이벤트에만 의존하므로 서로를 직접 참조하지 않습니다.

### 실제 전투 흐름

`ActorCombat`이 공격 측과 피격 측 흐름을 각각 담당합니다.

```
[공격 측] ActorCombat.Attack()
  OnAttackSuccess     → 데미지 증가, 크리 확률 증가 등 효과 적용
  OnBasicAttack       → 기본 공격 한정 효과
  ─────────────────── 여기서 데미지 계산 (IAttackStrategy)
  OnCrit              → 크리티컬 발생 시
  OnBackAttack        → 뒤에서 때렸을 때 (Vector2.Dot으로 판정)
  ─────────────────── target.OnHit() 호출
  OnAfterAtk          → 최종 입힌 데미지 확인

[피격 측] ActorCombat.ReceiveHit()
  OnBeforeHit         → 무적/방어/회피. hitDisable을 켜면 여기서 중단
  ─────────────────── 방어력 공식 적용
  OnHit               → 피격 처리
  OnCritHit           → 크리티컬로 맞았을 때
  ─────────────────── CurHp 차감
  OnAfterHit          → 증감이 전부 적용된 후
```

**이벤트 실행 순서가 계산 순서보다 앞선다**는 점이 중요합니다. 데미지 증가·크리 확률 증가 효과가 계산에 반영되려면 계산 전에 이벤트가 돌아야 하기 때문입니다.

```csharp
// 이벤트 실행을 데미지 계산 전에 호출해야함
// 데미지 증가, 크리티컬 확률 증가 등 효과들이 적용되어야 하기 때문
_actor.ExecuteEvent(EventType.OnAttackSuccess, eventParameters);
```

임시 스탯 보정은 `BonusStatEvent`에 델리게이트를 붙였다가 `finally`에서 반드시 떼어내는 방식으로, 예외가 나도 보정이 남지 않게 했습니다.

### 책임 분리

`Actor`는 `partial class`로 관심사별 파일로 나뉘어 있고, 실제 로직은 별도 클래스가 들고 있습니다.

| 파일 / 클래스 | 책임 |
|---|---|
| `Actor.cs` | 컨테이너, 생명주기, 방향 |
| `Actor.Event.cs` | 이벤트 등록/실행 (`ActorEvents`에 위임) |
| `Actor.Stat.cs` | 스탯, 배리어 (`BarrierCalculator`) |
| `Actor.Buff.cs` | 버프 보유 |
| `Actor.Immunity.cs` | 무적/면역 (`ImmunityController`) |
| `Actor.View.cs` | 렌더러 추상화 (`IActorRenderer` — 일반 스프라이트 / Spine 교체 가능) |
| `ActorCombat` | 공격·피격 흐름 |
| `EffectSpawner` | 이펙트 생성 |

<br/>

---

# 시스템별 상세

<br/>

<a id="sys-actor"></a>
<details>
<summary><b>▶ Actor / 전투</b> — 유닛 기본 클래스, 이벤트 파이프라인, 렌더러 추상화</summary>

<br/>

위 [3번 항목](#3-actor--기능을-상속이-아니라-이벤트로-붙인다)에서 다룬 내용의 나머지입니다.

### 렌더러 추상화

`IActorRenderer`로 일반 스프라이트와 Spine을 같은 인터페이스 뒤에 두었습니다. Spine 유닛은 `SpineRootMotionHelper`가 루트 모션을 물리와 연동합니다.

### Player

| 구성 | 설명 |
|---|---|
| `PlayerStateMachine` | Idle / Move / Jump / Dash / Attack / Skill 상태 전이. `NextState` 테이블로 전이 가능 여부 정의 |
| `IInterruptable` | 상태별로 어떤 입력에 끊길 수 있는지를 인터페이스로 분리 |
| `InputBuffer` | 선입력 버퍼. `PriorityQueue` 기반이라 버퍼가 찼을 때 낮은 우선순위 입력이 밀려남 |
| `IPlayerAttack` / `IPlayerDash` | 공격·대시 구현을 인터페이스로 분리해 캐릭터별 교체 가능 |
| `PlayerCooldown` | 쿨다운 통합 관리 |
| `Command` 패턴 | `PlayerCommand` 파생 클래스로 입력→행동 매핑 (Move / Jump / Dash / Interact / UseActiveSkill …) |

### Monster

| 구성 | 설명 |
|---|---|
| `Monster` | Actor 상속. `partial`로 OnOff / Interaction / Util 분리 |
| `Recognition` | 플레이어 인식 범위 · 인식 상태 진입/이탈 이벤트 (`OnRecognitionEnter` / `OnRecognitionExit`) |
| `PatternGroup` | 공격/이동 패턴을 그룹으로 묶어 조합 |
| `CommonMonster/State` | 일반 몬스터용 경량 상태 머신 |
| `BossMonster` | Behaviour Tree + `AnimBehaviour` 기반 페이즈 제어 |

</details>

<br/>

<a id="sys-buff"></a>
<details>
<summary><b>▶ 버프 시스템</b> — Buff / SubBuff 계층, 해제 전략, 계산 분리</summary>

<br/>

### 왜 두 계층인가

버프를 단일 리스트로 관리하면 "적을 공격하면 독을 부여한다" 같은 효과를 표현할 수 없습니다. **효과를 부여하는 주체**와 **실제로 붙는 상태이상**이 다른 대상에 붙기 때문입니다.

| 계층 | 역할 | 예시 |
|---|---|---|
| `Buff` | 상위 효과. 조건과 부여 규칙을 가짐 | "적을 공격 시 독 부여" |
| `SubBuff` | 실제로 대상에 붙는 상태이상 | 대상에게 붙은 독 디버프 |

하나의 `Buff`가 여러 `SubBuff`를 생성·관리하며, 서로 다른 대상에 붙일 수 있습니다.

### 적용 방식 (Strategy)

```csharp
_applyStrategy = ApplyType switch
{
    0 => new NormalApply(this),      // 조건 충족 시 적용
    1 => new PermanentApply(this),   // 영구 적용
    2 => new TempApply(this),        // 일시 적용
};
```

### SubBuff 계층

```
SubBuff
├─ Buff_Base        ─ Buff_Stat        (스탯 증가)
├─ Debuff_base      ─ Debuff_Stat      (스탯 감소)
│                   ├─ Debuff_CC       (군중제어)
│                   └─ Debuff_DotDmg   (지속 피해)
└─ BarrierBase                          (배리어)
```

### 부가 전략

- **`DispellStrategy`** — 해제 조건 분리 (시간 경과 / 특정 이벤트 / 영구)
- **`IBuffUpdate`** — 갱신 방식 분리 (`DotDmgUpdate` 등)
- **`IBuffCollectionUpdate`** — 스택 처리 방식 분리
- **타입 기반 제거·면역** — `SubBuffType`으로 "독 계열 전부 해제", "화상 면역" 같은 처리
- **`SubBuffCollector`** — 같은 타입이 몇 개 붙어 있는지로 최초 적용/최종 해제 시점 판정

```csharp
public virtual void OnAdd()
{
    if (_user.SubBuffCount(Type) == 1) OnTypeAdd();  // 이 타입의 첫 스택일 때만
    OnBuffAdd.Invoke(this);
}
```

### 계산 로직 분리

배리어처럼 상태 변경과 계산이 얽히는 것은 별도 계산기로 뺐습니다.

```csharp
user.BarrierCalculator.BarrierAddEvent   += AddBarrier;
user.BarrierCalculator.BarrierMinusEvent -= MinusBarrier;
```

</details>

<br/>

<a id="sys-skill"></a>
<details>
<summary><b>▶ 스킬 / 스킬트리</b> — 사용 방식 5종, 데코레이터 스탯 합성, Visitor 스킬트리</summary>

<br/>

### 사용 방식을 전략으로 분리

스킬을 "발동한다"가 아니라 **"어떻게 발동되는가"를 교체 가능한 축**으로 뒀습니다. `ISkillActive` 구현체를 바꾸면 같은 스킬이 즉발형에서 차지형이 됩니다.

| 구현체 | 동작 |
|---|---|
| `InstantSkill` | 즉발 |
| `ChargeSkill` | 누르는 동안 차징 → 뗄 때 발동 (`OnChargeEnd` / `OnChargeCancel`) |
| `CastingSkill` | 캐스팅 시간 후 발동 (`OnCastingEnd` / `OnCastingCancel`) |
| `ToggleSkill` | 켜고 끄기 |
| `ContinuousSkill` | 누르는 동안 지속 |

```csharp
public interface ISkillActive
{
    bool durationUse { get; }     // 지속시간 사용 여부
    bool CheckUsable { get; }     // 사용 가능 여부
    void Activate(ActiveSkill skill);
    void DeActivate(ActiveSkill skill);
    float CalculateDmg(float dmg);
    void OnCancel();
    void OnUnEquip(ActiveSkill skill);
}
```

### 스탯 합성 (Decorator)

기본 스킬 스탯 위에 강화·룬·장비 효과를 겹칠 때, 원본을 수정하지 않고 감쌉니다.

```csharp
public class SkillDecorator : ISkill
{
    private readonly ISkill config;      // 원본
    private readonly ISkill attachment;  // 덧붙는 효과

    public SkillStat Stat => config.Stat + attachment.Stat;
}
```

중첩이 가능하므로 강화가 몇 겹이든 같은 방식으로 처리됩니다.

### 스킬트리 (Visitor)

`SkillTree`는 `SerializedScriptableObject`이고 `ISkillVisitor`를 구현합니다. 액티브/패시브 각각에 대해 다른 처리를 하도록 오버로드로 분기시켰습니다.

```csharp
public virtual void Activate(PlayerActiveSkill active, int level)  { ... }
public virtual void Activate(PlayerPassiveSkill passive, int level) { ... }
```

이름·설명은 `LanguageManager`의 문자열 테이블에서 가져와 다국어를 지원합니다.

</details>

<br/>

<a id="sys-bt"></a>
<details>
<summary><b>▶ Behaviour Tree (AI)</b> — 비주얼 에디터 + 3종 노드 확장</summary>

<br/>

<img width="1919" height="1000" alt="Behaviour Tree 에디터" src="https://github.com/user-attachments/assets/673cba93-0ca7-491f-b5b7-8c17376fd8d8" />

<br/><br/>

UI Toolkit / GraphView 기반 **비주얼 에디터를 직접 제작**해서, 기획자가 코드 없이 노드를 조립해 AI를 구성할 수 있게 했습니다.

<!-- [촬영] 에디터에서 노드를 연결하고 플레이 중 실행 노드가 하이라이트되는 GIF -->

### 노드 구조

| 종류 | 역할 | 구현 예 |
|---|---|---|
| `ActionNode` | 실제 행동 | `MoveNode`, `DashToPos`, `JumpNode`, `SetAnimation`, `TeleportToPos`, `BossAtk` |
| `DecoratorNode` | 조건 · 흐름 제어 | `HpCheck`, `IfPlayerDistance`, `CoolDownCheck`, `RaycastCheck`, `RepeatNode`, `CheckPhase` |
| `CompositeNode` | 자식 노드 조합 | `SequenceNode`, `SelectNode`, `ExcuteAll`, `ProbSelect` (확률 선택) |

새 노드는 세 베이스 중 하나를 상속하면 에디터 메뉴에 자동으로 노출됩니다. 공용(`Common`) / 몬스터 / 보스 / 플레이어로 네임스페이스가 나뉘어 있어 목적별로 골라 쓸 수 있습니다.

### 런타임

```csharp
tree = tree.Clone();          // 인스턴스마다 트리 복제 — SO 원본 오염 방지
tree.Init(actor, Repeat);
```

- **`StartType`** — `OnStart` / `ByScript`로 시작 시점 제어
- **`UpdateType`** — `Normal` / `Fixed`로 물리 기반 AI 대응
- **`BlackBoard`** — 노드 간 상태 공유 및 현재 실행 노드 추적 (에디터 하이라이트용)

</details>

<br/>

<a id="sys-attackobject"></a>
<details>
<summary><b>▶ AttackObject / Projectile</b> — 데미지 계산 · 판정 방식 · 투사체 확장의 3축 분리</summary>

<br/>

공격 오브젝트를 **"얼마를 때리는가" / "어떻게 판정하는가" / "어떻게 날아가는가"** 세 축으로 나눠서, 각각 독립적으로 교체할 수 있게 했습니다.

### 축 1 — 데미지 계산 (`IAttackStrategy`)

```csharp
public interface IAttackStrategy
{
    float DmgRatio { get; set; }
    float Calculate(IOnHit target);
}
```

| 구현체 | 계산식 |
|---|---|
| `FixedAmount` | 고정 수치 |
| `AtkBase` | 시전자 공격력 × 계수 + 기본값 |
| `AtkItemCalculation` | (무기 공격력 + 시전자 공격력) × 계수 |

### 축 2 — 판정 방식 (`IAttackType`)

같은 콜라이더라도 판정 규칙이 다릅니다.

| 타입 | 동작 |
|---|---|
| `Normal` | 콜라이더 진입 시마다 |
| `Once` | 초기화 전까지 같은 대상 1회만 |
| `Tick` | 일정 주기마다 (장판) |
| `Delay` | 진입 후 지연 발동 |
| `Cd` | 대상별 쿨다운 |
| `OnlyFirst` | 최초 1회만 |

생성은 `AttackObjectFactory`가 풀링 기반으로 처리하고, 공격마다 `attackGuid`를 발급해 **같은 공격에 여러 번 맞는 중복 판정을 차단**합니다.

```csharp
public bool CheckDuplicationAtk(AttackObject atkObj)
    => recentHitInfo != Guid.Empty && atkObj.firedAtkGuid == recentHitInfo;
```

### 축 3 — 투사체 (`Projectile`)

물리 파라미터(중력·가속도·최대 이동거리·초기 속도·방향 회전·속도 0 처리)를 인스펙터에서 조절하고, **충돌 대상별로 다른 반응**을 지정합니다.

```csharp
public enum ProjectileConflictType
{
    None,       // 아무일도 없음
    Destroy,    // 파괴
    Reflect,    // 닿은 면에 대해 반사
    Penetrate,  // 관통 (최대 횟수 · 관통당 데미지/크기 증감)
    Stop        // 정지
}
```

벽 / 바닥 / 타겟 / 보스에 각각 다른 타입을 줄 수 있어서, "벽에는 반사되고 적은 관통하는" 투사체 같은 조합이 설정만으로 나옵니다.

### 확장은 컴포넌트 조합으로

기능을 상속으로 늘리지 않고 `ProjectileExtension` 컴포넌트를 붙이는 방식입니다. Odin의 `[Button]`으로 **인스펙터에서 드롭다운으로 추가**할 수 있게 해서 기획자도 조합할 수 있습니다.

| 확장 | 기능 |
|---|---|
| `GuideExtension` | 유도 |
| `RadialFunction` | 파괴 시 방사체 생성 |
| `CreatePlateExtension` | 파괴 시 장판 생성 |
| `StickTargets` | 대상이 투사체에 붙음 |
| `SoundWaveExtension` | 점점 커지거나 작아짐 |

파생 타입으로 `Boomerang`, `CircleAroundProjectile`, `Grab` 등이 있습니다.

<!-- [촬영] 유도 · 관통 · 반사 · 방사가 각각 동작하는 GIF -->

</details>

<br/>

<a id="sys-ui"></a>
<details>
<summary><b>▶ UI</b> — 계층 · 상태 요소 · 그룹 포커스 · 패드 대응</summary>

<br/>

Unity 기본 UI를 그대로 쓰지 않고, **계층 / 요소 / 입력 내비게이션 / 포커스**를 각각 분리해 재설계했습니다. 게임패드 대응이 목적입니다.

### 계층

`UIManager`가 타입별로 다른 정렬 순서와 생명주기를 관리합니다.

| 타입 | 역할 | 관리 방식 |
|---|---|---|
| `UI_Main` | 항상 유지 (HUD) | 리스트, 활성화와 무관 |
| `UI_Scene` | 씬 단위 고정 창 | 단일 |
| `UI_Popup` | 팝업 | 스택 (겹칠수록 order 증가) |
| `UI_Ingame` | 월드 좌표 추종 (몬스터 HP바 등) | 리스트 |
| `UI_Hover` | 마우스 추종 (드래그 아이템 등) | 최상위 order |

UI 프리팹은 `AddressablePooling` 기반으로 풀링되며, 루트(`@UI_Root`)는 `DontDestroyOnLoad`로 유지됩니다.

### 요소 (`UIElement`)

Button / Slider / Toggle / Carousel / InvenSlot을 전부 `UIElement` 하나에서 파생시켜, **상태와 입력 처리를 공통화**했습니다.

```csharp
[Flags]
public enum UIElementState
{
    Default = 1 << 0,
    Hover   = 1 << 1,
    Select  = 1 << 2,
    Disable = 1 << 3,
    Pressed = 1 << 4
}
```

- `[Flags]`라 "선택 + 호버" 같은 복합 상태를 표현합니다
- `WillStateChange` / `StateChanged` 이벤트로 연출(`UIEffector`)을 상태 로직에서 분리
- `isFrozen`으로 상태 변화를 일시적으로 잠금
- `isFocusSelect`로 "클릭해야 포커스" / "호버만으로 포커스"를 요소별로 전환

### 그룹 포커스 (`FocusParent`)

포커스를 개별 요소가 아니라 **그룹 단위**로 관리합니다.

```csharp
public enum NavigationMode { Horizontal, Vertical, Inventory }
```

`Inventory` 모드는 그리드 탐색용으로, 끝에 도달했을 때의 동작을 방향별로 지정할 수 있습니다.

```csharp
public struct TableNavigationData
{
    public int x, y;                                            // 그리드 크기
    public bool isLeftLoop, isRightLoop, isUpLoop, isDownLoop;  // 방향별 순환 여부

    // 순환하는 대신 호출할 외부 함수
    // (예: 장비창에서 아래로 누르면 인벤토리창으로 포커스를 넘김)
    public MoveEvent moveLeft, moveRight, moveUp, moveDown;
}
```

이 델리게이트 덕분에 **창과 창 사이 포커스 이동**을 UI 코드 수정 없이 인스펙터에서 연결할 수 있습니다.

### 창 간 내비게이션 (`UI_NavigationController`)

창 사이 이동 규칙을 리스트로 정의하고 내부에서 맵으로 변환합니다.

```csharp
public struct NavigationRule
{
    public MonoBehaviour origin;
    public NavigationDirection direction;
    public MonoBehaviour destination;
}
```

입력을 각 UI가 알아서 처리하는 대신 컨트롤러가 전체 흐름을 쥐고 있어서, 포커스가 어디로 갈지 한 곳에서 파악됩니다.

### 입력 장치 자동 전환

`GameManager`가 매 프레임 입력 장치를 감지해서, 패드 입력이 들어오면 **UI의 키 안내 이미지가 즉시 패드 아이콘으로 바뀌고** 커서가 잠깁니다.

```csharp
if (Gamepad.current != null && Gamepad.current.allControls.Any(x => x.IsPressed()))
{
    DataAccess.Settings.Data.LoadGamePadImages();
    // ... OnKeyChange 발화, 커서 숨김
}
```

키 리매핑은 `KeySettingManager`가 담당하며 설정은 영구 저장됩니다.

<!-- [촬영] 패드로 인벤토리 그리드를 탐색하고 장비창↔인벤토리로 포커스가 넘어가는 GIF -->

</details>

<br/>

<a id="sys-sound"></a>
<details>
<summary><b>▶ Sound</b> — 채널 분리, 위치 기반 SFX, 영역 기반 BGM</summary>

<br/>

### SoundManager

BGM / SFX / UI / Ambience / Master 5개 채널을 AudioMixer 그룹과 1:1로 연결하고, 볼륨·음소거를 채널별로 관리합니다.

### SFX

`SFXPlayer`는 AudioSource를 부착한 풀링 오브젝트입니다. 위치 기반 재생 후 자동 반환되므로, 전투 이펙트나 투사체에서 발생시킨 소리가 오브젝트 파괴와 무관하게 끝까지 재생됩니다.

### 영역 기반 BGM

BGM을 코드가 아니라 **맵 배치로** 제어합니다.

| 컴포넌트 | 동작 |
|---|---|
| `SceneMusicFadeArea` | 영역 진입 시 페이딩하며 BGM 전환 |
| `SetSceneMusicVolumeArea` | 영역 기반 볼륨 조절 |

### AudioSourceUtil

재생 규칙 자체를 공용화한 유틸리티입니다. 발소리처럼 매번 다른 클립이 필요하거나, Intro → Loop 구조가 필요한 BGM을 개별 구현 없이 처리합니다.

- Random 재생 / 순차 재생
- Intro → Loop 구조
- 반복 재생 제어
- 종료 / 루프 이벤트 분리

### 메모리

SFX는 라벨 단위로 전역 프리로드하고, BGM은 클립 하나가 수 MB이므로 씬 매니페스트에 등록해 Scene 스코프로 관리합니다. ([1번 항목](#1-애셋-수명-관리--개별-추적-대신-스코프-단위-해제) 참고)

</details>

<br/>

<a id="sys-save"></a>
<details>
<summary><b>▶ Save / Scene / Database</b> — 이중 스키마 세이브, 씬 로드 파이프라인, JSON 테이블</summary>

<br/>

### 세이브 — Persistent / Slot 이중 스키마

성격이 다른 두 데이터를 별도 파일·별도 스키마로 나눴습니다.

| 스키마 | 파일 | 내용 |
|---|---|---|
| `PersistentDataSchema` | `persistent.es3` | 설정, 전체 진행도 — 슬롯과 무관하게 유지 |
| `SlotDataSchema` | `slot_{id}.es3` | 슬롯별 플레이 데이터 |

`DataSchema`가 저장/로드/초기화를 추상화하고, 실제 데이터는 `ISaveData` 구현체가 `BeforeSave()` / `OnLoaded()` 훅으로 직렬화 전후 처리를 담당합니다.

```csharp
public abstract class DataSchema
{
    protected abstract ES3File SaveFile { get; }
    public void Save(string key)  { ... }
    public void SaveAll()         { ... }
    public void LoadAll()         { ... }
    public void ResetAll()        { ... }
}
```

**로드 순서에 의존성이 있는 데이터를 명시적으로 처리했습니다.** `TempSaveData`가 플레이어 생성 로직을 포함하므로 반드시 마지막에 등록됩니다.

**손상된 세이브 복구** — 로드 중 예외가 나면 데이터를 날리는 대신, 시스템 알림으로 사용자에게 알리고 초기화 후 재시작하는 경로를 제공합니다.

```csharp
catch (Exception e)
{
    SystemManager.SystemAlert("date load error!\nConfirm to data clear and restart the game.",
        () => { Initializer.DataClear(); /* 재시작 */ });
    throw;
}
```

접근은 `DataAccess.Settings` / `DataAccess.GameData` 정적 진입점으로 통일했습니다.

### 씬 로드 파이프라인

```
SceneLoad 요청
  → FadeManager 페이드 아웃
  → WhenSceneLoadBegin 발화 (잔여 일시정지 티켓 정리)
  → ResourceUtil.ReleaseSceneAssets()  (Scene 스코프 + 풀 해제)
  → LoadingScene
  → ResourceUtil.PreloadSceneAsync()   (다음 씬 애셋 프리로드 + 풀 prewarm)
  → 씬 활성화
  → WhenSceneLoaded 발화 (플레이어/UI 토글, 게임 상태 갱신)
  → 페이드 인
```

`SceneData`가 씬의 성격(`SceneType`, `isPlayerMustExist`)을 들고 있어서, 플레이어가 없는 씬에서는 자동으로 메인 UI를 끄고 `DefaultState`로 전환됩니다. 타이틀 복귀도 이 정보로 판정합니다 — Init/Loading은 경유 씬이라 무시하고, `Other → Title` 전이일 때만 `WhenReturnedToTitle`을 발화해서 각 매니저가 상태를 리셋합니다.

### 데이터베이스

기획 데이터는 JSON 테이블로 두고 타입별 클래스로 역직렬화합니다.

`Buff` / `SubBuffOption` / `BuffGroup` / `SubBuffType` / `Config` / `Monster` / `SkillTree` / `Level` + 문자열 테이블(`LanguageManager`, 다국어).

`FormulaConfig`로 방어력 공식 같은 밸런스 상수를 코드에서 분리했습니다.

</details>

<br/>

<a id="sys-stage"></a>
<details>
<summary><b>▶ Stage / 레벨 오브젝트</b> — 트리거 전략, 플랫폼, 상호작용</summary>

<br/>

### 트리거 — 조건과 결과의 분리

"언제 발동하는가"와 "무엇을 하는가"를 각각 인터페이스로 나눠서, 조합으로 새 트리거를 만듭니다.

| 축 | 인터페이스 | 구현 |
|---|---|---|
| 조건 | `ITriggerStrategy` | `TS_PlayerEnter`, `TS_PlayerExit` |
| 결과 | `ITriggerActivate` | `TA_SetGameObject`, `TA_SpawnComponent`, `TA_SceneMusic`, `TA_SceneMusicFading` |

### 레벨 오브젝트

| 분류 | 구현 |
|---|---|
| 플랫폼 | `MovingObj`, `RepeatMovingObj`, `WayPointPlatform`, `Elevator`, `ConveyorBelt`, `ClockWork`, `VerticalPlatforms` |
| 이동 | `SpringJump`, `SuperJump`, `FadePortal`, `Door`, `TriggerDoor` |
| 상호작용 | `Interaction`, `SingleUseInteraction`, `Lever`, `InteractionFadePortal` |
| 함정 | `Trap`, `FireProjectileTrap` |
| 파괴 | `DestroyableObject` + `IDestroyCheck` (파괴 조건 분리) |
| 드롭 | `DropItem`, `CollideItem`, `ItemPickUp` |

상호작용은 `IOnInteract`를 구현하면 되고, 상호작용 UI가 뜨는 동안 `InteractionState`가 티켓으로 켜져 조작이 제한됩니다.

</details>

<br/>

---

## 폴더 구조

```
Assets/
├─ BehaviourTree/          Behaviour Tree 시스템
│  ├─ Editor/              비주얼 에디터 (UI Toolkit / GraphView)
│  └─ Scripts/             런타임 노드 (Action / Decorator / Composite)
│
├─ Editor/                 에디터 확장 (애셋 생성기, 빈 폴더 정리 등)
│
└─ Scripts/
   ├─ Actor/               유닛 기본 클래스
   │  ├─ Player/           플레이어 (상태 머신 · 입력 버퍼 · 쿨다운)
   │  ├─ Monster/          몬스터 · 보스 (인식 · 패턴 · 상태)
   │  └─ Summons/          소환수
   ├─ Buff/                버프 / SubBuff 계층
   ├─ Skill/               스킬 (전략 · 데코레이터 · 스킬트리)
   ├─ SpawnObject/         공격 오브젝트 · 투사체 · 이펙트
   ├─ UI/                  UI 계층 · 요소 · 포커스 · 네비게이션
   ├─ Sound/               SFX 플레이어 · 영역 기반 BGM
   ├─ Save/                세이브 스키마 · 슬롯
   ├─ Stage/               레벨 오브젝트 · 트리거 · 드롭
   ├─ Item/                아이템 · 인벤토리
   ├─ Command/             입력 커맨드
   ├─ Datas/               밸런스 데이터
   ├─ DatabaseTypes/       JSON 테이블 타입
   ├─ Scenes/              초기화 · 로딩
   └─ Utils/
      ├─ Managers/         GameManager, UI, Sound, Scene, Save, Database …
      ├─ Resource/         AssetScope · AssetRegistry · 프리로드 · 풀링
      ├─ DesignPattern/    Singleton · ObjectPool · Factory · Observer · StateMachine
      ├─ Events/           이벤트 핸들러
      ├─ DataStructure/    PriorityQueue · SerializedDictionary 등
      └─ Interface/        공용 인터페이스
```

<br/>

---

<sub>이 저장소는 게임 완성품이 아니라 **구조를 검증하고 재사용하기 위한 템플릿**입니다.
`Practice/`, `_Recovery/`, `GameManager.Sample.cs`, `GameState/Sample/`은 예시·실험용 코드로, 템플릿 사용 시 삭제 가능합니다.</sub>
