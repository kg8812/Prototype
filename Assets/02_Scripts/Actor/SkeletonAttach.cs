using Apis;
using Default;
using Unity.Mathematics;
using UnityEngine;

public class SkeletonAttach : MonoBehaviour
{
    Actor _actor;
    public Actor Actor => _actor ??= transform.GetComponentInParentAndChild<Actor>();
    private FootPrintCreator _footPrintCreator;
    protected virtual void Awake()
    {
        _footPrintCreator = Actor.transform.GetComponentInParentAndChild<FootPrintCreator>();
    }
    
    public void AttackInCombo(int combo)
    {
        if (Actor is Player player)
        {
            player.AttackInCombo(combo);
        }
    }
    
    public void Attack(int combo)
    {
        if (Actor is Player player)
        {
            player.Attack(combo);
        }
    }

    public void Slash()
    {
        Debug.Log("Slash");
        if (Actor is Player player)
        {
            player.Slash();
        }
    }
    
    public void PlaySFX(string fileName)
    {
        GameManager.Sound.PlayInPosition(fileName,"SFXsetting01",Actor.Position,Define.Sound.SFX);
    }

    public void PlayAmbience(string fileName)
    {
        GameManager.Sound.PlayInPosition(fileName,"AmbienceSetting01",Actor.Position,Define.Sound.Ambience);
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
    
    public void SpawnVFXInSpine(string address)
    {
        if (Actor != null)
        {
            Actor.EffectSpawner.Spawn(address, "center",Vector2.zero,true);
        }
        else
        {
            Debug.LogError("Actor 할당이 되지 않았습니다.");
        }
    }
    public void ActionWeaponSkill(int index)
    {
        if (Actor is Player player)
        {
            player.DoWeaponSkillAction(index);
        }
    }

    public void ActionActiveSkill(int index)
    {
        if (Actor is Player player && player.ActiveSkill != null)
        {
            player.ActiveSkill.actionList[index].Invoke();
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

    public void PullGrab(float time)
    {
        if(Actor is Player player)
            player.PullGrab(time);
    }

    public void InvinciblityOn()
    {
        if(Actor is Player player)
            player.Invincibility(true);
    }

    public void InvincibilityOff()
    {
        if(Actor is Player player)
            player.Invincibility(false);
    }

    public void ShakePlayerCam(string str)
    {
        Actor.ShakePlayerCam(str);
    }

    public void CreateFootPrint()
    {
        if (_footPrintCreator != null)
        {
            _footPrintCreator.SpawnFootPrint();
        }
    }

    public void AttackEvent(string str)
    {
        if(Actor is not Player player || str.Length == 0) return;

        var indices = str.Split(",");

        if(indices.Length > 2){
            Debug.LogError("Too Many Indices");
            return;
        }

        if(player.AttackEvents == null){
            Debug.LogError("AttackEvent Container is not loaded");
            return;
        }

        if(player.PressingDir == 0 
            && indices[0].Length > 0 
            && int.TryParse(indices[0], out int istay))
        {
            // 정지 공격 이벤트
            if(player.AttackEvents.Count <= istay){
                Debug.LogError("AttackEvents index out of range");
                return;
            }
            player.AttackEvents[istay].Invoke(player);
        }
        else if(player.PressingDir != 0 
            && indices[1].Length > 0
            && int.TryParse(indices[1], out int imove))
        {
            // 이동동 공격 이벤트
            if(player.AttackEvents.Count <= imove){
                Debug.LogError("AttackEvents index out of range");
                return;
            }
            player.AttackEvents[imove].Invoke(player);
        }
    }

    public void StopComboDelay()
    {
        if(Actor is not Player player) return;

        player.CoolDown.CompleteCd(EPlayerCd.AttackComboDelay);
    }

    public void StopAfterDelay()
    {
        if(Actor is not Player player) return;

        player.CoolDown.CompleteCd(EPlayerCd.AttackAfterDelay);
    }
    
    public void Dummy()
    {
        
    }
}
