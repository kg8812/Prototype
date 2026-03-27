# Prototype Project Overview

## Purpose
Unity 2D 액션 게임 프로토타입. 플레이어, 몬스터(일반/보스), 스킬/버프 시스템, 투사체/공격 오브젝트, 스테이지 등으로 구성.

## Tech Stack
- Unity (2D, C#)
- DOTween (애니메이션/시퀀스)
- Sirenix OdinInspector (에디터 커스터마이징)
- Spine (스켈레탈 애니메이션)
- ScriptableObject 기반 데이터 관리

## Project Structure (Assets/Scripts/)
- Actor/ : Player, Monster(Common/Boss), ActorRenderer 등 캐릭터 관련
- Buff/ : BuffSystem, SubBuffManager, 버프 효과들
- SpawnObject/ : AttackObject(투사체/공격 이펙트), SpawnObject 기반 오브젝트들
- Skill/ : 스킬 시스템
- Stage/ : 스테이지, 트랩, 오브젝트
- Item/ : 아이템 시스템
- UI/ : UI 컴포넌트
- Utils/ : GameManager, Factory, 유틸리티

## Key Architecture Patterns
- 오브젝트 풀링: FactoryManager / Factory<T>
- 이벤트 시스템: IEventUser, BuffEvent, EventParameters
- 전략 패턴: IAttackStrategy, IAttackType
- ScriptableObject: ProjectileInfo 등 데이터 분리
- partial class: Actor가 여러 .cs 파일로 분리됨 (Actor.cs, Actor.Buff.cs 등)
- Namespace: Apis (공격/투사체), Default (기본 인터페이스들)
