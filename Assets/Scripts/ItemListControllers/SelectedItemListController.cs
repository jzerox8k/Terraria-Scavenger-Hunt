using System;
using System.Linq;
using System.Threading;
using TerrariaAssets;
using UnityEngine;

public class SelectedItemListController : MonoBehaviour
{
    public TerrariaItemDataSource SelectedItems;

    public ItemLibraryController ItemLibraryController;

    public RecyclableItemGridLayoutGroup RecyclableItemGridLayoutGroup;

    private void Awake() { }

    public void OnSelectedDataSourceChanged(TerrariaItemDictionary dictionary)
    {
        SelectedItems.ItemDictionaryData = dictionary;
    }

    public int GetItemCount()
    {
        return SelectedItems.ItemDictionaryData.Count;
    }

    public void SetContentElementData(
        RecyclableScrollRectContentElement element,
        int index
    )
    {
        ItemListElement itemListElement = element as ItemListElement;
        itemListElement.ConfigureElement(
            SelectedItems.ItemDictionaryData.Values.ToList()[index]
        );
    }
}
