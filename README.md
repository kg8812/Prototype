# Prototype

---

## 개요

이 프로젝트는 Unity 기반 게임 개발에서 반복적으로 사용되는 기능들을
공용 프레임워크 형태로 정리한 프로젝트입니다.

단순히 기능을 구현하는 것을 넘어,
전투, 버프, 스킬 등의 시스템을 재사용 가능한 구조로 설계하는 데 초점을 두었습니다.

<br/><br/>

## 개발 목표

프로젝트에 종속되지 않는 공용 구조 설계
시스템 간 결합도 최소화
유지보수와 확장이 용이한 코드 구조 구축
기능 중심 개발에서 구조 중심 설계로 전환

<br/><br/>

## 핵심 구조

+ ### Actor
<br/>
Actor는 유닛의 기본 클래스로 기능을 직접 구현하는 객체가 아니라,
여러 시스템을 조합하는 중심 컨테이너 역할을 합니다.

전투, 버프, 이벤트, 스킬 등의 시스템을 연결하고
기능 추가가 아닌 구성 추가 방식으로 확장하였으며

또한 Partial Class를 사용하여
하나의 클래스에 집중된 책임을 분리했습니다.
<br/><br/><br/>

+ ### 이벤트 기반 구조
<br/>
시스템 간 직접 참조를 줄이기 위해
이벤트를 중심으로 동작 흐름을 구성했습니다.

actor.AddEvent(EventType.OnHit, OnHitHandler);

이 구조를 통해:

시스템 간 결합도 감소
기능 추가 시 기존 코드 수정 최소화
전투 / 버프 / 스킬 / AI 간 유연한 연결 가능
<br/><br/><br/>

* ### 전투 흐름 구조
<br/>
전투는 하나의 로직에서 처리되지 않고,
여러 이벤트 단계로 나뉘어 처리됩니다.

OnAttackSuccess
OnBasicAttack
OnCrit
OnBackAttack
OnBeforeHit
OnHit
OnAfterHit
OnDeath

각 단계에서 기능을 확장할 수 있도록 설계되어 있습니다.
<br/><br/><br/>

* ### 버프 구조
<br/>
버프 시스템은 단순 리스트가 아닌
**계층 구조(Buff / SubBuff)**로 구성되어 있습니다.

Buff: 상위 효과
SubBuff: 실제 버프

특징:

하나의 Buff에 여러 버프 조합 가능
타입 기반 제거 및 면역 처리 지원
순회 기반 처리 구조

예시 : 적을 공격시 독을 부여합니다.
적을 공격하면 독을 부여하는 효과는 Buff에 들어가고 독 디버프는 subBuff로 분리되어 Buff로 부터 생성되어 부여되고 관리됩니다.
<br/><br/><br/>

* ### 계산 로직 분리
<br/>
버프와 연계된 계산(예: 배리어)은
별도의 계산기로 분리되어 있습니다.

BarrierCalculator를 사용하는 예시코드)
user.BarrierCalculator.BarrierAddEvent += AddBarrier;
user.BarrierCalculator.BarrierMinusEvent += MinusBarrier;

계산 로직과 상태 변경을 분리하여 구조를 단순화했습니다.
<br/><br/><br/>

* ### 스킬 구조
<br/>
스킬은 단순 실행이 아니라
사용 방식 자체를 확장할 수 있는 구조로 설계되었습니다.

ActiveSkill / PassiveSkill 분리
전략패턴 / 데코레이터 패턴 구조 적용

구현된 사용방식 : 

Instant (즉발)
Charge (차지)
Casting (캐스팅)
Continuous (지속)
Toggle (토글)
<br/><br/><br/>

* ### Behaviour Tree Tool (AI)
<br/>
Behaviour Tree 기반으로 AI를 비개발자도 UI로 조작할 수 있도록 툴을 만들었습니다.
개발쪽은 Action, Decorator, Composite 세가지를 기반으로 새로운 노드를 쉽게 만들 수 있게 설계되었습니다.

예시 이미지

<br/>

<img width="1919" height="1000" alt="Image" src="https://github.com/user-attachments/assets/673cba93-0ca7-491f-b5b7-8c17376fd8d8" />

<br/><br/>

복잡한 AI를 구조적으로 비교적 쉽게 관리 및 확장 가능
<br/><br/><br/>

* ### AttackObject / Projectile
<br/>
공격 오브젝트 시스템은 공격 계산 방식, 충돌 처리 방식, 생성 방식, 투사체 확장 기능을 분리한 구조로 설계되어 있습니다.

AttackObject는 공격 오브젝트의 기본 베이스 역할을 하며, IAttackStrategy를 통해 데미지 계산을 분리하고, 
IAttackType을 통해 Normal, Once, Tick, Delay, Cd, OnlyFirst 같은 공격 방식을 교체할 수 있습니다. 

또한 AttackObjectFactory를 통해 풀링 기반으로 생성됩니다.

<br/>

Projectile은 AttackObject를 상속하는 투사체 전용 구조로, 중력, 가속도, 최대 이동거리, 초기 속도, 방향 회전, 속도 0 처리 등을 설정할 수 있습니다.

