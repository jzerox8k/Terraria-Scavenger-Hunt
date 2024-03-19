using System;
using System.Linq;
using System.Threading;
using TerrariaAssets;
using UnityEngine;

public class SelectedItemListController : MonoBehaviour
{
    public TerrariaItemDataSource SelectedItems = new();

    public RecyclableItemGridLayoutGroup RecyclableItemGridLayoutGroup;

    public ItemRandomizerController ItemRandomizerController;

    public TabsController TabsController;

    public void OnSelectedDataSourceChanged(TerrariaItemDictionary dictionary)
    {
        SelectedItems.ItemDictionaryData = dictionary;

        Debug.Log(
            $"Attempting to load RecyclableItemGridLayoutGroup {RecyclableItemGridLayoutGroup} with {SelectedItems.GetItemCount()} items."
        );

        RecyclableItemGridLayoutGroup.LoadDataSource(SelectedItems);
        ItemRandomizerController.OnDataSourceChanged(
            SelectedItems.ItemDictionaryData
        );
    }
}
