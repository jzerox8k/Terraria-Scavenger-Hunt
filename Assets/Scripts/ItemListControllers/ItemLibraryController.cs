using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TerrariaAssets;
using UnityEngine;

public class ItemLibraryController : MonoBehaviour
{
    public Transform content;
    public TextAsset itemDataFile;

    public TerrariaItemDataSource itemLibraryDataSource;

    public TerrariaItemDataSource selectedItemDataSource;

    // Add references to all subscribers here.
    // That way, when the script is initialized, this script can set up event
    // subscriptions right away.

    public SelectedItemListController selectedItemListController;

    public void Awake()
    {
        // Set the selected item list controller as a subscriber to events pertaining to changes to the selected items.
        /// TODO: Selection changes (filters, item IDs, tags, etc.) will happen by changing the
        /// selected items in <see cref="selectedItemListController"/>, either in this script,
        /// or in a separate script that referneces this one.
        selectedItemDataSource.OnDataSourceChanged +=
            selectedItemListController.OnSelectedDataSourceChanged;
    }

    private void Start()
    {
        // Initialize this object's data source.
        itemLibraryDataSource = new();

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

        foreach (TerrariaItemData itemData in itemJsonData.Values)
        {
            // Retrieve the image for the item from the assets and insert it into the image dataset.
            string path = "Image Data/" + itemData.imagefile.Split(".")[0];
            Sprite sprite = Resources.Load<Sprite>(path);
            itemJsonData[itemData.itemid].sprite = sprite;

            // Insert the item list element into the library.
            itemLibraryDataSource.ItemDictionaryData.Add(
                itemData.itemid,
                itemData
            );
        }

        Debug.Log(
            $"{itemLibraryDataSource.ItemDictionaryData.Count} items found after removing unobtainable items"
        );

        Debug.Log($"About to invoke OnDataSourceLoaded event Action");

        // The assignment should trigger the event!
        selectedItemDataSource = new(itemLibraryDataSource);
    }
}
