using System.Collections.Generic;
using System.Text;
using _02_Scripts.UI.UI_SubItem;
using Apis;
using Apis;
using Apis.DataType;
using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using NewNewInvenSpace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UISpaces
{
    public class UI_Shop : UI_Scene
    {
        // private const int Error_Shortage_Gold = 402;
        // private const int Error_Shortage_LobbyGold = 403;
        
        
        public enum ShopType
        {
            All,
            SkillTree,
            AttackItem,
            Accessory,
            Enhance
        }

        #region Bindings

        private enum Texts
        {
            ShopTitle,
            
            Item_Name,
            Item_Description,
            
            AtkItem_Spirit,       // 영
            AtkItem_Body,       // 신
            AtkItem_Finesse,    // 기
            
            Acc_Cost
        }

        private enum GameObjects
        {
            AtkItem_Stat_Obj,
            Acc_Cost_Obj,
            ItemImg
        }

        private enum Transforms
        {
            ListContent
        }

        private enum FocusParents
        {
            ShopHeader, ListContent
        }

        private enum Images
        {
            ItemImg
        }

        private enum RawImages
        {
            SkillTree_VideoImg
        }

        private enum VideoPlayers
        {
            VideoPlayer
        }

        #endregion
        

        private List<UI_ShopListItem> _createdListItem;

        private ShopType _curShopType;
        private ShopDataType shopData;
        private List<ShopListDataType> shopItemListData;
        private List<ShopListDataType> showShopItemListData;

        private FocusParent _headerFocus;
        private FocusParent _contentFocus;


        private UI_ShopListItem _curSelectedListItem;
        


        #region Init

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<GameObject>(typeof(GameObjects));
            Bind<Transform>(typeof(Transforms));
            Bind<FocusParent>(typeof(FocusParents));
            Bind<Image>(typeof(Images));
            Bind<RawImage>(typeof(RawImages));
            Bind<VideoPlayer>(typeof(VideoPlayers));

            showShopItemListData = new();
            _createdListItem ??= new();
            _headerFocus = Get<FocusParent>((int)FocusParents.ShopHeader);
            _contentFocus = Get<FocusParent>((int)FocusParents.ListContent);
            
            _headerFocus.InitCheck();
            _contentFocus.InitCheck();

            Get<VideoPlayer>((int)VideoPlayers.VideoPlayer).isLooping = true;
            Get<VideoPlayer>((int)VideoPlayers.VideoPlayer).Stop();


            // shop header shop type 업데이트이벤트 달아주기
            Get<FocusParent>((int)FocusParents.ShopHeader).FocusChanged.AddListener((int ind) =>
            {
                UpdateShopType((ShopType)ind);
            });
        }
        
        public void Init(int id)
        {
            if (!ShopData.TryGetShopData(id, out shopData)) return;
            Get<TextMeshProUGUI>((int)Texts.ShopTitle).text = LanguageManager.Str(shopData.shopName) ?? "상점";

            if (!ShopData.TryGetShopListData(shopData.sellingGroup, out shopItemListData)) return;

            
            UpdateShopType(ShopType.All, true);
            Get<FocusParent>((int)FocusParents.ShopHeader).MoveTo(0);
            Get<FocusParent>((int)FocusParents.ListContent).MoveTo(0);
        }

        public override void TryActivated(bool force = false)
        {
            Get<FocusParent>((int)FocusParents.ShopHeader)?.MoveTo(0);
            Get<FocusParent>((int)FocusParents.ListContent)?.MoveTo(0);
            base.TryActivated(force);
        }

        #endregion
        

        #region Update

        public void UpdateShopType(ShopType shopType, bool force = false)
        {
            if (!force && _curShopType == shopType || shopItemListData == null) return;

            _curShopType = shopType;
            
            UpdateListDataByShopType(_curShopType);
            for (int i = 0; i < Mathf.Max(showShopItemListData.Count, _createdListItem.Count); i++)
            {
                // 보여줄 list item이 있는 경우
                if (i < showShopItemListData.Count)
                {
                    // list item 오브젝트가 존재하지 않는 경우 -> 생성
                    if (i >= _createdListItem.Count)
                    {
                        CreateListItem();
                    }
                        
                    if (!_createdListItem[i].gameObject.activeSelf)
                    {
                        _createdListItem[i].gameObject.SetActive(true);
                        _createdListItem[i].SelectOff(true);
                        Get<FocusParent>((int)FocusParents.ListContent).RegisterElement(_createdListItem[i], i);
                    }
                    _createdListItem[i].UpdateShopItem = UpdateItemDescription;
                    _createdListItem[i].UpdateData(showShopItemListData[i]);
                }
                else
                {
                    // 없으니 disable + focusParent에서 해제
                    if (_createdListItem[i].gameObject.activeSelf)
                    {
                        _createdListItem[i].SelectOff(true);
                        _createdListItem[i].gameObject.SetActive(false);
                        _createdListItem[i].UpdateShopItem = null;
                        Get<FocusParent>((int)FocusParents.ListContent).RemoveElement(_createdListItem[i]);
                    }
                }
            }
            Get<FocusParent>((int)FocusParents.ListContent).MoveTo(0, true);
        }

        private void UpdateListDataByShopType(ShopType shopType)
        {
            showShopItemListData.Clear();
            foreach (var item in shopItemListData)
            {
                if(!CheckItemShow(item)) continue;
                switch (shopType)
                {
                    case ShopType.All:
                        showShopItemListData.Add(item);
                        break;
                    
                    case ShopType.SkillTree:
                        if(item.itemType == 4)
                            showShopItemListData.Add(item);
                        break;
                    
                    case ShopType.AttackItem:
                        if(item.itemType is 2 or 3)
                            showShopItemListData.Add(item);
                        break;
                    
                    case ShopType.Accessory:
                        if(item.itemType == 1)
                            showShopItemListData.Add(item);
                        break;
                    
                    case ShopType.Enhance:
                        // TODO: 아직 5 type은 없음.
                        // 추가 강화 item들 여기에 할당.
                        if(item.itemType == 5)
                            showShopItemListData.Add(item);
                        break;
                }
            }
        }

        /// <summary>
        /// unlock 등 조건 여부
        /// </summary>
        private bool CheckItemShow(ShopListDataType listData)
        {
            return true;
            // TODO: 상점 해금 체크 로직.
            switch (listData.itemType)
            {
                // 재화
                case 0:
                    return true;
                
                // acc
                case 1:
                    break;
                
                // 마법
                case 2:
                    break;
                
                // 무기
                case 3:
                    break;
                
                // 스킬트리
                case 4:
                    break;
                
                // 강화?
                case 5:
                    return true;
                    break;
            }

            return true;
        }

        

        private void CreateListItem()
        {
            UI_ShopListItem item = GameManager.UI.MakeSubItem(
                "ShopListItem", 
                Get<Transform>((int)Transforms.ListContent)
                ) as UI_ShopListItem;
            if (item == null) return;
            item.InitCheck();
            _createdListItem.Add(item);
            Get<FocusParent>((int)FocusParents.ListContent).RegisterElement(item);
            item.UpdateShopItem = UpdateItemDescription;
        }

        private void UpdateItemDescription(UI_ShopListItem listItem)
        {
            ShopListDataType itemData = listItem?.ItemData;
            // item 선택이 안된 경우. 일단 만들고 봄. 애초에 없을것 같음.
            if (listItem != null && itemData != null)
            {
                /*
                 * 0 = 재화
                 * 1 = 악세사리
                 * 2 = 마법
                 * 3 = 무기
                 * 4 = 고유트리
                 */
                
                _curSelectedListItem = listItem;
                // 악세 cost 설정( 1 = 악세 )
                if (itemData.itemType == 1)
                {
                    if (!AccessoryData.DataLoad.TryGetData(itemData.itemId, out var accData))
                    {
                        Debug.Log($"{itemData.itemId} index의 acc가 존재하지 않습니다.");
                    }
                    else
                    {
                        // GetImage((int)Images.ItemImg).sprite = ResourceUtil.Load<Sprite>(accData.iconPath);
                        GetText((int)Texts.Acc_Cost).text = $"Cost: {accData.cost}";
                    }
                }
                Get<GameObject>((int)GameObjects.Acc_Cost_Obj).SetActive(itemData.itemType == 1);
                
                
                // 무장 영신기 스탯 설정 (2, 3 = 마법, 무기)
                if (itemData.itemType == 2)
                {
                    // TODO: skill dict를 index로 가능하게끔
                    // if (!MagicSkillDatas.magicSkills.TryGetValue(itemData.itemId, out var activeSkillData)
                    //     || !GameManager.Item.ActiveSkill.SkillDict.TryGetValue(itemData.itemName, out var activeSkillData2))
                    // {
                    //     Debug.Log($"{itemData.itemId} index의 skill이 존재하지 않습니다.");
                    // }
                    // else
                    // {
                    //     GetImage((int)Images.ItemImg).sprite = ResourceUtil.Load<Sprite>(activeSkillData.iconPath);
                    //     GetText((int)Texts.AtkItem_Spirit).text = Mathf.RoundToInt(activeSkillData2.SpiritFactor) + "%";
                    //     GetText((int)Texts.AtkItem_Body).text = Mathf.RoundToInt(activeSkillData2.BodyFactor) + "%";
                    //     GetText((int)Texts.AtkItem_Finesse).text = Mathf.RoundToInt(activeSkillData2.FinesseFactor) + "%";
                    // }
                }
                if (itemData.itemType == 3)
                {
                    if (!WeaponData.DataLoad.TryGetWeaponData(itemData.itemId, out var wpData))
                    {
                        Debug.Log($"{itemData.itemId} index의 weapon이 존재하지 않습니다.");
                    }
                    else
                    {
                        // GetImage((int)Images.ItemImg).sprite = ResourceUtil.Load<Sprite>(wpData.iconPath);
                        GetText((int)Texts.AtkItem_Spirit).text = Mathf.RoundToInt(wpData.spiritFactor) + "%";
                        GetText((int)Texts.AtkItem_Body).text = Mathf.RoundToInt(wpData.bodyFactor) + "%";
                        GetText((int)Texts.AtkItem_Finesse).text = Mathf.RoundToInt(wpData.finesseFactor) + "%";
                    }
                }
                Get<GameObject>((int)GameObjects.AtkItem_Stat_Obj).SetActive(itemData.itemType is 2 or 3);
                
                // Item Image
                Get<GameObject>((int)GameObjects.ItemImg).SetActive(itemData.itemType is 1 or 2 or 3);
                
                
                // 고유트리 비디오 설정 ( 4 = 고유트리)

                // Get<VideoPlayer>((int)VideoPlayers.VideoPlayer).clip = ResourceUtil.Load<VideoClip>(itemData.itemString);
                Get<RawImage>((int)RawImages.SkillTree_VideoImg).enabled = itemData.itemType == 4;
                // Get<VideoPlayer>((int)VideoPlayers.VideoPlayer).Play();
                
                
                // 공통 정보: 이름, 일러, 효과, 플레이버 텍스트, 가격
                LanguageManager.RegisterText(GetText((int)Texts.Item_Name), itemData.itemId);
                // LanguageManager.RegisterText(GetText((int)Texts.Item_Description), itemData.itemId);
                GetText((int)Texts.Item_Description).text = "아이템 설명란입니다.";
                
            }
        }

        #endregion


        #region Control

        public override void KeyControl()
        {
            base.KeyControl();
            _headerFocus.KeyControl();
            _contentFocus.KeyControl();
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                TryBuy(_curSelectedListItem);
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            _headerFocus.GamePadControl();
            _contentFocus.GamePadControl();
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                TryBuy(_curSelectedListItem);
            }
        }

        #endregion



        #region 로직

        private bool TryBuy(UI_ShopListItem listItem)
        {
            if (listItem == null || listItem.ItemData == null) return false;
            
            // price check
            switch (listItem.ItemData.priceType)
            {
                // 0 = gold, 1 = lobby gold
                case 0:
                    if (GameManager.instance.Soul < listItem.ItemData.price) return false;
                    break;
                case 1:
                    if (GameManager.instance.LobbySoul < listItem.ItemData.price) return false; 
                    break;
            }
            
            listItem.BuyTextToggle(true);
            Buy(listItem.ItemData);
            return true;
        }

        private void Buy(ShopListDataType listData)
        {
            if (listData == null) return;
            
            // 재화 차감
            switch (listData.priceType)
            {
                // 0 = gold(soul)
                case 0:
                    GameManager.instance.Soul -= listData.price;
                    break;
                // 1 = lobbyGold(Lobbysoul)
                case 1:
                    GameManager.instance.LobbySoul -= listData.price;
                    break;
            }
            
            // 아이템 추가
            switch (listData.itemType)
            {
                // 1 = acc
                case 1:
                    // if (!AccessoryData.DataLoad.TryGetData(listData.itemId, out var accData)) return;
                    // string accName = LanguageManager.Str(accData.accName);
                    if (InvenManager.instance.Acc.IsFull(InvenType.Storage))
                    {
                        var pickUp = GameManager.Item.AccPickUp.CreateNew(listData.itemId);
                        pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
                    }
                    else
                    {
                        var acc = GameManager.Item.GetAcc(listData.itemId);
                        InvenManager.instance.Acc.Add(acc, InvenType.Storage);
                    }
                    break;
                
                // 2 = magic skill
                case 2:
                    if (InvenManager.instance.AttackItem.IsFull(InvenType.Storage))
                    {
                        var pickUp = GameManager.Item.ActiveSkillPickUp.CreateNew(listData.itemId);
                        pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
                    }
                    else
                    {
                        var active = GameManager.Item.GetActiveSkill(listData.itemId);
                        InvenManager.instance.AttackItem.Add(active, InvenType.Storage);
                    }
                    break;
                
                // 3 = weapon
                case 3:
                    // if (!WeaponData.DataLoad.TryGetWeaponData(listData.itemId, out var weaponData)) return;
                    // string weaponName = LanguageManager.Str(weaponData.weaponNameString);
                    if (InvenManager.instance.AttackItem.IsFull(InvenType.Storage))
                    {
                        var pickUp = GameManager.Item.WeaponPickUp.CreateNew(listData.itemId);
                        pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
                    }
                    else
                    {
                        var wp = GameManager.Item.GetWeapon(listData.itemId);
                        InvenManager.instance.AttackItem.Add(wp, InvenType.Storage);
                    }
                    break;
                
                
                
                // 4 = skill tree
                case 4:
                    Debug.Log("이거는 기획쪽에 물어봐야 함. 이거 사면 어떻게 되는지 까먹음.");
                    break;
            }
        }



        // private void ShowShopError(int errorNameInd)
        // {
        //     // TODO
        //     // LanguageManager.Str(errorNameInd);
        // }

        #endregion
        
    }
}