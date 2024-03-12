using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsController : MonoBehaviour
{
    public GameObject library;
    public GameObject filters;
    public GameObject selected;

    [SerializeField]
    SelectedItemListController selectedItemListController;

    private void Awake()
    {
        selectedItemListController.OnDataSourceLoaded += OnDatasetLoaded;
    }

    private void OnDatasetLoaded(
        IRecyclableScrollRectDataSource.EventArguments arguments
    )
    {
        OpenLibrary();
    }

    public void OpenLibrary()
    {
        library.SetActive(true);
        filters.SetActive(true);

        selected.SetActive(false);
    }

    public void OpenSelected()
    {
        library.SetActive(false);
        filters.SetActive(false);

        selected.SetActive(true);
    }
}
