using System;
using System.Threading;
using UnityEngine;

public class SelectedItemListController
    : MonoBehaviour,
        IRecyclableScrollRectDataSource
{
    TerrariaItemDataSource SelectedItems;

    public ITerrariaDictionaryDataSource DataSourceEvents;

    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceChanged;
    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceLoaded;

    // Start is called before the first frame update
    void Start() { }

    private void Awake()
    {
        DataSourceEvents.OnDictionaryDataSourceChanged +=
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

    public void SetContentElementData(
        RecyclableScrollRectContentElement element,
        int index
    )
    {
        throw new NotImplementedException();
    }

    public int GetItemCount()
    {
        throw new NotImplementedException();
    }
}
