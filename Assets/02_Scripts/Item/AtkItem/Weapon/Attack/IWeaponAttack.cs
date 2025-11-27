using System;
using System.Collections.Generic;
using DG.Tweening;
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
        private UnityEvent _onAfterAtk;
        protected Weapon weapon;

        public Weapon_BasicAttack(Weapon weapon)
        {
            GroundAttacks.RemoveAllListeners();
            AirAttacks.RemoveAllListeners();
            GroundAttacks.AddListener(GroundAttack);
            AirAttacks.AddListener(AirAttack);
            this.weapon = weapon;
        }

        protected Player player => weapon?.Player;
        public UnityEvent<int> GroundAttacks { get; } = new();

        public UnityEvent<int> AirAttacks { get; } = new();

        public UnityEvent OnAfterAtk => _onAfterAtk ??= new UnityEvent();

        public virtual int AttackType => 1;

        public virtual void GroundAttack(int index)
        {
        }

        public virtual void AirAttack(int index)
        {
        }
    }

    public class ProjectileAttack : IWeaponAttack // 투사체 공격
    {
        public enum ProjectileType
        {
            Bullet
        }

        protected readonly ProjectileWeapon weapon;
        private UnityEvent _onAfterAtk;
        protected Action<Projectile, int, int> BeforeAirFire;
        protected Action<Projectile, int, int> BeforeGroundFire;
        protected Action<Projectile, int, int> OnAirFire;

        protected Action<Projectile, int, int> OnGroundFire;

        private string projectileName;

        public ProjectileAttack(ProjectileWeapon weapon)
        {
            GroundAttacks.RemoveAllListeners();
            AirAttacks.RemoveAllListeners();

            GroundAttacks.AddListener(GroundAttack);
            AirAttacks.AddListener(AirAttack);

            this.weapon = weapon;
        }

        protected Player player => weapon?.Player;
        public virtual int AttackType => 1;

        public UnityEvent<int> GroundAttacks { get; } = new();

        public UnityEvent<int> AirAttacks { get; } = new();

        public UnityEvent OnAfterAtk => _onAfterAtk ??= new UnityEvent();

        protected string GetProjectileName(ProjectileType projectileType)
        {
            return projectileType switch
            {
                ProjectileType.Bullet => "bullet",
                _ => ""
            };
        }

        private Projectile CreateProjectile(int idx, ProjectileType projectileType)
        {
            projectileName = GetProjectileName(projectileType);

            return GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.AttackObject,
                projectileName, GetProjPos(idx));
        }

        protected virtual Vector2 GetProjPos(int idx)
        {
            return weapon.FirePos;
        }

        private FiredInfo FireProjectile(List<ProjectileWeapon.ProjInfo> infos, int idx, GroundOrAir groundOrAir)
        {
            List<Projectile> list = new();
            var seq = DOTween.Sequence();
            var guid = Guid.NewGuid();
            for (var i = 0; i < infos[idx].projCount; i++)
            {
                var temp = i;

                var proj = CreateProjectile(i, infos[idx].projType);
                proj.gameObject.SetActive(false);
                seq.AppendCallback(() =>
                {
                    switch (groundOrAir)
                    {
                        case GroundOrAir.Ground:
                            BeforeGroundFire?.Invoke(proj, idx, temp);
                            break;
                        case GroundOrAir.Air:
                            BeforeAirFire?.Invoke(proj, idx, temp);
                            break;
                    }

                    proj.gameObject.SetActive(true);
                    proj.Init(player, new AtkItemCalculation(player, weapon, infos[idx].info.dmg));
                    proj.Init(infos[idx].info);
                    proj.firedAtkGuid = guid;
                    proj.Fire();
                    switch (groundOrAir)
                    {
                        case GroundOrAir.Ground:
                            OnGroundFire?.Invoke(proj, idx, temp);
                            break;
                        case GroundOrAir.Air:
                            OnAirFire?.Invoke(proj, idx, temp);
                            break;
                    }

                    weapon.OnAtkObjectInit.Invoke(new EventParameters(proj));
                });
                seq.AppendInterval(infos[idx].fireTerm);
                list.Add(proj);
            }

            return new FiredInfo
            {
                seq = seq,
                projectiles = list
            };
        }

        protected virtual FiredInfo FireGroundProjectile(int idx)
        {
            return FireProjectile(weapon.groundProjectileInfos, idx, GroundOrAir.Ground);
        }

        protected virtual FiredInfo FireAirProjectile(int idx)
        {
            return FireProjectile(weapon.airProjectileInfos, idx, GroundOrAir.Air);
        }

        protected void GroundAttack(int idx)
        {
            FireGroundProjectile(idx);
        }

        protected void AirAttack(int idx)
        {
            FireAirProjectile(idx);
        }

        public struct FiredInfo
        {
            public Sequence seq;
            public List<Projectile> projectiles;
        }

        private enum GroundOrAir
        {
            Ground,
            Air
        }
    }
}