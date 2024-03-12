using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrariaAssets;
using System;

public class SelectedItemListController : MonoBehaviour
{
    public static Dictionary<int, TerrariaItemData> SelectedItems = new Dictionary<int, TerrariaItemData>();

    public static event Action OnSelectedItemsChanged;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        ItemLibraryController.OnDatasetLoaded += OnDatasetLoaded;
    }

    private void OnDatasetLoaded()
    {
        SelectedItems = new(TerrariaItemDataset.Instance.Items);
        OnSelectedItemsChanged.Invoke();
    }

    public void ReplaceSelectedItems(Dictionary<int, TerrariaItemData> newSelectedItems)
    {
        SelectedItems = new(newSelectedItems);
        OnSelectedItemsChanged.Invoke();
    }
}
