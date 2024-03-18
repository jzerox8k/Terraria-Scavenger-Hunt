using System;
using System.Collections.Generic;
using System.Linq;
using TerrariaAssets;
using UnityEngine;

public class TerrariaItemDictionary : Dictionary<int, TerrariaItemData>
{
    public TerrariaItemDictionary(TerrariaItemDictionary keyValuePairs)
        : base(keyValuePairs) { }

    public TerrariaItemDictionary()
        : base() { }
}

public class TerrariaItemDataSource : IRecyclableScrollRectDataSource
{
    private TerrariaItemDictionary _itemDictionaryData = new();

    public TerrariaItemDictionary ItemDictionaryData
    {
        get => _itemDictionaryData;
        set
        {
            Debug.Log(
                $"Assignment to a TerrariaItemDictionary is invoking OnDataSourceChanged"
            );
            _itemDictionaryData = value;
            OnDataSourceChanged.Invoke(_itemDictionaryData);
        }
    }

    public TerrariaItemDataSource(TerrariaItemDataSource terrariaItemDataSource)
    {
        _itemDictionaryData = new(terrariaItemDataSource._itemDictionaryData);
    }

    public TerrariaItemDataSource()
    {
        _itemDictionaryData = new();
    }

    /// <summary>
    /// An event action for external objects to subscribe to.
    /// </summary>
    /// <remarks>
    /// Used to indicate that the datasource has changed. It supplies the dictionary data that was changed.
    /// </remarks>
    public event Action<TerrariaItemDictionary> OnDataSourceChanged;

    public int GetItemCount()
    {
        return ItemDictionaryData.Count;
    }

    public void SetContentElementData(
        RecyclableScrollRectContentElement element,
        int index
    )
    {
        ItemListElement itemListElement = element as ItemListElement;
        itemListElement.ConfigureElement(
            ItemDictionaryData.Values.ToList()[index]
        );
    }
}
