using Apis.UI.Focus;
using NewNewInvenSpace;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class UITab_Accesary : UITab_Inventory
    {
        protected override InventoryGroup invenGroupManager => InvenManager.instance.Acc;
    }
}