using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaAssets;

public interface ITerrariaDictionaryDataSource
    : IDictionaryDataSource<int, TerrariaItemData> { }

/// <summary>
/// A class used to implement various events relating to <see cref="TerrariaItemData"/> data sources.
/// </summary>
public class TerrariaItemDataSource
    : ITerrariaDictionaryDataSource,
        IRecyclableScrollRectDataSource
{
    public Dictionary<int, TerrariaItemData> Data { get; set; }

    public event Action<ITerrariaDictionaryDataSource.EventArguments> OnDictionaryDataSourceChanged;
    public event Action<ITerrariaDictionaryDataSource.EventArguments> OnDictionaryDataSourceLoaded;
    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceChanged;
    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceLoaded;

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
}
