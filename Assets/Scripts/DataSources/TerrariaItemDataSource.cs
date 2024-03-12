using System;
using System.Collections.Generic;
using System.Linq;
using TerrariaAssets;
using UnityEngine;

/// <summary>
/// A class used to implement various events relating to <see cref="TerrariaItemData"/> data sources.
/// </summary>
public class TerrariaItemDataSource
    : MonoBehaviour,
        ITerrariaDictionaryDataSource,
        IRecyclableScrollRectDataSource
{
    public Dictionary<int, TerrariaItemData> Data { get; set; }

    public int GetItemCount()
    {
        return Data.Count;
    }

    public void SetContentElementData(
        RecyclableScrollRectContentElement element,
        int index
    )
    {
        ItemListElement itemListElement = element as ItemListElement;
        itemListElement.ConfigureElement(Data.Values.ToList()[index]);
    }

    public TerrariaItemDataSource(Dictionary<int, TerrariaItemData> data)
    {
        Data = data;
    }

    public TerrariaItemDataSource()
    {
        Data = new();
    }

    public void InvokeOnDictionaryDataSourceChanged(
        ITerrariaDictionaryDataSource.EventArguments arguments
    )
    {
        OnDictionaryDataSourceChanged.Invoke(arguments);
    }

    public void InvokeOnDictionaryDataSourceLoaded(
        ITerrariaDictionaryDataSource.EventArguments arguments
    )
    {
        OnDictionaryDataSourceLoaded.Invoke(arguments);
    }

    public void InvokeOnDataSourceChanged(
        IRecyclableScrollRectDataSource.EventArguments arguments
    )
    {
        OnDataSourceChanged(arguments);
    }

    public void InvokeOnDataSourceLoaded(
        IRecyclableScrollRectDataSource.EventArguments arguments
    )
    {
        OnDataSourceLoaded(arguments);
    }

    private void Awake() { }
}
