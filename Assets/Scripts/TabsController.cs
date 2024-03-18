using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsController : MonoBehaviour
{
    public GameObject library;
    public GameObject filters;
    public GameObject selected;

    public ItemRandomizerController ItemRandomizerController;

    private void Awake() { }

    private void Start()
    {
        ItemRandomizerController.OnDataSourceLoaded +=
            OnRandomizerDataSourceLoaded;
    }

    private void OnRandomizerDataSourceLoaded()
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
