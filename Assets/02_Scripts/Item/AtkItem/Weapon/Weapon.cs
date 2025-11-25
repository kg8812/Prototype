using System;
using Apis.DataType;
using System.Collections.Generic;
using System.Linq;
using Apis.Managers;
using Default;
using EventData;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace Apis
{
    public abstract class Weapon : Item , IAttackItem , IAttackItemStat
    {
        // protected new string name;
        // protected string flavourText;
        // protected string description;

        public override int ItemId => _data.weaponId;
        // public override string Name => name;
        //
        // public override string FlavourText => flavourText;
        //
        // public override string Description => description;

        #region Enums
        public enum WeaponGrade
        {
            Normal,Rare,Legend,Noble,Special
        }
        public enum AttackType
        {
            Collider,Projectile
        }

        #endregion
        
        #region Inspector
        
        [SerializeField] protected int dataId;
        [FormerlySerializedAs("skill")] [SerializeField] MagicSkill _skill;

        [SerializeField] private Transform firePos;

        public virtual Vector2 FirePos
        {
            get
            {
                if (firePos == null)
                {
                    return GameManager.instance.ControllingEntity.Position;
                }

                return firePos.transform.position;
            }
        }
        public float atkSpeed = 1;
        [LabelText("Appear,Disappear 적용여부")] public bool appearUse = true;
        [LabelText("스파인 여부")] public bool isSpine;

        [HideIf("isSpine")] public Sprite[] sprites;
        [HideIf("isSpine")]public Material[] materials;
        public string[] slots;
        [ShowIf("isSpine")] [SpineAtlasRegion] public string[] atlasRegion;
        [ShowIf("isSpine")] public SpineAtlasAsset atlasAsset;
        
        [TitleGroup("공격설정")]
        [TabGroup("공격설정/설정","지상")]
        [ShowIf("attackType",AttackType.Collider)]
        [LabelText("지상공격 데미지")] public List<atkInfo> groundAtkDmgs = new();
        [TabGroup("공격설정/설정","공중")]
        [ShowIf("attackType",AttackType.Collider)]
        [LabelText("공중공격 데미지")] public List<atkInfo> airAtkDmgs = new();

        [LabelText("따라다니기")] [SerializeField] bool isFollow;
        [InfoBox("BoneFollower 적용하는 오브젝트들 \n 값으로는 boneName 넣을 것")] public Dictionary<GameObject, string> followers;

        [LabelText("아이콘에 스킬 적용여부")] [SerializeField]
        private bool isSkillIcon;

        private SFXPlayer sfxPlayer;
        public SFXPlayer SFXPlayer => sfxPlayer ??= GetComponent<SFXPlayer>();
        #endregion
        
        #region  Variables, Property
        
        Weapon_Config _config;
        IWeaponStat _weaponStat;
        
        [HideInInspector] public MagicSkill Skill;

        public UI_AtkItemIcon Icon { get; set; }

        public int AtkSlotIndex { get; set; }
        public virtual float Atk => _config.BonusStat.Stats[ActorStatType.Atk].Value * (1 + _config.BonusStat.Stats[ActorStatType.Atk].Ratio / 100f);
        public virtual float Dmg => Atk;
        public Player Player => GameManager.instance.Player;
        
        private IWeaponAttack _iAttack;
        public virtual IWeaponAttack IAttack => _iAttack ??= new Weapon_BasicAttack(this);
        
        public int AtkMotionType => IAttack.AttackType;
        public virtual AttackType attackType => AttackType.Collider;
        public WeaponGrade Grade => Data.weaponRarity;
        public WeaponSprite[] wpSprites { get; protected set; }

        private BoneFollower[] _boneFollower;

        public BoneFollower[] BoneFollower =>
            _boneFollower ??= followers?.Select(x => x.Key.GetComponent<BoneFollower>()).Where(x => x != null).ToArray();

        private List<float> groundCancelTimes;
        private List<float> airCancelTimes;
        
       
        public virtual List<float> GroundCancelTimes => groundCancelTimes ??= groundAtkDmgs.Select(x => x.cancelTime).ToList();
        public virtual List<float> AirCancelTimes => airCancelTimes ??= airAtkDmgs.Select(x => x.cancelTime).ToList();
        
        public bool IsFollow => isFollow;


        #endregion

        #region Events
        
        private UnityEvent<EventParameters> _onAtkObjectInit;
        public UnityEvent<EventParameters> OnAtkObjectInit => _onAtkObjectInit ??= new();
        public UnityEvent<int> GroundComboList => IAttack.GroundAttacks;
        public UnityEvent<int> AirComboList => IAttack.AirAttacks;
        #endregion
        
        #region 테이블 데이터
        WeaponDataType _data;
        public virtual WeaponDataType Data => _data;

        public virtual int Index => dataId;
        public virtual int MotionIndex => dataId;
        public virtual float BaseGroggyPower => Data.baseGroggyPower;

        public float BodyFactor => Data.bodyFactor;
        public float SpiritFactor => Data.spiritFactor;
        public float FinesseFactor => Data.finesseFactor;

        private Action<int> _onAttackEnd;

        #endregion
       
        [Serializable]
        public struct atkInfo
        {
            [LabelText("공격 설정(%)")] public float dmg;
            [LabelText("그로기 계수(%)")] public float groggy;
            [LabelText("캔슬 시간 계수(%)")] public float cancelTime;
            [LabelText("공격횟수")] public int atkCount;
            [LabelText("일반 넉백")] public KnockBackData knockBackData;
            [LabelText("그로기 넉백")] public KnockBackData groggyKnockBackData;
        }
       
        public virtual void WhenIconIsSet(UI_AtkItemIcon icon)
        {
            icon?.ChangeType(new UI_AtkItemIcon.NoUpdate(icon));
            if (isSkillIcon)
            {
                if (icon == null) return;

                icon.ChangeType(new UI_AtkItemIcon.NormalCdUpdate(icon));
                icon.Skill = Skill;
            }
        }
        public int CalculateGroggy(float factor)
        {
            return Mathf.RoundToInt(BaseGroggyPower * factor / 100f);
        }
        
        public virtual void SetGroundCollider(int idx, AttackObject[] attackObjs)
        {
            if (groundAtkDmgs.Count <= idx) return;
            foreach (var t in attackObjs)
            {
                t.Init(Player, new AtkItemCalculation(Player,this,groundAtkDmgs[idx].dmg));
                t.Init(Mathf.RoundToInt(BaseGroggyPower * groundAtkDmgs[idx].groggy / 100f));
                t.AdditionalAtkCount = groundAtkDmgs[idx].atkCount - 1;
                
                // knockback 관련 세팅
                t.knockBackData = groundAtkDmgs[idx].knockBackData;
                t.groggyKnockBackData = groundAtkDmgs[idx].groggyKnockBackData;
                OnAtkObjectInit.Invoke(new EventParameters(t));
            }
        }

        public virtual void SetAirCollider(int idx, AttackObject[] attackObjs)
        {
            if (airAtkDmgs.Count <= idx) return;

            foreach (var t in attackObjs)
            {
                t.Init(Player, new AtkItemCalculation(Player,this,airAtkDmgs[idx].dmg));
                t.Init(Player.atkInfo);
                t.Init(Mathf.RoundToInt(BaseGroggyPower * airAtkDmgs[idx].groggy / 100f));
                t.AdditionalAtkCount = airAtkDmgs[idx].atkCount - 1;
                t.knockBackData = airAtkDmgs[idx].knockBackData;
                t.groggyKnockBackData = airAtkDmgs[idx].groggyKnockBackData;
                OnAtkObjectInit.Invoke(new EventParameters(t));
            }
        }
        public virtual void Attack(int count)
        {            
            if(IAttack == null)
            {
                Debug.Log("공격이 할당되지 않음");
                return;
            }

            if (Player.onAir && count > 0)
            {
                AirComboList.Invoke(count - 1);
            }
            else if (count > 0)
            {
                GroundComboList.Invoke(count - 1);
            }
        }

        public void AfterAtk()
        {
            IAttack.OnAfterAtk?.Invoke();
        }
        public virtual bool TryAttack()
        {
            return true;
        }

        public override void Init()
        {
            base.Init();

            WeaponData.DataLoad.TryGetWeaponData(Index, out _data);
            // name = LanguageManager.Str(_data.weaponNameString);
            // flavourText = LanguageManager.Str(_data.description);
            
            // name ??= gameObject.name;
            // flavourText ??= "";
           
            _config = new Weapon_Config();
            _config.BonusStat.AddValue(ActorStatType.Atk, _data.attackPower);
            Decorate();
            if(_skill != null)
            {
                Skill = Instantiate(_skill);
                Skill.Init(this);
            }

            if (Image == null)
            {
                Image = ResourceUtil.Load<Sprite>(_data.iconPath);
            }

            wpSprites = GetComponentsInChildren<WeaponSprite>(true);
            foreach (var x in wpSprites)
            {
                x.ActiveRenderer(false);
            }
        }

        public override void Activate()
        {
            
        }

        // 스킬 사용 함수
        public void UseSkill()
        {
            if (Skill != null && Skill.isActive)
            {
                Skill.Use();
            }
        }

        protected override void OnEquip(IMonoBehaviour user)
        {
            base.OnEquip(user);
            Skill?.Equip(user);
        }

        protected override void OnUnEquip()
        {
            base.OnUnEquip();

            foreach (var x in wpSprites)
            {
                x.ActiveRenderer(false);
                x.Condition = 1;
            }

            SetBoneFollwer(false);
            
            if (!ReferenceEquals(Skill,null))
            {
                Skill.UnEquip();
            }
            IAttack?.OnAfterAtk?.Invoke();
        }

        private List<IWeaponStat> attachments = new();

        void Decorate()
        {
            _weaponStat = new Weapon_Stat(_config);
            foreach (var x in attachments)
            {
                if (x != null)
                {
                    _weaponStat = new Weapon_Decorator(_weaponStat, x);
                }
            }
        }
        
        public override void Return()
        {
            base.Return();
            GameManager.Item.Weapon.Return(this);
        }

        public virtual void OnAnimEnter(int index)
        {
            
        }

        public virtual void OnAnimExit(int index)
        {
            
        }

        public virtual void BeforeAttack()
        {
            Player.SetAttackToNormal();
            foreach (var x in wpSprites)
            {
                x.ActiveRenderer(true);
                x.Condition = 1;
            }
            Player._overrider.SetAnimation(MotionIndex,Player);
            Player.animator.SetInteger("AttackType", AtkMotionType);
            if (IsFollow)
            {
                var follower = gameObject.GetOrAddComponent<PetFollower>();
                follower.Init(Player.orbPos,Player);
                SetBoneFollwer(false);
            }
            else
            {
                SetBoneFollwer(true);
            }
        }

        void SetBoneFollwer(bool isOn)
        {
            if (followers != null)
            {
                foreach (var x in followers.Keys)
                {
                    BoneFollower follower = Utils.GetOrAddComponent<BoneFollower>(x.gameObject);

                    if (isOn)
                    {
                        follower.enabled = true;
                        follower.skeletonRenderer = Player.Mecanim;
                        follower.followLocalScale = true;
                        follower.followParentWorldScale = true;
                        follower.boneName = followers[x];
                        follower.Initialize();
                    }
                    else
                    {
                        follower.enabled = false;
                        follower.skeletonRenderer = null;
                    }
                }
            }
        }
        public virtual void UseAttack()
        {
            Player.SetState(EPlayerState.Attack);
        }

        public virtual void EndAttack()
        {
            Skill?.DeActive();
        }
        protected AttackCategory category;
        public AttackCategory Category => category;
    }
}
