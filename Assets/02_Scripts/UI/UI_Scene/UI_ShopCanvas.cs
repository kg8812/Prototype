// using Default;
// using Apis;
// using Apis.DataType;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
//
// public class UI_ShopCanvas : UI_Scene
// {
//     #region ControlSection
//
//     private int index = 0;
//
//     private bool isSlotControl;
//     
//     
//     public override void Control()
//     {
//         if (isSlotControl)
//         {
//             shop.Control();
//             if (InputManager.GetKeyDown(KeyCode.D))
//             {
//                 MoveFocusToInven();
//                 if (index == 0)
//                 {
//                     wpInven.Select(0, 0);
//                 }
//                 else
//                 {
//                     accInven.Select(0, 0);
//                 }
//             }
//         }
//         else
//         {
//             if (index == 0)
//             {
//                 // wpInven.focusParent.Control();
//             }
//             else
//             {
//                 // accInven.focusParent.Control();
//             }
//         }
//         if (InputManager.GetKeyDown(KeyCode.Q) || InputManager.GetKeyDown(KeyCode.E))
//         {
//             if (index == 0)
//             {
//                 accInven.On();
//                 wpInven.Off();
//                 index = 1;
//                 if(!isSlotControl)
//                     accInven.Select(0, 0);
//             }
//             else
//             {
//                 index = 0;
//                 accInven.Off();
//                 wpInven.On();
//                 if(!isSlotControl)
//                     wpInven.Select(0, 0);
//             }
//                     
//         }
//         base.Control();
//     }
//
//
//
//     public void MoveFocusToShopSlot()
//     {
//         if (isSlotControl) return;
//         isSlotControl = true;
//         if (index == 0)
//         {
//             // wpInven.focusParent.FocusReset();
//         }
//         else
//         {
//             // accInven.focusParent.FocusReset();
//         }
//     }
//
//     public void MoveFocusToInven()
//     {
//         if (!isSlotControl) return;
//         isSlotControl = false;
//         shop.slotFocusParent.FocusReset();
//         
//     }
//
//     
//     
//     private void WhenInvenMoveLeft(int ind)
//     {
//         Debug.LogError($"when inven move left {ind}");
//         MoveFocusToShopSlot();
//         shop.slotFocusParent.MoveTo(shop.slotFocusParent.curId);
//     }
//     
//     
//     #endregion
//
//     
//     enum Buttons
//     {
//         CloseButton
//     }
//
//     enum GameObjects
//     {
//         Slots,BuyButton,
//     }
//     
//
//     enum Texts
//     {
//         Title,SoulText
//     }
//
//     public ShopInven accInven;
//     public ShopInven wpInven;
//     private ShopDataType shopData;
//     private GameObject slots;
//     private TextMeshProUGUI title;
//     private TextMeshProUGUI soulText;
//
//     private GameObject buyButton;
//     private Shop shop;
//     public override void Init()
//     {
//         base.Init();
//         Bind<Button>(typeof(Buttons));
//         Bind<TextMeshProUGUI>(typeof(Texts));
//         Bind<GameObject>(typeof(GameObjects));
//         Get<Button>((int)Buttons.CloseButton).onClick.AddListener(() => GameManager.UI.CloseUI(this));
//         soulText = Get<TextMeshProUGUI>((int)Texts.SoulText);
//         slots = Get<GameObject>((int)GameObjects.Slots);
//         accInven.Init(InvenType.Inven);
//         wpInven.Init(InvenType.Inven);
//         shop = GetComponentInChildren<Shop>(true);
//         buyButton = Get<GameObject>((int)GameObjects.BuyButton);
//         subItems.Add(shop);
//         
//     }
//
//
//     public void Init(int id)
//     {
//         if (!ShopData.TryGetShopData(id, out shopData)) return;
//         title = Get<TextMeshProUGUI>((int)Texts.Title);
//         title.text = GameManager.Data.Str(shopData.shopName) ?? "상점";
//         shop.Init(shopData);
//         // wpInven.focusParent.tableData.moveLeft = WhenInvenMoveLeft;
//         // accInven.focusParent.tableData.moveLeft = WhenInvenMoveLeft;
//         Debug.Log($"move left add listener {shop.slotFocusParent.tableData.moveLeft != null}");
//         isSlotControl = true;
//         shop.slotFocusParent.MoveTo(2);
//         shop.OnBuy.AddListener(_ => RefreshInven());
//         shop.OnBuy.AddListener(_ => buyButton.SetActive(false));
//         shop.OnBuy.AddListener(item =>
//         {
//             switch (item.Type)
//             {
//                 case ItemType.Accessory:
//                     accInven.On();
//                     wpInven.Off();
//                     index = 1;
//                     break;
//                 case ItemType.Weapon:
//                     wpInven.On();
//                     accInven.Off();
//                     index = 0;
//                     break;
//             }
//         });
//     }
//     
//     private void Update()
//     {
//         soulText.text = GameManager.instance.Soul.ToString();
//     }
//     
//
//     public override void TryActivated(bool force = false)
//     {
//         accInven.Clear();
//         wpInven.Clear();
//         index = 0;
//         
//         RefreshInven();
//         wpInven.On();
//         accInven.Off();
//         buyButton.SetActive(false);
//         base.TryActivated(force);
//     }
//
//     void RefreshInven()
//     {
//         accInven.Clear();
//         wpInven.Clear();
//
//         int count = 0;
//         foreach (var x in InvenManager.instance.Acc.Invens[InvenType.Inven].Slots)
//         {
//             accInven.AddItem(x.Item, count);
//             count++;
//         }
//
//         count = 0;
//         foreach (var x in InvenManager.instance.Wp.Invens[InvenType.Inven].Slots)
//         {
//             wpInven.AddItem(x.Item, count);
//             count++;
//         }
//     }
//     protected override void Deactivated()
//     {
//         base.Deactivated();
//         accInven.Clear();
//         wpInven.Clear();
//     }
// }