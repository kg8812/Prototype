using Apis;
using Apis.UI.Focus;
using NewNewInvenSpace;
using UnityEngine;

namespace Apis
{
    public class UITab_AttackItem: UITab_Inventory
    {
        public FocusParent playerSkillParent;
        public SkillSlot playerActiveSkillSlot;
        public SkillSlot playerPassiveSkillSlot;
        protected override InventoryGroup invenGroupManager => InvenManager.instance.AttackItem;

        private AttackItemDescription AtkDesc => description as AttackItemDescription;

        public override void Init()
        {
            playerSkillParent.FocusGroup = this;
            base.Init();

            SkillSlotInit(playerActiveSkillSlot);
            SkillSlotInit(playerPassiveSkillSlot);
        }

        private void SkillSlotInit(SkillSlot skillSlot)
        {
            skillSlot.InitCheck();
            skillSlot.OnValueChanged.AddListener(isOn =>
            {
                if(isOn)
                    AtkDesc.ChangeInfo(skillSlot.CurSkill);
            });
            skillSlot.UpdateSkill();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            playerActiveSkillSlot.UpdateSkill();
            playerPassiveSkillSlot.UpdateSkill();
        }
        
        protected override void ChangeOn()
        {
            base.ChangeOn();
            Equipment.tableData.moveLeft = MoveLeftOnEquipment;
            Equipment.tableData.moveRight = MoveRightOnEquipment;

            playerSkillParent.tableData.moveUp = MoveUpOnSkill;
            playerSkillParent.tableData.moveDown = MoveDownOnSkill;
            playerSkillParent.tableData.moveLeft = MoveLeftOnSkill;
            playerSkillParent.tableData.moveRight = MoveRightOnSkill;
        }
        protected override void ChangeOff()
        {
            base.ChangeOff();
            Equipment.tableData.moveLeft = null;
            Equipment.tableData.moveRight = null;

            playerSkillParent.tableData.moveUp = null;
            playerSkillParent.tableData.moveDown = null;
            playerSkillParent.tableData.moveLeft = null;
            playerSkillParent.tableData.moveRight = null;
        }
        
        public override void ChangeFocusParent(FocusParent fp)
        {
            _curFocus = fp;
            if (ReferenceEquals(fp, Equipment))
            {
                Inven.FocusReset();
                playerSkillParent.FocusReset();
            }
            else if(ReferenceEquals(fp, Inven))
            {
                Equipment.FocusReset();
                playerSkillParent.FocusReset();
            }
            else
            {
                Inven.FocusReset();
                Equipment.FocusReset();
            }
        }
        
        
        
        #region KeyBoardMoveSection

        protected override void MoveUpOnEquipment(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Equipment.tableData.x + playerSkillParent.tableData.x, Inven.tableData.x);
            
            Inven.MoveTo(realX + ((Inven.focusList.Count - realX - 1) / Inven.tableData.x) * Inven.tableData.x );
            _curFocus = Inven;
        }

        protected override void MoveDownOnEquipment(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Equipment.tableData.x + playerSkillParent.tableData.x, Inven.tableData.x);
            Inven.MoveTo(realX);
            _curFocus = Inven;
        }
        private void MoveLeftOnEquipment(int y)
        {
            playerSkillParent.MoveTo(Mathf.Max(playerSkillParent.focusList.Count-1, (y+1)*(playerSkillParent.tableData.x)-1));
            _curFocus = playerSkillParent;
        }
        
        private void MoveRightOnEquipment(int y)
        {
            playerSkillParent.MoveTo(y*playerSkillParent.tableData.x);
            _curFocus = playerSkillParent;
        }

        protected override void MoveUpOnInven(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Inven.tableData.x, Equipment.tableData.x + playerSkillParent.tableData.x);
            
            if (realX < Equipment.tableData.x)
            {
                Equipment.MoveTo(realX + ((Equipment.focusList.Count - realX - 1) / Equipment.tableData.x) * Equipment.tableData.x );
                _curFocus = Equipment;
            }
            else
            {
                realX -= Equipment.tableData.x;
                playerSkillParent.MoveTo(realX + ((playerSkillParent.focusList.Count - realX - 1) / playerSkillParent.tableData.x) * playerSkillParent.tableData.x );
                _curFocus = playerSkillParent;
            }
        }

        protected override void MoveDownOnInven(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Inven.tableData.x, Equipment.tableData.x + playerSkillParent.tableData.x);
            
            // 해당하는 가장 밑의 개체로 이동.
            if (realX < Equipment.tableData.x)
            {
                Equipment.MoveTo(realX);
                _curFocus = Equipment;
            }
            else
            {
                realX -= Equipment.tableData.x;
                playerSkillParent.MoveTo(realX);
                _curFocus = playerSkillParent;
            }
        }
        
        
        
        private void MoveUpOnSkill(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x+Equipment.tableData.x, Equipment.tableData.x + playerSkillParent.tableData.x, Inven.tableData.x);
            // 해당하는 가장 밑의 개체로 이동.
            Inven.MoveTo(realX + ((Inven.focusList.Count - realX - 1) / Inven.tableData.x) * Inven.tableData.x );
            _curFocus = Inven;
        }

        private void MoveDownOnSkill(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x+Equipment.tableData.x, Equipment.tableData.x + playerSkillParent.tableData.x, Inven.tableData.x);
            Inven.MoveTo(realX);
            _curFocus = Inven;
        }
        
        private void MoveLeftOnSkill(int y)
        {
            Equipment.MoveTo(Mathf.Max(Equipment.focusList.Count-1, (y+1)*(Equipment.tableData.x)-1));
            _curFocus = Equipment;
        }

        private void MoveRightOnSkill(int y)
        {
            Equipment.MoveTo(y*Equipment.tableData.x);
            _curFocus = Equipment;
        }
        

        #endregion
    }
}