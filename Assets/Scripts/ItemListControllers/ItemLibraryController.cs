using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TerrariaAssets;
using UnityEngine;

public class ItemLibraryController
    : MonoBehaviour,
        IRecyclableScrollRectDataSource.Events
{
    public Transform content;
    public TextAsset itemDataFile;
    public ITerrariaItemDataSource itemLibraryDataSource;

    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceChanged;
    public event Action<IRecyclableScrollRectDataSource.EventArguments> OnDataSourceLoaded;

    // Start is called before the first frame update.
    private void Start()
    {
        // Get the content transform if it isn't assigned from the editor.
        if (content == null)
        {
            content = transform.Find("Content");
        }

        Debug.Log(itemDataFile.text);

        // Get item data from file assets.
        Dictionary<int, TerrariaItemData> itemJsonData =
            JsonConvert.DeserializeObject<Dictionary<int, TerrariaItemData>>(
                itemDataFile.text
            );

        Debug.Log($"{itemJsonData.Count} items found and parsed");

        // Remove unobtainable items.
        itemJsonData = itemJsonData
            .Where(x => !(x.Value.unobtainable ?? false))
            .ToDictionary(p => p.Key, p => p.Value);

        Debug.Log(
            $"{itemJsonData.Count} items found after removing unobtainable items"
        );

        foreach (TerrariaItemData itemData in itemJsonData.Values)
        {
            // Retrieve the image for the item from the assets and insert it into the image dataset.
            string path = "Image Data/" + itemData.imagefile.Split(".")[0];
            Sprite sprite = Resources.Load<Sprite>(path);
            itemJsonData[itemData.itemid].sprite = sprite;

            // Insert the item list element into the library.
            itemLibraryDataSource.Data.Add(itemData.itemid, itemData);
        }

        Debug.Log($"about to invoke OnDatasetLoaded event Action");

        OnDataSourceLoaded.Invoke(new(itemLibraryDataSource));
    }
}
