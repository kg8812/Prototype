using System;
using System.Collections.Generic;
using Apis;
using DG.Tweening;
using Default;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public interface IWeaponAttack
    {
        public UnityEvent<int> GroundAttacks { get; }
        public UnityEvent<int> AirAttacks { get; }

        public UnityEvent OnAfterAtk { get; }
        public int AttackType { get; }
    }

    public class Weapon_BasicAttack : IWeaponAttack // 기본 공격 (특수 능력 X)
    {
        public UnityEvent<int> GroundAttacks => attacks;
        public UnityEvent<int> AirAttacks => airAttacks;
        private UnityEvent _onAfterAtk;
        public UnityEvent OnAfterAtk => _onAfterAtk ??= new();

        readonly UnityEvent<int> attacks = new();
        readonly UnityEvent<int> airAttacks = new();
        protected Player player => weapon?.Player;
        protected Weapon weapon;

        public virtual int AttackType => 1;

        public Weapon_BasicAttack(Weapon weapon)
        {
            attacks.RemoveAllListeners();
            airAttacks.RemoveAllListeners();
            attacks.AddListener(GroundAttack);
            airAttacks.AddListener(AirAttack);
            this.weapon = weapon;
        }

        public virtual void GroundAttack(int index)
        {
        }

        public virtual void AirAttack(int index)
        {
        }
    }

    public class SuryongeumAtk : Weapon_BasicAttack // 수룡음 공격
    {
        [System.Serializable]
        public struct MapleTreeInfo
        {
            [LabelText("데미지")] public float dmg;
            [LabelText("지속시간")] public float duration;
            [LabelText("나무 크기")] public Vector2 size;
            [LabelText("그로기 계수")] public float groggy;
        }

        private MapleTreeInfo info;

        public SuryongeumAtk(Weapon weapon, MapleTreeInfo info) : base(weapon)
        {
            this.info = info;
        }

        public override void GroundAttack(int index)
        {
            base.GroundAttack(index);

            switch (index)
            {
                case 2:
                    AttackObject obj = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "MapleTree",
                        player.transform.position + Vector3.right * ((int)player.Direction * Mathf.Abs(info.size.x)));
                    info.size.x *= (int)player.Direction;
                    obj.transform.localScale = info.size;

                    obj.Init(player, new AtkItemCalculation(player, weapon,info.dmg), info.duration);
                    obj.Init(weapon.CalculateGroggy(info.groggy));
                    GameObject explosion = GameManager.Factory.Get(FactoryManager.FactoryType.Effect, "MapleExplosion",
                        obj.transform.position);
                    GameManager.Factory.Return(explosion, 1);

                    break;
            }
        }

        public override void AirAttack(int index)
        {
            base.AirAttack(index);
            switch (index)
            {
                case 2:
                    AttackObject obj = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "MapleTree",
                        player.transform.position + Vector3.right * (int)player.Direction * Mathf.Abs(info.size.x));
                    info.size.x *= (int)player.Direction;
                    obj.transform.localScale = info.size;

                    obj.Init(player, new AtkBase(player, info.dmg), info.duration);
                    GameObject explosion = GameManager.Factory.Get(FactoryManager.FactoryType.Effect, "MapleExplosion",
                        obj.transform.position);
                    obj.Init(weapon.CalculateGroggy(info.groggy));
                    GameManager.Factory.Return(explosion, 1);

                    break;
            }
        }
    }

    public class RushAtk : Weapon_BasicAttack
    {
        public override int AttackType => 2;

        [System.Serializable]
        public struct RushInfo
        {
            [LabelText("투척거리")] public float distance;
            [LabelText("돌진시간")] public float rushTime;
            [LabelText("돌진Ease")] public Ease rushEase;
        }

        private RushInfo info;

        public RushAtk(Weapon weapon, RushInfo info) : base(weapon)
        {
            this.info = info;
        }

        public override void GroundAttack(int index)
        {
            base.GroundAttack(index);
            player.animator.ResetTrigger("RushEnd");

            switch (index)
            {
                case 1:

                    Vector2 pos = player.Mecanim.transform.InverseTransformPoint(player.Position +
                        Vector3.right * ((int)player.Direction * info.distance));
                    player.WeaponBone.SetLocalPosition(pos);
                    
                    break;
                case 2:
                    player.TurnOffBoneFollower();
                    Tween tween = player.Rb
                        .DOMoveX(player.Position.x + (int)player.Direction * info.distance, info.rushTime)
                        .SetUpdate(UpdateType.Fixed).SetEase(info.rushEase).SetAutoKill(true);
                    tween.onKill += () =>
                    {
                        player.animator.SetTrigger("RushEnd");
                    };
                    break;
            }
        }
    }

    public class BeamAttack : IWeaponAttack
    {
        public int AttackType => 1;

        public UnityEvent<int> GroundAttacks => attacks;
        public UnityEvent<int> AirAttacks => airAttacks;

        readonly UnityEvent<int> attacks = new();
        readonly UnityEvent<int> airAttacks = new();
        private UnityEvent _onAfterAtk;
        public UnityEvent OnAfterAtk => _onAfterAtk ??= new();

        protected Player player => weapon?.Player;
        private readonly ProjectileWeapon weapon;

        public enum BeamType
        {
            LeadVocalBeam,
        }

        private readonly string projectileName;
        readonly BeamEffect.BeamInfo beamInfo;
        
        public BeamAttack(ProjectileWeapon weapon, BeamType type, BeamEffect.BeamInfo beamInfo)
        {
            attacks.RemoveAllListeners();
            airAttacks.RemoveAllListeners();

            this.beamInfo = beamInfo;
            
            projectileName = type switch
            {
                BeamType.LeadVocalBeam => "LeadVocalBeam",
                _ => ""
            };

            attacks.AddListener(GroundAttack);
            airAttacks.AddListener(AirAttack);

            this.weapon = weapon;
        }

        void FireProjectile(int idx)
        {
            for (int i = 0; i < weapon.groundProjectileInfos[idx].projCount; i++)
            {
                BeamEffect proj = GameManager.Factory.Get<BeamEffect>(FactoryManager.FactoryType.AttackObject,
                    projectileName, weapon.FirePos);

                proj.Init(player, new AtkItemCalculation(player, weapon,weapon.groundProjectileInfos[idx].info.dmg), beamInfo.fireTime);
                proj.Init(weapon.groundProjectileInfos[idx].info);
                proj.Init(beamInfo);
                proj.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.groundProjectileInfos[idx].groggy / 100));
                proj.Fire();
                weapon.OnAtkObjectInit.Invoke(new EventParameters(proj));
            }
        }

        void FireAirProjectile(int idx)
        {
            for (int i = 0; i < weapon.airProjectileInfos[idx].projCount; i++)
            {
                BeamEffect proj = GameManager.Factory.Get<BeamEffect>(FactoryManager.FactoryType.AttackObject,
                    projectileName, weapon.FirePos);

                proj.Init(player, new AtkItemCalculation(player, weapon,weapon.airProjectileInfos[idx].info.dmg));
                proj.Init(weapon.airProjectileInfos[idx].info);
                proj.Init(beamInfo);
                proj.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.airProjectileInfos[idx].groggy / 100));

                proj.Fire();
                weapon.OnAtkObjectInit.Invoke(new EventParameters(proj));
            }
        }

        protected void GroundAttack(int idx)
        {
            FireProjectile(idx);
        }

        protected void AirAttack(int idx)
        {
            FireAirProjectile(idx);
        }
    }

    public class ProjectileAttack : IWeaponAttack // 투사체 공격
    {
        public virtual int AttackType => 1;

        public UnityEvent<int> GroundAttacks => attacks;
        public UnityEvent<int> AirAttacks => airAttacks;

        readonly UnityEvent<int> attacks = new();
        readonly UnityEvent<int> airAttacks = new();
        private UnityEvent _onAfterAtk;
        public UnityEvent OnAfterAtk => _onAfterAtk ??= new();

        protected Player player => weapon?.Player;
        protected readonly ProjectileWeapon weapon;

        protected Action<Projectile,int,int> OnGroundFire;
        protected Action<Projectile,int,int> OnAirFire;
protected Action<Projectile,int,int> BeforeGroundFire;
        protected Action<Projectile,int,int> BeforeAirFire;

        public enum ProjectileType
        {
            Bullet,
        }

        private string projectileName;

        public struct FiredInfo
        {
            public Sequence seq;
            public List<Projectile> projectiles;
        }
        
        public ProjectileAttack(ProjectileWeapon weapon)
        {
            attacks.RemoveAllListeners();
            airAttacks.RemoveAllListeners();

            attacks.AddListener(GroundAttack);
            airAttacks.AddListener(AirAttack);

            this.weapon = weapon;
        }

        enum GroundOrAir
        {
            Ground,Air
        }

        protected string GetProjectileName(ProjectileType projectileType)
        {
            return projectileType switch
            {
                ProjectileType.Bullet => "bullet",
                _ => ""
            };
        }
        Projectile CreateProjectile(int idx, ProjectileType projectileType)
        {
            projectileName = GetProjectileName(projectileType);
            
            return GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.AttackObject,
                projectileName, GetProjPos(idx));
        }

        protected virtual Vector2 GetProjPos(int idx)
        {
            return weapon.FirePos;
        }
        FiredInfo FireProjectile(List<ProjectileWeapon.ProjInfo> infos,int idx,GroundOrAir groundOrAir)
        {
            List<Projectile> list = new();
            Sequence seq = DOTween.Sequence();
            Guid guid = Guid.NewGuid();
            for (int i = 0; i < infos[idx].projCount; i++)
            {
                int temp = i;
                
                Projectile proj = CreateProjectile(i, infos[idx].projType);
                proj.gameObject.SetActive(false);
                seq.AppendCallback(() =>
                {
                    switch (groundOrAir)
                    {
                        case GroundOrAir.Ground:
                            BeforeGroundFire?.Invoke(proj,idx,temp);
                            break;
                        case GroundOrAir.Air:
                            BeforeAirFire?.Invoke(proj,idx,temp);
                            break;
                    }
                    proj.gameObject.SetActive(true);
                    proj.Init(player, new AtkItemCalculation(player,weapon,infos[idx].info.dmg));
                    proj.Init(infos[idx].info);
                    proj.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * infos[idx].groggy / 100));
                    proj.firedAtkGuid = guid;
                    proj.Fire();
                    switch (groundOrAir)
                    {
                        case GroundOrAir.Ground:
                            OnGroundFire?.Invoke(proj,idx,temp);
                            break;
                        case GroundOrAir.Air:
                            OnAirFire?.Invoke(proj,idx,temp);
                            break;
                    }
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(proj));
                });
                seq.AppendInterval(infos[idx].fireTerm);
                list.Add(proj);
            }
            return new()
            {
                seq = seq,
                projectiles = list
            };
        }
        protected virtual FiredInfo FireGroundProjectile(int idx)
        {
            return FireProjectile(weapon.groundProjectileInfos, idx,GroundOrAir.Ground);
        }

        protected virtual FiredInfo FireAirProjectile(int idx)
        {
            return FireProjectile(weapon.airProjectileInfos, idx,GroundOrAir.Air);
        }

        protected void GroundAttack(int idx)
        {
            FireGroundProjectile(idx);
        }

        protected void AirAttack(int idx)
        {
            FireAirProjectile(idx);
        }
    }
    public class MagicAtk : ProjectileAttack // 마법 공격 (마법 투사체)
    {
        public MagicAtk(ProjectileWeapon weapon) : base(weapon)
        {
        }

        protected override Vector2 GetProjPos(int idx)
        {
            return player.Position + player.topPivot + Vector3.up * 0.5f * idx;
        }
    }
    public class OrbAtk : IWeaponAttack // 터뜨리기 공격
    {
        public UnityEvent<int> GroundAttacks => attacks;
        public UnityEvent<int> AirAttacks => airAttacks;
        private UnityEvent _onAfterAtk;
        public UnityEvent OnAfterAtk => _onAfterAtk ??= new();

        public int AttackType => 1;
        
        readonly UnityEvent<int> attacks = new();
        readonly UnityEvent<int> airAttacks = new();

        protected readonly Weapon weapon;

        public struct OrbAtkInfo
        {
            [LabelText("폭발 횟수")] public int count;
            [LabelText("폭발 반경")] public float radius;
            [LabelText("폭발 시간간격")] public float timePadding;
            [LabelText("공격 판정범위")] public Vector2 size;
            [LabelText("공격 사정거리")] public float distance;
            [LabelText("폭발 공격설정")] public ProjectileInfo orbInfo;
        }

        private List<OrbAtkInfo> groundOrbInfos;
        private List<OrbAtkInfo> airOrbInfos;
        protected Player player => weapon?.Player;
        public OrbAtk(Weapon weapon, List<OrbAtkInfo> groundOrbInfos,List<OrbAtkInfo> airOrbInfos)
        {
            this.weapon = weapon;
            attacks.RemoveAllListeners();
            airAttacks.RemoveAllListeners();

            attacks.AddListener(GroundAtk);
            airAttacks.AddListener(AirAtk);
            this.groundOrbInfos = groundOrbInfos;
            this.airOrbInfos = airOrbInfos;
        }

        protected virtual void SpawnExplosion(Weapon.atkInfo dmgInfo, OrbAtkInfo atkInfo,Vector2 position)
        {
            Sequence seq = DOTween.Sequence();
            
            for (int i = 0; i < atkInfo.count; i++)
            {
                seq.AppendCallback(() =>
                {
                    AttackObject explosion = GameManager.Factory.Get<AttackObject>(
                        FactoryManager.FactoryType.Effect,
                        Define.DummyEffects.Explosion,
                        position);
                    explosion.Init(player, new AtkItemCalculation(player,weapon, dmgInfo.dmg),1);
                    explosion.Init(atkInfo.orbInfo);
                    explosion.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * dmgInfo.groggy / 100));
                    explosion.transform.localScale = Vector3.one * (2 * atkInfo.radius);
                });
                seq.AppendInterval(atkInfo.timePadding);
            }
        }
        protected virtual void GroundAtk(int idx)
        {
            if (groundOrbInfos.Count <= idx) return;
            OrbAtkInfo info = groundOrbInfos[idx];
            var ray = Physics2D.BoxCast(player.Position, info.size, 0, Vector2.right * player.DirectionScale,info.distance,
                LayerMasks.Enemy | LayerMasks.Wall);

            Vector2 position;
            if (ray.collider == null)
            {
                position = player.Position + Vector3.right * (player.DirectionScale * info.distance);
            }
            else
            {
                position = ray.collider.TryGetComponent(out IMonoBehaviour target)
                    ? target.Position
                    : ray.collider.transform.position;
            }
            
            SpawnExplosion(weapon.groundAtkDmgs[idx], info, position);
        }

        protected virtual void AirAtk(int idx)
        {
            if (airOrbInfos.Count <= idx) return;
            OrbAtkInfo info = airOrbInfos[idx];
            var ray = Physics2D.BoxCast(player.Position, info.size, 0, Vector2.right * player.DirectionScale,info.distance,
                LayerMasks.Enemy | LayerMasks.Wall);

            Vector2 position;
            if (ray.collider == null)
            {
                position = player.Position + Vector3.right * (player.DirectionScale * info.distance);
            }
            else
            {
                position = ray.collider.TryGetComponent(out IMonoBehaviour target)
                    ? target.Position
                    : ray.collider.transform.position;
            }
            
            SpawnExplosion(weapon.airAtkDmgs[idx], info, position);
        }
    }

    public class MagicStemAtk : IWeaponAttack // 마법줄기 공격
    {
        public int AttackType => 1;

        public UnityEvent<int> GroundAttacks => attacks;
        public UnityEvent<int> AirAttacks => airAttacks;
        private UnityEvent _onAfterAtk;
        public virtual UnityEvent OnAfterAtk => _onAfterAtk ??= new();

        readonly UnityEvent<int> attacks = new();
        readonly UnityEvent<int> airAttacks = new();

        protected readonly Weapon weapon;
        protected Player player => weapon?.Player;

        protected readonly Vector2 size;
        protected readonly float duration;
        protected readonly string stemName;

        public enum StemType
        {
            Magic,
            Whip
        }

        public MagicStemAtk(Weapon weapon, Vector2 size, float duration, StemType type)
        {
            this.size = size;
            this.weapon = weapon;
            this.duration = duration;
            attacks.RemoveAllListeners();
            airAttacks.RemoveAllListeners();

            attacks.AddListener(GroundAtk);
            airAttacks.AddListener(AirAtk);

            switch (type)
            {
                case StemType.Magic:
                    stemName = "MagicStem";
                    break;
                case StemType.Whip:
                    stemName = "WhipStem";
                    break;
            }
        }

        protected virtual void GroundAtk(int idx)
        {
            AttackObject atk = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                stemName, player.Position + Vector3.right * (size.x / 2 + 0.5f) * (int)player.Direction);
            atk.transform.localScale = size;
            atk.Init(player, new AtkItemCalculation(player,weapon, weapon.groundAtkDmgs[idx].dmg),
                duration);
            atk.Init(player.atkInfo);
            atk.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.groundAtkDmgs[idx].groggy / 100f));
            atk.AdditionalAtkCount = weapon.groundAtkDmgs[idx].atkCount - 1;
            weapon.OnAtkObjectInit.Invoke(new EventParameters(atk));

        }

        protected virtual void AirAtk(int idx)
        {
            AttackObject atk = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                stemName, player.Position + Vector3.right * (size.x / 2 + 0.5f) * (int)player.Direction);
            atk.transform.localScale = size;
            atk.Init(player, new AtkItemCalculation(player,weapon, weapon.airAtkDmgs[idx].dmg),
                duration);
            atk.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.airAtkDmgs[idx].groggy / 100f));
            atk.AdditionalAtkCount = weapon.airAtkDmgs[idx].atkCount - 1;
            weapon.OnAtkObjectInit.Invoke(new EventParameters(atk));
        }
    }

    public class GoatSeguAtk : IWeaponAttack // 음표공격
    {
        public int AttackType => 1;

        public UnityEvent<int> GroundAttacks => attacks;
        public UnityEvent<int> AirAttacks => airAttacks;

        public UnityEvent OnAfterAtk
        {
            get
            {
                if (onAfterAtk == null)
                {
                    onAfterAtk = new();
                    onAfterAtk.AddListener(Remove);
                }

                return onAfterAtk;
            }
        }

        private UnityEvent onAfterAtk;
        readonly UnityEvent<int> attacks = new();
        readonly UnityEvent<int> airAttacks = new();

        private readonly Weapon weapon;
        private Player player => weapon?.Player;

        readonly Vector2 size;
        private readonly Vector2 expSize;

        private AttackObject atk;

        public GoatSeguAtk(Weapon weapon, Vector2 size, Vector2 expSize)
        {
            this.size = size;
            this.expSize = expSize;
            this.weapon = weapon;
            attacks.RemoveAllListeners();
            airAttacks.RemoveAllListeners();

            attacks.AddListener(Atk);
            airAttacks.AddListener(AirAtk);
        }

        void Atk(int idx)
        {
            switch (idx)
            {
                case 0:
                    atk = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "GoatSeguStaff", player.Position + Vector3.right * size.x / 2 * (int)player.Direction);
                    atk.transform.localScale = size;
                    atk.Init(player, new AtkItemCalculation(player, weapon, weapon.groundAtkDmgs[idx].dmg),2);
                    atk.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.groundAtkDmgs[idx].groggy / 100));

                    atk.AdditionalAtkCount = weapon.groundAtkDmgs[idx].atkCount - 1;
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(atk));

                    break;
                case 1:
                    atk?.Destroy();
                    atk = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "GoatSeguStaff2", player.Position + Vector3.right * size.x / 2 * (int)player.Direction);
                    Transform[] children = atk.transform.GetChildren();
                    atk.transform.localScale = size;

                    foreach (var transform in children)
                    {
                        transform.localScale = new Vector2(1 / size.x, 1);
                    }

                    atk.Init(player, new AtkItemCalculation(player, weapon, weapon.groundAtkDmgs[idx].dmg),2);
                    atk.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.groundAtkDmgs[idx].groggy / 100));

                    atk.AdditionalAtkCount = weapon.groundAtkDmgs[idx].atkCount - 1;
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(atk));

                    break;
                case 2:
                    atk?.Destroy();
                    var explosion = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "GoatSeguExp", player.Position + Vector3.right * size.x / 2 * (int)player.Direction);
                    explosion.transform.localScale = Vector3.one * expSize;
                    explosion.Init(player, new AtkItemCalculation(player, weapon,weapon.groundAtkDmgs[idx].dmg), 1);
                    explosion.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.groundAtkDmgs[idx].groggy / 100));

                    explosion.AdditionalAtkCount = weapon.groundAtkDmgs[idx].atkCount - 1;
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(explosion));
                    atk = null;
                    break;
            }
        }

        void AirAtk(int idx)
        {
            switch (idx)
            {
                case 0:
                    atk = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "GoatSeguStaff", player.Position + Vector3.right * size.x / 2 * (int)player.Direction);
                    atk.transform.localScale = size;
                    atk.Init(player, new AtkItemCalculation(player, weapon,weapon.airAtkDmgs[idx].dmg));
                    atk.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.airAtkDmgs[idx].groggy / 100));
                    atk.AdditionalAtkCount = weapon.airAtkDmgs[idx].atkCount - 1;
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(atk));

                    break;
                case 1:
                    atk?.Destroy();
                    atk = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "GoatSeguStaff2", player.Position + Vector3.right * size.x / 2 * (int)player.Direction);
                    Transform[] children = atk.transform.GetChildren();
                    atk.transform.localScale = size;

                    foreach (var transform in children)
                    {
                        transform.localScale = new Vector2(1 / size.x, 1);
                    }

                    atk.Init(player, new AtkItemCalculation(player, weapon,weapon.airAtkDmgs[idx].dmg));
                    atk.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.airAtkDmgs[idx].groggy / 100));

                    atk.AdditionalAtkCount = weapon.airAtkDmgs[idx].atkCount - 1;
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(atk));

                    break;
                case 2:
                    atk?.Destroy();
                    var explosion = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                        "GoatSeguExp", player.Position + Vector3.right * size.x / 2 * (int)player.Direction);
                    explosion.transform.localScale = Vector3.one * expSize;
                    explosion.Init(player, new AtkItemCalculation(player, weapon,weapon.airAtkDmgs[idx].dmg), 1);
                    explosion.Init(Mathf.RoundToInt(weapon.BaseGroggyPower * weapon.airAtkDmgs[idx].groggy / 100));
                    explosion.AdditionalAtkCount = weapon.airAtkDmgs[idx].atkCount - 1;
                    atk = null;
                    weapon.OnAtkObjectInit.Invoke(new EventParameters(explosion));

                    break;
            }
        }

        void Remove()
        {
            if (atk != null)
            {
                atk.Destroy();
                atk = null;
            }
        }
    }
}