벽, 바닥, 타겟, 보스에 대해 각각 다른 충돌 타입을 적용할 수 있으며, 파괴 / 반사 / 관통 / 정지 같은 동작을 지원합니다. 

또한 유도, 방사, 분리같은 확장 기능을 컴포넌트로 조합할 수 있도록 설계되어 있습니다.
<br/><br/><br/>

* ### UI 구조
<br/>
UI 시스템은 단순히 추가기능을 구성하는 수준이 아니라,
UI 계층, 공용 UI 요소, 입력 내비게이션, 포커스 관리를 분리하여
재사용 가능한 구조로 유니티의 기존 UI 기능을 재설계했습니다.
<br/>

#### UI 계층 구조

UI는 UI_Base를 중심으로 공통 동작을 정의하고,
역할에 따라 다음과 같이 분리했습니다.

UI_Main : 항상 유지되는 UI (HUD 등)
UI_Scene : 기본 UI창
UI_Popup : 팝업창
UI_Ingame, UI_Hover : 월드/마우스 기반 UI

UI를 단순한 화면 단위가 아니라
역할 기준으로 나눠서 설계했습니다.

<br/>

#### UI 요소 구조

기본 UI 요소는 Unity 기본 컴포넌트를 직접 사용하는 대신,
UIElement를 기반으로 재구성했습니다.

상태 관리 (Default / Hover / Select / Pressed / Disable)
공통 입력 처리

이 구조를 바탕으로 Button, Slider, Inventory Slot 등의
UI Asset을 공통 형태로 재사용할 수 있도록 구성했습니다.

UI마다 입력과 상태를 따로 구현하지 않도록
공통 구조로 통합하는 것을 목표로 했습니다.

<br/>

#### 입력 및 포커스 구조

UI 입력은 UI_NavigationController를 중심으로
전체 흐름을 관리하도록 설계했습니다.

UI 간 이동 및 선택 흐름 제어
키보드 / 패드 기반 탐색 지원

또한 포커스는 FocusParent 단위로 관리하여,
UI를 개별 요소가 아닌 그룹 단위로 제어할 수 있도록 구성했습니다.

그룹 단위 포커스 이동
Grid 기반 (Inventory) 탐색 지원

입력을 각 UI에 맡기지 않고
전체 UI 흐름에서 제어하도록 설계했습니다.

<br/>

#### 복합 UI 구조

InventoryContent와 같은 구조를 통해
슬롯 기반 UI를 데이터와 함께 관리하도록 구성했습니다.

단순한 표시용 UI가 아니라
재사용 가능한 UI 모듈 형태로 확장할 수 있도록 설계했습니다.
<br/><br/><br/>

* ### Sound 구조
<br/>
사운드 시스템은 단순 재생 기능이 아니라,
게임 전반에서 재사용 가능한 사운드 관리 구조로 설계되었습니다.

#### SoundManager (핵심)

사운드 시스템의 중심은 SoundManager입니다.

역할:

BGM / SFX 재생 관리
사운드 채널 분리
위치 기반 사운드 처리
전체 사운드 흐름 제어

모든 사운드 재생은 SoundManager를 통해 이루어지는 구조입니다.

##### 효과음 구조 (SFX)

효과음은 단순 재생 호출이 아니라
AudioSource를 부착시킨 사운드 오브젝트 기반으로 관리됩니다.

SFXPlayer
풀링 가능한 사운드 오브젝트
위치 기반 사운드 재생
자동 재생 / 반환 처리

전투 이펙트, 투사체 등과 자연스럽게 연동 가능

<br/>

##### BGM 및 씬 기반 제어

BGM은 단순 재생이 아니라
씬과 공간에 따라 동적으로 제어됩니다.

SceneMusicFadeArea
특정 영역 진입 시 페이딩하며 BGM 전환
SetSceneMusicVolumeArea
영역 기반 볼륨 조절

사운드를 코드가 아닌
환경(맵) 기준으로 제어

<br/>

#### AudioSourceUtil

AudioSourceUtil은 사운드 재생 방식을 공용화한 유틸리티입니다.

지원 기능:

Random 재생
순차 재생
Intro → Loop 구조
반복 재생 제어
종료 / 루프 이벤트 분리

사운드 재생 로직을 개별 구현하지 않고
재생 규칙을 공통화화여 전투, UI, 스킬 등 다양한 영역에서 재사용 가능합니다.

<br/><br/><br/>

* ### 공용 유틸리티
<br/>
각종 매니저 / 인터페이스 / 확장 기능등 프로젝트 전반에서 반복되는 기능들을 공용 유틸리티로 분리했습니다.

<br/>

--- 

<br/><br/><br/>

## 설계 방향

이 프로젝트는 단순한 기능 구현이 아니라,

Actor 내부 책임 분리
시스템 단위 구조 분리
데이터 흐름 명확화
전투 / 버프 / 스킬 구조 재설계
공용 코드로의 재구성 등

“재사용 가능한 구조를 만드는 과정”

에 목적이 있습니다.

게임 개발에서 반복되는 문제를 해결하기 위해
기능이 아닌 구조를 정리하고,
이를 공용 프레임워크로 확장하는 것을 목표로 합니다.
