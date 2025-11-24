using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_CheatItemPage : MonoBehaviour
{
    int currentPage;
    int maxPage;

    public GameObject pageItemsParent;
    public Button leftArrow;
    public Button rightArrow;
    public TextMeshProUGUI pageText;
    public Button backButton;
    
    List<Item> wholeItemList = new();
    private readonly CustomQueue<Button> pageItems = new();

    private CheatManager2 manager;
    
    protected virtual void Awake()
    {
        leftArrow.onClick.RemoveAllListeners();
        leftArrow.onClick.AddListener(() =>
        {
            if (maxPage <= 1) return;
            SetPage(currentPage - 1);
        });
        rightArrow.onClick.RemoveAllListeners();
        rightArrow.onClick.AddListener(() =>
        {
            if (maxPage <= 1) return;
            SetPage(currentPage + 1);
        });
    }

    protected virtual void OnEnable()
    {
        SetWholeItems();
        currentPage = 1;
        SetPage(currentPage);
    }
    
    void SetWholeItems()
    {
        wholeItemList = GetWholeItemList();
        maxPage = wholeItemList.Count / 12 + 1;
    }
    
    protected abstract List<Item> GetWholeItemList();

    void SetPage(int page)
    {
        if (page > maxPage)
        {
            currentPage = 1;
        }
        else if (page < 1)
        {
            currentPage = maxPage;
        }
        else
        {
            currentPage = page;
        }

        pageText.text = $"{currentPage}/{maxPage}";
        SetPageItems();
    }

    void SetPageItems()
    {
        int remains = Mathf.Clamp(wholeItemList.Count - (currentPage - 1) * 12, 0, wholeItemList.Count);

        int itemShowCount = Mathf.Clamp(remains, 0, 12);

        while (itemShowCount > pageItems.Count)
        {
            pageItems.Enqueue(GameManager.UI.MakeUI<Button>("BaseButton",pageItemsParent.transform));
        }

        while (itemShowCount < pageItems.Count)
        {
            GameManager.UI.ReturnUI(pageItems.Dequeue().gameObject);
        }

        for (int i = 0; i < itemShowCount; i++)
        {
            int temp = (currentPage - 1) * 12 + i;
            pageItems[i].GetComponentInChildren<TextMeshProUGUI>().text = StrUtil.GetEquipmentName(wholeItemList[temp].ItemId);
            pageItems[i].onClick.RemoveAllListeners();
            pageItems[i].onClick.AddListener(() => AddItem(wholeItemList[temp]));
        }
    }

    protected abstract void AddItem(Item item);
}
