# Prototype

**Unity 게임을 위한 베이스 템플릿 프로젝트.**

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
촬영 #1 · 대표 이미지 4컷
아래 #2 / #4 / #6 / #9 GIF에서 대표 프레임을 뽑아 쓰면 따로 찍을 필요가 없습니다.
URL을 채운 뒤 이 주석 기호를 지우세요.

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

- [핵심 시스템](#핵심-시스템)
  - [1. 리소스 레이어 — 애셋 로딩 · 수명 · 프리로드](#1-리소스-레이어--애셋-로딩--수명--프리로드)
  - [2. Behaviour Tree — AI를 노드로 조립하는 에디터](#2-behaviour-tree--ai를-노드로-조립하는-에디터)
  - [3. UI — 패드로 다룰 수 있는 UI 프레임워크](#3-ui--패드로-다룰-수-있는-ui-프레임워크)
  - [4. Actor — 모든 유닛의 기본 클래스](#4-actor--모든-유닛의-기본-클래스)
  - [5. 스킬 — 사용 방식과 스탯을 따로 갈아끼우는 구조](#5-스킬--사용-방식과-스탯을-따로-갈아끼우는-구조)
- [그 외 시스템](#그-외-시스템)
  - [AttackObject / Projectile](#sys-attackobject)
  - [버프 시스템](#sys-buff)
  - [게임 상태 관리](#sys-gamestate)
  - [Sound](#sys-sound)
  - [Save / Scene / Database](#sys-save)
  - [Stage / 레벨 오브젝트](#sys-stage)
- [폴더 구조](#폴더-구조)

<br/>

---

# 핵심 시스템

전체를 다 보실 필요는 없습니다. 아래 다섯 개가 **설계 판단이 가장 많이 들어간 부분**입니다.

<br/>

## 1. 리소스 레이어 — 애셋 로딩 · 수명 · 프리로드

> `Assets/Scripts/Utils/Resource/`

### 어떤 기능인가

**Addressables로 애셋을 올리고 내리는 일 전부를 담당하는 레이어입니다.** 게임 코드가 `Addressables` API를 직접 부르는 곳은 한 군데도 없고, 전부 이 레이어를 거칩니다.

호출부가 보는 건 `ResourceUtil` 하나입니다.

```csharp
// 로드 — 어느 스코프에 올릴지만 지정한다
var icon   = ResourceUtil.Load<Sprite>("BuffIcon_Poison");
var prefab = ResourceUtil.Load<GameObject>("Slime", AssetLifetime.Scene);

// 인스턴스 생성
var go = ResourceUtil.Instantiate("HitEffect");

// 씬 진입 전 프리로드 (로딩 화면에서 호출)
await ResourceUtil.PreloadSceneAsync("Stage1", progress);

// 씬 이탈 시 정리 — 이 한 줄이 해제의 전부
ResourceUtil.ReleaseSceneAssets();
```

| 구성 | 역할 |
|---|---|
| `ResourceUtil` | 유일한 진입점. 로드 / 생성 / 프리로드 / 정리 |
| `AssetScope` | 수명이 같은 애셋 묶음. Addressables 핸들의 **소유자** |
| `AssetRegistry` | 스코프 보관소 (`Global` / `Scene`) |
| `PreloadManifest` | 미리 올릴 애셋 목록 (ScriptableObject) |
| `SceneManifestTable` | 씬 이름 → 매니페스트 매핑 |
| `AddressablePooling` | 주소 단위 오브젝트 풀 |

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

**프리로드를 데이터로 분리** — 무엇을 미리 올릴지는 코드가 아니라 ScriptableObject가 들고 있습니다.

```csharp
[CreateAssetMenu(menuName = "Config/Preload Manifest")]
public class PreloadManifest : ScriptableObject
{
    public string[] labels;         // Addressables 라벨 단위로 통째로 로드
    public string[] addresses;      // 개별 주소 지정
    public PrewarmEntry[] prewarm;  // 풀에 미리 생성해 둘 오브젝트
}
```

`SceneManifestTable`이 씬 이름 → 매니페스트를 매핑하고, 로딩 화면이 씬 활성화 직전에 `PreloadSceneAsync(sceneName)`을 부릅니다. **오브젝트 풀 prewarm까지 이 단계에 넣어서**, 게임플레이 중에는 로드도 인스턴스 생성도 일어나지 않습니다.

### 이 구조에서 신경 쓴 지점

**① 프리로드 누락을 사람이 찾지 않게 했습니다.**
매니페스트에서 빠진 애셋은 런타임 동기 로드로 이어지는데, 이건 눈으로 못 찾습니다. `AssetScope`가 동기 로드된 주소를 기록해두고 `ResourceUtil.LogPreloadReport()`로 뽑습니다. 한 바퀴 플레이한 뒤 그 로그를 매니페스트에 그대로 옮기면 됩니다.

**② 프리팹 인스턴스가 핸들을 갖지 않게 했습니다.**
`Addressables.InstantiateAsync`는 인스턴스마다 핸들을 만들어 풀링과 충돌합니다. **프리팹은 스코프가 핸들 하나로 붙잡고, 인스턴스는 `Object.Instantiate`로** 만듭니다. 파기는 `Destroy` 하나로 끝납니다.

**③ 도메인 리로드를 꺼도 깨지지 않게 했습니다.**
도메인 리로드를 끄면 static이 살아남아 이전 플레이 세션의 죽은 핸들이 남습니다. `RuntimeInitializeOnLoadMethod(SubsystemRegistration)`으로 스코프를 다시 만듭니다.

**④ BGM은 라벨 통째로 올리지 않습니다.**
SFX는 짧아서 라벨 단위로 다 올리지만, BGM은 클립 하나가 수 MB라 전부 올리면 메모리를 크게 먹습니다. 씬에서 쓰는 것만 씬 매니페스트에 등록합니다.

### 검증 — 성능 이득이 실제로 있는지 실험했습니다

리팩토링 직전 커밋(`44ade73`)과 현재 버전으로 **같은 벤치마크 씬을 돌렸습니다.** 실제 게임 프로젝트에서 가져온 보스 이펙트 32종(31.8MB)을 스폰하면서 프레임 시간과 GC Alloc을 기록합니다.

리팩토링 전에는 **프리로드가 없었습니다.** 게임플레이 도중 처음 등장하는 이펙트를 그 자리에서 로드했고, 그때 프레임이 크게 튀었습니다. 프리로드는 이 리팩토링으로 들어온 것이므로 그 이득도 리팩토링의 몫입니다.

**첫 스폰 최악 프레임 256.5 ms → 91.3 ms (−64%)**

이 이득이 어디서 왔는지 분해하려고 대조군 두 행을 넣었습니다. 특히 2행은 **옛 구조에 프리로드만 억지로 붙인 것**입니다. 옛 코드에는 무엇을 언제 올릴지 정할 구조가 없어서, 주소를 직접 순회해 로드하는 방식으로 흉내 냈습니다.

| 조건 | 첫 스폰 최악 프레임 | GC | 640회 스폰 | GC |
|---|---:|---:|---:|---:|
| **리팩토링 전** — 프리로드 없음 (실제 상태) | 256.5 ms | 4.59 MB | 58.0 ms | 1.42 MB |
| 리팩토링 전 + 프리로드 (대조군) | 106.8 ms | 4.44 MB | 65.9 ms | 1.53 MB |
| 현재 − 프리로드 (대조군) | 255.0 ms | 4.65 MB | 62.1 ms | 1.13 MB |
| **현재** — 프리로드 사용 (실제 상태) | **91.3 ms** | 4.28 MB | 62.3 ms | 1.36 MB |

<sub>Unity 6000.3 에디터 · 프로파일러 활성 상태 기준.</sub>

**① 이득의 대부분은 프리로드입니다.** 256.5 → 106.8 ms. 로드 비용이 게임플레이에서 로딩 단계로 옮겨간 몫입니다.

**② 나머지는 스코프 캐시가 가져갑니다.** 106.8 → 91.3 ms. 옛 구조는 미리 올려둬도 스폰할 때 Addressables를 다시 거치지만, 지금은 캐시에 바로 적중합니다.

**③ 구조 변경 자체는 GC 할당에서 드러났습니다.** 640회 스폰 구간에서 1.42 → 1.13 MB, 1.53 → 1.36 MB로 **두 조건 모두 같은 방향으로 11~20% 줄었습니다.** 인스턴스마다 Addressables 핸들을 만들지 않게 한 결과이고, 시간이 아니라 할당량으로 나타났습니다.

<!-- ▼ 촬영 #8 · 프로파일러 before / after (스크린샷 2장)
     표의 1행과 4행(= 실제 상태 두 개)을 찍습니다. 워밍업 120프레임 직후,
     첫 소환이 일어나는 121프레임의 Hierarchy에서 "Bench.ColdSpawn" 행이 보이게 캡처.
       리팩토링 전(프리로드 없음)  약 256ms
       현재(프리로드 사용)         약  91ms
     주의: CPU Usage 그래프는 Y축이 자동 스케일이라 봉우리 높이가 둘 다 비슷해 보입니다.
     왼쪽 ms 눈금이 같이 들어가게 잡으세요.
     URL을 채운 뒤 이 주석 기호를 지우세요.

| 리팩토링 전 | 리팩토링 후 |
|:---:|:---:|
| ![before](URL) | ![after](URL) |
-->

### 재보고 나서야 안 것

재기 전에는 인스턴스당 핸들을 없앤 효과가 **반복 스폰 시간**에서 크게 나올 거라고 봤습니다. 그런데 풀링 때문에 두 번째 라운드부터는 인스턴스 생성이 아예 일어나지 않아서, 시간으로는 잴 수가 없는 구조였습니다. 같은 변경이 GC 할당에서는 일관되게 드러났고요. 재보지 않았으면 엉뚱한 것을 성과로 적었을 겁니다.

프리로드 없이 비교하면 첫 스폰 비용이 두 버전에서 똑같습니다(256.5 vs 255.0). Addressables 그룹이 `Pack Together`(단일 번들)이라, 어느 이펙트를 먼저 건드리든 그 순간 번들 전체가 올라오기 때문입니다. 같은 이유로 `ReleaseSceneAssets()`로 Scene 스코프를 버려도 **번들 참조가 남아 메모리가 반납되지 않습니다.**

수명을 코드로 나누는 것과, 번들을 물리적으로 나누는 것은 별개였습니다. 그룹 분할은 다음 작업입니다.

<br/>

## 2. Behaviour Tree — AI를 노드로 조립하는 에디터

> `Assets/BehaviourTree/`

### 어떤 기능인가

**몬스터와 보스의 AI를 코드가 아니라 노드 그래프로 만드는 시스템입니다.** 런타임과 **비주얼 에디터를 직접 제작**했습니다.

<img width="1919" height="1000" alt="Behaviour Tree 에디터" src="https://github.com/user-attachments/assets/673cba93-0ca7-491f-b5b7-8c17376fd8d8" />

<br/><br/>

사용법은 두 단계입니다.

1. 에디터에서 노드를 놓고 연결해 트리 애셋(ScriptableObject)을 만든다
2. 몬스터 프리팹에 `BehaviourTreeRunner`를 붙이고 그 트리를 지정한다

```csharp
public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    public StartType startType;    // OnStart / ByScript
    public UpdateType updateType;  // Normal / Fixed
    public bool Repeat;
}
```

노드는 세 종류뿐이고, AI 로직은 전부 이 조합으로 표현됩니다.

| 종류 | 역할 | 구현 예 |
|---|---|---|
| `ActionNode` | 실제 행동 | `MoveNode`, `DashToPos`, `JumpNode`, `SetAnimation`, `TeleportToPos`, `BossAtk` |
| `DecoratorNode` | 조건 · 흐름 제어 | `HpCheck`, `IfPlayerDistance`, `CoolDownCheck`, `RaycastCheck`, `RepeatNode`, `CheckPhase` |
| `CompositeNode` | 자식 노드 조합 | `SequenceNode`, `SelectNode`, `ExcuteAll`, `ProbSelect`(확률 선택) |

### 문제

보스 패턴을 상태 머신으로 짜면 두 가지가 걸렸습니다.

- **상태 전이 폭발** — 패턴이 늘어날 때마다 상태가 늘고, 전이 조건은 상태 수의 제곱으로 늘어납니다. "체력 50% 아래에서 거리가 멀면 돌진, 가까우면 광역"처럼 조건이 겹치기 시작하면 코드에서 흐름이 안 보입니다.
- **기획 이터레이션이 프로그래머를 거침** — 패턴 순서나 확률을 바꾸는 사소한 조정에도 코드 수정·컴파일이 필요해서, 밸런싱 속도가 코드 수정 속도에 묶였습니다.

### 접근

**행동의 흐름을 트리 구조로 옮기고, 그 트리를 데이터(ScriptableObject)로 만들었습니다.** 조건은 데코레이터로, 순서·선택은 컴포지트로 표현되므로 흐름이 그래프 모양 그대로 읽힙니다.

노드 하나는 세 개의 훅만 구현하면 됩니다.

```csharp
public abstract class TreeNode : ScriptableObject
{
    public enum State { Running, Failure, Success, Null }

    public abstract void  OnStart();
    public abstract void  OnStop();
    public abstract State OnUpdate();   // Running을 반환하는 동안 계속 유지된다
}
```

베이스가 `Update()`에서 시작/종료 시점을 관리하므로, 구현체는 "이번 프레임에 무엇을 할지"만 쓰면 됩니다.

```csharp
public virtual State Update()
{
    if (!isStarted) { OnStart(); isStarted = true; }

    state = OnUpdate();
    if (state is State.Failure or State.Success) { OnStop(); isStarted = false; }

    return state;
}
```

### 이 구조에서 신경 쓴 지점

**① 새 노드는 상속만 하면 에디터 메뉴에 자동으로 뜹니다.**
노드를 추가할 때 에디터 코드를 건드리지 않도록, `TypeCache`로 파생 타입을 훑어 컨텍스트 메뉴를 구성합니다.

```csharp
var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
// ...
evt.menu.AppendAction($"Action/{type.Name}/{t.Name}", _ => CreateNode(t, nodePosition));
```

베이스가 `CommonActionNode` / `BossActionNode` / `PlayerActionNode`로 나뉘어 있어서 메뉴도 그 계층대로 묶여 나옵니다.

**② 새 노드 스크립트를 에디터에서 바로 만듭니다.**
그래프 우클릭 메뉴의 `Create New Script`를 누르면 템플릿을 기반으로 `.cs` 파일이 생성되고, `Create New Type`은 새 카테고리 폴더와 추상 베이스까지 함께 만듭니다.

```csharp
var template = File.ReadAllText("Assets/BehaviourTree/Templates/ActionNodeTemplate.txt");
template = template.Replace("#Name#", scriptName).Replace("#Name2#", classOriginalName);
File.WriteAllText(scriptPath, template);
AssetDatabase.Refresh();
```

생성된 파일은 `OnStart` / `OnStop` / `OnUpdate` 뼈대만 있는 상태라, 바로 내용만 채우면 됩니다. 노드를 하나 늘리는 데 드는 작업이 "파일 만들고 → 폴더 정하고 → 베이스 상속하고"에서 **버튼 하나**로 줄었습니다.

**③ 트리 애셋 원본이 런타임에 오염되지 않게 했습니다.**
노드가 `ScriptableObject`이므로 여러 몬스터가 같은 트리를 참조하면 실행 상태(`isStarted`, `state`)를 공유해 버립니다. 그래서 인스턴스마다 트리를 통째로 복제합니다.

```csharp
tree = tree.Clone();          // SO 원본 보호
tree.Init(actor, Repeat);
```

**④ 물리 기반 AI를 위해 갱신 시점을 선택할 수 있게 했습니다.**
돌진·점프처럼 `Rigidbody2D`를 다루는 노드는 `Update`에서 돌면 물리와 어긋납니다. `UpdateType`으로 `Update` / `FixedUpdate` 중 하나에서 트리가 돌도록 했습니다.

**⑤ 실행 중인 노드를 추적합니다.**
`BlackBoard`가 노드 간 공유 상태와 현재 실행 노드를 들고 있어서, 에디터에서 플레이 중 어느 노드가 도는지 볼 수 있습니다. AI가 의도대로 안 움직일 때 디버깅 시간이 가장 많이 줄어든 부분입니다.

<!-- ▼ 촬영 #2 · Behaviour Tree 에디터 조작 (GIF, 8~12초)
     담을 것: 노드를 드래그해 배치 → 포트 연결 → 플레이 진입 → 실행 중인 노드가 하이라이트되는 흐름
     URL을 채운 뒤 이 주석 기호를 지우세요.

![Behaviour Tree 에디터 조작](URL)
-->

<br/>

## 3. UI — 패드로 다룰 수 있는 UI 프레임워크

> `Assets/Scripts/UI/`, `Assets/Scripts/Utils/Managers/UIManager.cs`

### 어떤 기능인가

**UI를 여닫는 계층 관리부터, 버튼·슬라이더 같은 공용 요소, 키보드/패드 포커스 이동까지를 묶은 프레임워크입니다.** 프로젝트에서 가장 큰 부분(약 10,000줄)입니다.

네 층으로 나뉘어 있습니다.

| 층 | 담당 | 대표 클래스 |
|---|---|---|
| 계층 | UI를 열고 닫고, 정렬 순서를 관리 | `UIManager`, `UI_Base` |
| 요소 | 버튼·슬라이더 등 개별 컨트롤의 상태와 입력 | `UIElement` |
| 그룹 포커스 | 한 창 안에서의 포커스 이동 | `FocusParent` |
| 창 간 이동 | 창과 창 사이의 포커스 이동 | `UI_NavigationController` |

UI는 역할별로 타입이 나뉘고, `UIManager`가 타입마다 다른 생명주기와 정렬 순서를 적용합니다.

| 타입 | 역할 | 관리 방식 |
|---|---|---|
| `UI_Main` | 항상 유지 (HUD) | 리스트, 활성화와 무관 |
| `UI_Scene` | 씬 단위 고정 창 | 단일 |
| `UI_Popup` | 팝업 | 스택 (겹칠수록 order 증가) |
| `UI_Ingame` | 월드 좌표 추종 (몬스터 HP바 등) | 리스트 |
| `UI_Hover` | 마우스 추종 (드래그 아이템 등) | 최상위 order |

UI 프리팹은 `AddressablePooling`으로 풀링되고, 루트(`@UI_Root`)는 `DontDestroyOnLoad`로 유지됩니다.

<!-- ▼ 촬영 #3 · UI 계층 (스크린샷 1장)
     담을 것: HUD(UI_Main) + 팝업 2개가 겹친 상태(UI_Popup 스택) + 몬스터 머리 위 HP바(UI_Ingame)가
     한 화면에 동시에 보이는 장면. 타입별로 order가 나뉘는 것이 눈으로 보이면 됩니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![UI 계층](URL)
-->

### 문제

Unity 기본 UI는 **마우스를 전제로 만들어져 있습니다.** 게임패드를 지원하려 하니 기본 제공되는 것으로는 부족했습니다.

- **`Selectable`의 자동 내비게이션은 인접 요소만 봅니다.** 인벤토리 그리드 끝 칸에서 반대편으로 순환하거나, 장비창 맨 아래에서 인벤토리창으로 넘어가는 처리를 표현할 수 없었습니다.
- **포커스가 개별 요소 단위입니다.** 실제로 필요한 건 "지금 포커스가 장비창 그룹에 있다"는 그룹 단위 개념인데, 없으니 창 단위 처리를 매번 손으로 짜야 했습니다.
- **요소마다 상태 처리가 제각각이었습니다.** hover / select / pressed / disable을 버튼과 슬라이더가 각자 구현하면서 연출과 동작이 서로 달라졌습니다.

### 접근

**포커스를 "요소"가 아니라 "그룹"의 문제로 다시 정의하고, 입력을 각 UI가 아니라 컨트롤러가 쥐도록 뒤집었습니다.**

**요소 — 상태를 하나로 통일**
Button / Slider / Toggle / Carousel / InvenSlot을 전부 `UIElement` 하나에서 파생시켰습니다.

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

`[Flags]`라 "선택 + 호버" 같은 복합 상태를 그대로 표현합니다. 연출은 `WillStateChange` / `StateChanged` 이벤트로 분리해서(`UIEffector`), 상태 로직과 애니메이션이 섞이지 않게 했습니다.

**그룹 포커스 — 경계에서의 동작을 데이터로**
`FocusParent`가 자식 요소들의 포커스를 그룹 단위로 관리합니다. 탐색 방식은 `Horizontal / Vertical / Inventory` 셋이고, 그리드용인 `Inventory` 모드는 **끝에 도달했을 때의 동작을 방향별로 지정**할 수 있습니다.

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

이 델리게이트 덕분에 **창과 창 사이 포커스 이동을 UI 코드 수정 없이 인스펙터에서 연결**할 수 있습니다.

**창 간 이동 — 규칙을 한 곳에 모음**
`UI_NavigationController`가 `출발 UI · 방향 · 도착 UI` 규칙을 리스트로 받아 내부에서 맵으로 변환합니다. 입력을 각 UI가 알아서 처리하는 대신 컨트롤러가 전체 흐름을 쥐고 있어서, **포커스가 지금 어디에 있고 다음에 어디로 갈지가 한 곳에서 파악**됩니다.

### 이 구조에서 신경 쓴 지점

**① 입력 장치가 바뀌면 UI가 즉시 따라갑니다.**
`GameManager`가 매 프레임 입력 장치를 감지해서, 패드가 눌리면 **UI의 키 안내 이미지가 그 자리에서 패드 아이콘으로 바뀌고** 커서가 잠깁니다. 키 리매핑(`KeySettingManager`) 결과도 같은 경로로 반영되고 영구 저장됩니다.

**② 포커스 방식을 요소별로 고를 수 있게 했습니다.**
마우스만 올려도 포커스가 가야 하는 UI가 있고, 클릭해야만 가야 하는 UI가 있습니다. `isFocusSelect` 한 값으로 요소·그룹 단위 전환이 됩니다.

**③ 상태 변화를 잠글 수 있게 했습니다.**
연출 중이거나 확인 대기 중일 때 상태가 바뀌면 안 되므로, `isFrozen`으로 모든 상태 변화를 막습니다.

<!-- ▼ 촬영 #4 · UI 포커스 네비게이션 (GIF, 8~12초)
     담을 것: 패드로 인벤토리 그리드 이동 → 끝 칸에서 순환 → 장비창으로 포커스가 넘어가는 순간
     중간에 키보드를 한 번 눌러 키 안내 아이콘이 즉시 바뀌는 것까지 담으면 좋습니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![UI 포커스 네비게이션](URL)
-->

<br/>

## 4. Actor — 모든 유닛의 기본 클래스

> `Assets/Scripts/Actor/`

### 어떤 기능인가

**플레이어 · 몬스터 · 보스 · 소환수가 전부 상속하는 유닛 베이스 클래스입니다.** 체력, 스탯, 방향, 이동, 피격, 사망처럼 "유닛이라면 다 갖는 것"을 담고 있습니다.

```
Actor  (추상)
├─ Player      상태 머신 · 입력 커맨드 · 스킬
├─ Monster     인식 · 패턴 · AI
│  ├─ CommonMonster   경량 상태 머신
│  └─ BossMonster     Behaviour Tree · 페이즈
└─ Summon      소환수
```

여기에 버프·스킬·아이템·AI가 전부 붙어서 동작합니다. 즉 **이 클래스가 다른 모든 시스템이 만나는 지점**이고, 그래서 설계가 가장 조심스러웠던 부분입니다.

### Actor는 기능을 갖지 않고, 능력 인터페이스를 조합한다

`Actor`가 "체력도 있고 스탯도 있고 이동도 하는 만능 클래스"인 것이 아닙니다. **능력 하나하나를 인터페이스로 쪼개 두고, Actor는 그중 유닛에 공통인 것만 골라 구현**합니다.

| 인터페이스 | "이 대상은 …" | 구현 파일 |
|---|---|---|
| `IOnHit` | 맞을 수 있다 (체력 · 무적 · 사망) | `Actor.cs` |
| `IOnHitReaction` | 맞았을 때 반응한다 (넉백) | `Actor.cs` |
| `IAttackable` | 공격할 수 있다 | `Actor.cs` |
| `IDirection` | 바라보는 방향이 있다 | `Actor.cs` |
| `IAnimator` | 애니메이션을 가진다 | `Actor.cs` |
| `IEventUser` | 이벤트를 주고받는다 | `Actor.Event.cs` |
| `IStatUser` | 스탯을 가진다 | `Actor.Stat.cs` |
| `IBarrierUser` | 배리어를 가진다 | `Actor.Stat.cs` |
| `IImmunity` | 면역을 가진다 | `Actor.Immunity.cs` |

**`partial` 파일을 나눈 기준이 곧 인터페이스입니다.** 어떤 능력이 어디 있는지 찾을 때 선언부만 보면 됩니다.

파생 클래스는 여기에 자기 능력만 더 붙입니다.

```csharp
public partial class Player  : IDashUser, IMovable, IPlayer
public partial class Monster : Actor, IRecognition, IMovable, IPoolObject
```

이동(`IMovable`)이 `Actor`가 아니라 `Player` · `Monster`에 있는 이유는 **움직이지 않는 유닛도 있기 때문**입니다. `Summon`은 맞고 죽을 수는 있어도 스스로 이동하지 않아서 `IMovable`을 붙이지 않습니다.

### 문제

두 가지가 걸렸습니다.

- **상속으로 기능을 더하면 조합이 폭발합니다.** "독 데미지를 주는 몬스터"와 "폭발하는 몬스터"와 "독 데미지를 주면서 폭발하는 몬스터"가 각각 클래스가 됩니다.
- **다른 시스템이 `Actor`를 직접 알면 유닛이 아닌 대상을 다룰 수 없습니다.** 부술 수 있는 상자나 벽은 유닛이 아닌데도 맞고 부서져야 합니다. 공격 시스템이 `Actor`를 받도록 짜면 이런 대상을 위해 코드를 또 쓰거나, 상자를 억지로 `Actor`로 만들어야 합니다.

### 접근 ① — 시스템은 `Actor`가 아니라 능력에 의존한다

공격 시스템은 대상이 무엇인지 모릅니다. **`IOnHit`, 즉 "맞을 수 있는 것"만 압니다.**

```csharp
public interface IAttackStrategy
{
    float Calculate(IOnHit target);   // Actor가 아니라 IOnHit
}
```

덕분에 `Actor`를 상속하지 않는 오브젝트도 같은 전투 파이프라인에 들어옵니다.

```csharp
public class DestroyableObject : MonoBehaviour, IOnHit   // 유닛이 아닌 파괴 가능 오브젝트
```

상자를 때리는 코드와 몬스터를 때리는 코드가 같습니다. 새로운 "맞을 수 있는 것"을 추가할 때 전투 코드는 건드리지 않습니다.

인터페이스 쪽에는 **기본 구현(default interface implementation)** 을 넣어서, 구현체가 컴포넌트만 물려주면 공통 동작을 그대로 얻도록 했습니다.

```csharp
public interface IMovable : IMonoBehaviour
{
    public UnitMoveComponent MoveComponent { get; }    // 이것만 구현하면
    public void MoveOn() => MoveComponent?.MoveOn();   // 아래는 전부 따라온다
    public void JumpOn() => MoveComponent?.JumpOn();
    public void Stop()   => MoveComponent?.Stop();
}
```

이동 로직은 `UnitMoveComponent` 한 곳에만 있고, `IMovable`을 붙인 쪽은 중복 구현 없이 그것을 씁니다.

### 접근 ② — 기능은 이벤트로 붙인다

`Actor`를 **기능 구현체가 아니라 조합 컨테이너**로 두고, 개별 기능은 이벤트 구독으로 붙입니다.

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

임시 스탯 보정은 `BonusStatEvent`에 델리게이트를 붙였다가 `finally`에서 반드시 떼어내는 방식으로, 예외가 나도 보정이 남지 않게 했습니다.

<!-- ▼ 촬영 #5 · 전투 이벤트 흐름 (GIF, 5~8초)
     담을 것: 일반 타격 → 크리티컬 → 백어택 순으로 때려서 데미지 텍스트가 각각 다르게 뜨는 장면.
     피격 측 반응(넉백/무적 깜빡임)까지 보이면 좋습니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![전투 이벤트 흐름](URL)
-->

### 정리 — 세 겹으로 나눠져 있다

| 겹 | 무엇 | 예 |
|---|---|---|
| **능력** | 인터페이스. "무엇을 할 수 있는가"의 계약 | `IOnHit`, `IMovable`, `IStatUser` |
| **구현** | `partial` 파일 하나가 인터페이스 하나를 구현 | `Actor.Stat.cs` → `IStatUser`, `IBarrierUser` |
| **실제 로직** | 구현 파일은 대부분 위임만 하고, 로직은 별도 클래스에 | `StatManager`, `BarrierCalculator`, `ImmunityController`, `ActorEvents`, `ActorCombat`, `EffectSpawner` |

`Actor.cs`는 컨테이너·생명주기·방향·체력만 들고 있고 나머지는 전부 위임입니다. 새 능력이 생기면 **인터페이스 하나 + `partial` 파일 하나 + 로직 클래스 하나**를 더하면 되고, 기존 `Actor.cs`는 그대로 둡니다.

렌더링도 같은 방식입니다. `IActorRenderer`로 추상화해서 일반 스프라이트(`ActorNormalRenderer`)와 Spine을 교체할 수 있고, `Actor`는 어느 쪽인지 모릅니다.

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

<br/>

## 5. 스킬 — 사용 방식과 스탯을 따로 갈아끼우는 구조

> `Assets/Scripts/Skill/`

### 어떤 기능인가

**플레이어와 몬스터가 쓰는 스킬, 그리고 스킬을 성장시키는 스킬트리까지를 다룹니다.**

| 구분 | 설명 |
|---|---|
| `ActiveSkill` | 직접 발동. 쿨다운·자원 소모·사용 방식을 가짐 |
| `PassiveSkill` | 상시 적용. 장착 시 효과가 걸리고 해제 시 되돌아감 |
| `SkillTree` | 레벨을 올려 스킬 성능·효과를 확장 |

스킬 하나는 세 조각의 조합입니다.

```
스킬 = 스탯(SkillStat)  +  발동 로직(Active)  +  사용 방식(ISkillActive)
        └ 데미지·쿨타임      └ 무엇이 일어나는가    └ 어떻게 발동되는가
```

세 조각을 **각각 독립적으로** 바꿀 수 있게 한 것이 이 시스템의 전부입니다.

### 문제

스킬 수가 늘면서 두 가지가 걸렸습니다.

- **사용 방식이 클래스로 굳었습니다.** "즉발 파이어볼"과 "차지 파이어볼"은 발동 결과가 같은데도 별도 클래스가 됐습니다. 차징 중 취소, 캐스팅 중 피격 같은 처리가 스킬마다 중복 구현됐습니다.
- **강화 수치를 더할 자리가 없었습니다.** 스킬 레벨·룬·장비 효과가 겹칠 때 원본 스탯을 직접 수정하면 해제할 때 되돌릴 수가 없고, 적용 순서에 따라 결과가 달라졌습니다.

### 접근

**① 사용 방식을 전략으로 분리**

스킬을 "발동한다"가 아니라 **"어떻게 발동되는가"를 교체 가능한 축**으로 뒀습니다. `ISkillActive` 구현체만 바꾸면 같은 스킬이 즉발형에서 차지형이 됩니다.

```csharp
public interface ISkillActive
{
    bool  durationUse { get; }     // 지속시간 사용 여부
    bool  CheckUsable { get; }     // 사용 가능 여부
    void  Activate(ActiveSkill skill);
    void  DeActivate(ActiveSkill skill);
    float CalculateDmg(float dmg);
    void  OnCancel();              // 캔슬 처리를 한 곳에 모음
    void  OnUnEquip(ActiveSkill skill);
}
```

| 구현체 | 동작 |
|---|---|
| `InstantSkill` | 즉발 |
| `ChargeSkill` | 누르는 동안 차징 → 뗄 때 발동 (`OnChargeEnd` / `OnChargeCancel`) |
| `CastingSkill` | 캐스팅 시간 후 발동 (`OnCastingEnd` / `OnCastingCancel`) |
| `ToggleSkill` | 켜고 끄기 |
| `ContinuousSkill` | 누르는 동안 지속 |

`CalculateDmg`가 인터페이스에 있는 이유는, 차지형처럼 **사용 방식 자체가 데미지에 관여**하는 경우가 있기 때문입니다. 차징 정도에 따른 배율을 스킬 로직이 아니라 사용 방식이 계산합니다.

차징 완료·취소 같은 순간은 Actor 이벤트(`OnChargeEnd`, `OnCastingCancel` 등)로 발화되므로, UI 게이지나 이펙트가 스킬을 직접 참조하지 않고 붙습니다.

**② 스탯 합성을 데코레이터로**

원본을 수정하는 대신 감쌉니다.

```csharp
public class SkillDecorator : ISkill
{
    private readonly ISkill config;      // 원본
    private readonly ISkill attachment;  // 덧붙는 효과

    public SkillStat Stat => config.Stat + attachment.Stat;
}
```

중첩이 가능하므로 강화가 몇 겹이든 같은 방식으로 처리되고, **해제는 감싼 것을 벗기기만 하면 됩니다.** 적용 순서에 따라 결과가 달라지는 문제도 사라집니다.

### 스킬트리 (Visitor)

`SkillTree`는 `SerializedScriptableObject`이고 `ISkillVisitor`를 구현합니다. 액티브/패시브에 각각 다른 처리를 하도록 오버로드로 분기시켰습니다.

```csharp
public virtual void Activate(PlayerActiveSkill active, int level)   { ... }
public virtual void Activate(PlayerPassiveSkill passive, int level) { ... }
```

스킬 쪽은 `Accept(visitor, level)`만 호출하므로, **트리 노드가 늘어도 스킬 클래스는 그대로**입니다. 이름·설명은 `LanguageManager`의 문자열 테이블에서 가져와 다국어를 지원합니다.

<!-- ▼ 촬영 #6 · 스킬 사용 방식 5종 (GIF, 10~15초)
     담을 것: 즉발 → 차지(게이지 차오름) → 캐스팅(캐스팅 바) → 토글(켜고 끄기) → 지속 순으로 하나씩 발동.
     차징 도중 한 번 취소해서 OnChargeCancel이 도는 장면까지 넣으면 설명이 확실해집니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![스킬 사용 방식 5종](URL)
-->

<!-- ▼ 촬영 #7 · 스킬트리 UI (스크린샷 1장)
     담을 것: 스킬트리 창에서 노드 몇 개를 찍어 레벨이 오른 상태. 설명 텍스트가 보이면 좋습니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![스킬트리](URL)
-->

<br/>

---

# 그 외 시스템

<br/>

<a id="sys-attackobject"></a>
<details>
<summary><b>▶ AttackObject / Projectile</b> — 데미지 계산 · 판정 방식 · 투사체 확장의 3축 분리</summary>

<br/>

근접 판정, 장판, 투사체 등 **"때리는 것" 전부를 담당하는 오브젝트**입니다. **"얼마를 때리는가" / "어떻게 판정하는가" / "어떻게 날아가는가"** 세 축으로 나눠서, 각각 독립적으로 교체할 수 있게 했습니다.

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

<!-- ▼ 촬영 #9 · 투사체 확장 조합 (GIF, 8~12초)
     담을 것: 유도 → 벽 반사 → 적 관통(관통마다 크기 변화) → 파괴 시 방사체 생성 순으로.
     같은 프리팹에서 설정만 바꿔 다르게 날아간다는 게 보이면 가장 좋습니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![투사체 확장 조합](URL)
-->

<!-- ▼ 촬영 #10 · 판정 방식 차이 (GIF, 5~8초)
     담을 것: Tick 장판 위에 서 있을 때 주기적으로 데미지가 들어가는 장면과,
     Once 판정 공격이 같은 대상을 한 번만 때리는 장면 비교.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![판정 방식](URL)
-->

</details>

<br/>

<a id="sys-buff"></a>
<details>
<summary><b>▶ 버프 시스템</b> — Buff / SubBuff 계층, 해제 전략, 계산 분리</summary>

<br/>

**유닛에 붙는 모든 지속 효과를 다루는 시스템입니다.** 스탯 증감, 상태이상(독·화상), 군중제어(기절·속박), 배리어, 지속 피해가 전부 여기를 통합니다.

유닛마다 `BuffSystem` 컴포넌트가 붙고, 부여 경로는 둘입니다.

```csharp
// ① 타입만 지정해 바로 부여
target.BuffSystem.AddSubBuff(caster, SubBuffType.어떤_상태이상);

// ② 데이터로 만든 상위 효과(Buff)가 SubBuff를 생성해 부여
var buff = new Buff(buffData, caster);
target.BuffSystem.AddSubBuff(caster, buff, subBuff);
```

부여·갱신·해제·스택·면역·UI 갱신을 시스템이 맡기 때문에, 새 효과를 만들 때는 **효과 내용만** 작성하면 됩니다.

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
user.BarrierCalculator.BarrierMinusEvent += MinusBarrier;
```

<!-- ▼ 촬영 #11 · 버프 / 디버프 동작 (GIF, 8~12초)
     담을 것: 적을 때려 독을 부여(Buff가 SubBuff를 생성) → 도트 데미지가 주기적으로 들어감 →
     HUD 버프 아이콘에 스택과 남은 시간이 표시됨 → 해제되며 아이콘이 사라짐.
     배리어가 데미지를 먼저 깎는 장면을 함께 넣으면 계산 분리를 보여줄 수 있습니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![버프 디버프 동작](URL)
-->

</details>

<br/>

<a id="sys-gamestate"></a>
<details>
<summary><b>▶ 게임 상태 관리</b> — 티켓 기반 상태 전환과 일시정지</summary>

<br/>

**"지금 플레이어가 움직일 수 있는가, UI만 조작되는가, 아무것도 안 되는가"를 한 곳에서 관리합니다.** 팝업·컷신·페이드·상호작용마다 조작 범위가 달라지는 것을 `GameManager`가 상태 단위로 모아서 처리합니다.

```csharp
public abstract class GameState
{
    public abstract int Priority { get; }   // 낮을수록 우선

    public abstract void OnEnterState();
    public abstract void OnExitState();

    public virtual void KeyBoardControlling() { ... }
    public virtual void GamePadControlling()  { ... }
}
```

| 상태 | 조작 범위 | 언제 |
|---|---|---|
| `DefaultState` | UI만 | 페이드 중, 시스템 알림, 플레이어 없는 씬 |
| `InteractionState` | UI + 기본 조작 | 상호작용 UI가 떠 있을 때 |
| `PlayState` | 전체 | 일반 플레이 (기본값) |

매 프레임 활성 상태 하나의 `~Controlling()`만 호출되므로 **입력 분기가 상태 클래스 안에만 존재**합니다.

### 플래그 대신 티켓

조작 가능 여부를 bool로 관리하면 **여러 주체가 동시에 같은 상태를 요구할 때** 무너집니다. 컷신이 조작을 막고 그 위에 팝업이 또 막았는데 팝업이 먼저 닫히면서 플래그를 풀어버리면, 컷신 중인데 플레이어가 움직입니다.

그래서 상태를 켠 사람마다 **Guid 티켓**을 발급하고, 티켓이 하나도 남지 않아야 실제로 해제되도록 했습니다.

```csharp
var guid = GameManager.instance.TryOnGameState<InteractionState>();
// ...
GameManager.instance.TryOffGameState<InteractionState>(guid);
```

```csharp
/// <summary>
///     상태별로 "이 상태를 켜 둔 사람들"의 티켓. 개수가 0보다 크면 켜진 것이다.
///     on/off 플래그를 따로 두지 않는 이유: 장부가 둘이면 서로 어긋날 수 있기 때문.
/// </summary>
private readonly Dictionary<GameState, HashSet<Guid>> _stateGuids = new();
```

동시에 여러 상태가 켜져 있을 수 있고, 그중 `Priority`가 가장 작은 것이 활성 상태가 됩니다. 상태 전환 함수는 `private`이라 **외부에서 직접 바꿀 수 없고 오직 티켓을 통해서만** 바뀝니다.

```csharp
public static class StatePriority
{
    public const int Default     = 0;
    public const int Interaction = 10;
    public const int Play        = 100;  // 가장 낮으므로 기본 상태가 된다
}
```

### 티켓 보유자가 사라지는 경우

씬이 전환되면 티켓을 들고 있던 UI가 통째로 사라져서, 아무도 해제할 수 없는 티켓이 영구히 남습니다.

```csharp
// 씬이 바뀌면 일시정지 guid 보유자(UI 등)가 통째로 사라지므로 남은 일시정지를 먼저 푼다.
// WhenSceneLoaded가 아니라 Begin에 거는 이유: 새 씬의 UI가 등록한 일시정지까지 지우면 안 되기 때문.
Scene.WhenSceneLoadBegin.AddListener(_ => ClearAllPauses());
```

일시정지(`RegisterPause` / `RemovePause`)도 같은 티켓 방식이며, 정리 시점을 `WhenSceneLoaded`가 아니라 `WhenSceneLoadBegin`으로 잡은 것이 핵심입니다.

### 템플릿과 게임 코드의 분리

게임별 코드가 템플릿을 오염시키지 않도록 `partial void` 훅을 썼습니다.

```csharp
// GameManager.cs (템플릿)
partial void OnSampleAwake();   // 선언만 — 구현이 없으면 호출은 컴파일 단계에서 사라진다
```

```csharp
// GameManager.Sample.cs (게임별 — 지워도 컴파일된다)
partial void OnSampleAwake()
{
    RegisterState(new BattleState());  // 게임 고유 상태 등록
}
```

<!-- ▼ 촬영 #12 · 상태에 따른 조작 제한 (GIF, 5~8초)
     담을 것: 플레이 중 이동 → 팝업 열림(조작 막힘, UI만 반응) → 그 위에 알림 팝업 하나 더 → 
     알림만 닫으면 여전히 조작이 막혀 있고, 팝업을 전부 닫아야 이동이 돌아오는 순서.
     티켓이 하나라도 남으면 해제되지 않는다는 게 눈으로 보이는 장면입니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![상태에 따른 조작 제한](URL)
-->

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

<!-- ▼ 촬영 #13 · 영역 기반 BGM 전환 (소리 있는 영상, 10~15초)
     담을 것: 씬 뷰에서 영역 콜라이더가 보이는 상태로 플레이어가 경계를 넘어갈 때
     BGM이 크로스페이드되는 구간. 이건 소리가 있어야 전달되므로 GIF보다 mp4가 낫습니다.
     GitHub는 이슈/코멘트에 mp4를 끌어다 놓으면 재생 가능한 링크를 만들어 줍니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

https://github.com/user-attachments/assets/URL
-->

### AudioSourceUtil

재생 규칙 자체를 공용화한 유틸리티입니다. 발소리처럼 매번 다른 클립이 필요하거나, Intro → Loop 구조가 필요한 BGM을 개별 구현 없이 처리합니다.

- Random 재생 / 순차 재생
- Intro → Loop 구조
- 반복 재생 제어
- 종료 / 루프 이벤트 분리

### 메모리

SFX는 라벨 단위로 전역 프리로드하고, BGM은 클립 하나가 수 MB이므로 씬 매니페스트에 등록해 Scene 스코프로 관리합니다. ([1번 항목](#1-리소스-레이어--애셋-로딩--수명--프리로드) 참고)

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

<!-- ▼ 촬영 #14 · 씬 로드 파이프라인 (GIF, 8~12초)
     담을 것: 포털 진입 → 페이드 아웃 → 로딩 화면(진행률 바가 프리로드 진행에 따라 차오름) →
     새 씬 페이드 인. 위 파이프라인 다이어그램과 나란히 놓이면 이해가 빨라집니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![씬 로드 파이프라인](URL)
-->

<!-- ▼ 촬영 #15 · 세이브 슬롯 UI (스크린샷 1장)
     담을 것: 슬롯 선택 화면에 저장된 슬롯과 빈 슬롯이 같이 보이는 장면.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![세이브 슬롯](URL)
-->

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

<!-- ▼ 촬영 #16 · 레벨 오브젝트 (GIF, 8~12초)
     담을 것: 움직이는 발판 → 스프링 점프 → 레버를 당겨 문이 열림 → 함정 작동 순으로 이어지는 짧은 구간.
     한 스테이지를 쭉 달리면서 여러 오브젝트를 지나가게 찍으면 한 번에 담깁니다.
     URL을 채운 뒤 이 주석 기호를 지우세요.

![레벨 오브젝트](URL)
-->

</details>

<br/>

---

## 폴더 구조

```
Assets/
├─ BehaviourTree/          Behaviour Tree 시스템
│  ├─ Editor/              비주얼 에디터 (UI Toolkit / GraphView)
│  ├─ Scripts/             런타임 노드 (Action / Decorator / Composite)
│  └─ Templates/           노드 스크립트 생성용 템플릿
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


<!--
════════════════════════════════════════════════════════════════
촬영 체크리스트 (작성자용 메모 — 페이지에는 보이지 않습니다)

  #    자리                    형식        내용                                     우선순위
  ──────────────────────────────────────────────────────────────
  1    최상단 히어로           이미지 4    #2/#4/#6/#9에서 대표 프레임 추출         높음
  2    Behaviour Tree          GIF         노드 배치·연결 + 실행 노드 하이라이트     높음
  3    UI 어떤기능인가         스크린샷    HUD + 팝업 스택 + 월드 HP바 동시 표시     보통
  4    UI 마지막               GIF         패드 그리드 탐색 + 창 간 포커스 이동      높음
  5    Actor 전투 흐름         GIF         일반 → 크리 → 백어택 데미지 차이          보통
  6    스킬 마지막             GIF         즉발/차지/캐스팅/토글/지속 + 차징 취소    높음
  7    스킬 마지막             스크린샷    스킬트리 창                               낮음
  8    리소스 마지막           스크린샷 2  프로파일러 before / after (44ade73 비교)  ★최우선
  9    AttackObject            GIF         유도 → 반사 → 관통 → 방사                 보통
  10   AttackObject            GIF         Tick 장판 vs Once 판정 비교               낮음
  11   버프 마지막             GIF         독 부여 → 도트뎀 → 아이콘 스택 → 해제     보통
  12   게임 상태 마지막        GIF         팝업 2겹 → 하나만 닫아도 조작 막힘        보통
  13   Sound 영역 BGM          영상(mp4)   영역 경계 통과 시 BGM 크로스페이드        보통
  14   Save/Scene              GIF         페이드 → 로딩바 → 새 씬 진입              보통
  15   Save/Scene              스크린샷    세이브 슬롯 선택 화면                     낮음
  16   Stage 마지막            GIF         발판 → 스프링 → 레버·문 → 함정            낮음

업로드 방법
  GitHub 이슈 작성창(또는 PR 코멘트창)에 파일을 끌어다 놓으면
  https://github.com/user-attachments/assets/... 링크가 생성됩니다.
  그 링크를 각 자리의 URL에 넣고 주석 기호를 지우면 됩니다. 이슈는 저장하지 않아도 됩니다.

권장 사양
  GIF  : 가로 800px 내외, 15fps, 10MB 이하 (GitHub 렌더링 한계 고려)
  영상 : mp4, 10MB 이하. 소리가 필요한 #13에만 사용
  캡처 : ScreenToGif, ShareX 등
════════════════════════════════════════════════════════════════
-->
