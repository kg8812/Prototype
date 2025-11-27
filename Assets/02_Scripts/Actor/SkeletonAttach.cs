using Apis;
using Default;
using Unity.Mathematics;
using UnityEngine;

public class SkeletonAttach : MonoBehaviour
{
    private Actor _actor;
    public Actor Actor => _actor ??= transform.GetComponentInParentAndChild<Actor>();

    protected virtual void Awake()
    {
    }

    public void AttackInCombo(int combo)
    {
        if (Actor is Player player) player.AttackInCombo(combo);
    }

    public void Attack(int combo)
    {
        if (Actor is Player player) player.Attack(combo);
    }

    public void Slash()
    {
        Debug.Log("Slash");
        if (Actor is Player player) player.Slash();
    }

    public void PlaySFX(string fileName)
    {
        GameManager.Sound.PlayInPosition(fileName, "SFXsetting01", Actor.Position, Define.Sound.SFX);
    }

    public void PlayAmbience(string fileName)
    {
        GameManager.Sound.PlayInPosition(fileName, "AmbienceSetting01", Actor.Position, Define.Sound.Ambience);
    }

    public void SpawnVFX(string address)
    {
        if (Actor != null)
        {
            Actor.EffectSpawner.Spawn(address, Actor.effectParent.position, true);
        }
        else
        {
            var vfx = GameManager.Factory.Get<ParticleSystem>(FactoryManager.FactoryType.Effect, address);
            vfx.transform.SetParent(Actor.effectParent);
            vfx.transform.localPosition = Vector3.zero;
            vfx.transform.localScale = Vector3.one;
            vfx.transform.localRotation = quaternion.identity;
            GameManager.Factory.Return(vfx.gameObject, vfx.main.duration);
        }
    }

    public void IdleOn()
    {
        Actor.IdleOn();
    }

    public void GameOver()
    {
        GameManager.instance.GameOver();
    }

    public void Die()
    {
        Actor.Die();
    }

    public void Return()
    {
        GameManager.Factory.Return(transform.parent.gameObject);
    }

    public void StopComboDelay()
    {
        if (Actor is not Player player) return;

        player.CoolDown.CompleteCd(EPlayerCd.AttackComboDelay);
    }

    public void StopAfterDelay()
    {
        if (Actor is not Player player) return;

        player.CoolDown.CompleteCd(EPlayerCd.AttackAfterDelay);
    }

    public void Dummy()
    {
    }
}