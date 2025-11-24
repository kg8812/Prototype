using Apis.DataType;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConvenienceSub : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Button button;

    public ConvenienceDataType Data { get; private set; }
    public void Init(ConvenienceDataType data)
    {
        Data = data;
        nameText.text = data.name;
        costText.text = data.priceWarmth.ToString();
        button.onClick.RemoveAllListeners();
    }

    public void Purchase()
    {
        ConvenienceFunctions.Dict[Data.index].ForEach(x => x.Apply());
        DataAccess.LobbyData.UnLock(Data.index);
    }
}
