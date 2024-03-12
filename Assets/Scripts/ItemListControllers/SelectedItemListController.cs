using System;
using System.Linq;
using System.Threading;
using TerrariaAssets;
using UnityEngine;

public class SelectedItemListController
    : MonoBehaviour,
        IRecyclableScrollRectDataSource
{
    [SerializeField]
    TerrariaItemDataSource SelectedItems;

    [SerializeField]
    public TerrariaItemDataSource ItemLibraryDataSource;

    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceChanged;
    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceLoaded;

    // Start is called before the first frame update
    void Start() { }

    private void Awake()
    {
        Debug.Log(
            "About to subscribe to ItemLibraryDataSource.OnDictionaryDataSourceLoaded."
        );

        Debug.Log(
            $"ItemLibraryDataSource: {(ItemLibraryDataSource ? ItemLibraryDataSource.name : "null")}"
        );

        ItemLibraryDataSource.OnDictionaryDataSourceLoaded +=
            OnSelectedDataSourceLoaded;
    }

    private void OnSelectedDataSourceLoaded(
        ITerrariaDictionaryDataSource.EventArguments arguments
    )
    {
        SelectedItems = new(arguments.DataSource.Data);
        OnDataSourceLoaded.Invoke(new(SelectedItems));
    }

    public void OnSelectedDataSourceChanged(
        ITerrariaDictionaryDataSource.EventArguments arguments
    )
    {
        SelectedItems = new(arguments.DataSource.Data);
        OnDataSourceChanged.Invoke(new(SelectedItems));
    }

    public int GetItemCount()
    {
        return SelectedItems.Data.Count;
    }

    public void SetContentElementData(
        RecyclableScrollRectContentElement element,
        int index
    )
    {
        ItemListElement itemListElement = element as ItemListElement;
        itemListElement.ConfigureElement(
            SelectedItems.Data.Values.ToList()[index]
        );
    }
}
