# Code Style & Conventions

## Naming
- 클래스/인터페이스: PascalCase (Actor, IAttackType, BuffSystem)
- 인터페이스: I 접두사 (IOnHit, IAttackable, IDirection)
- private 필드: _camelCase (_attacker, _buffSystem)
- public 필드: camelCase 또는 PascalCase 혼용
- 이벤트/Action: OnXxx 패턴 (OnFire, OnSetTarget)

## Patterns
- Lazy initialization: `??=` 패턴 광범위 사용
- GetOrAddComponent 확장 메서드 사용
- partial class로 Actor 등 대형 클래스 분리
- OdinInspector: [LabelText], [TabGroup], [ShowIf], [HideInInspector] 적극 활용

## Language
- 주석/LabelText: 한국어
- 변수명: 영어
