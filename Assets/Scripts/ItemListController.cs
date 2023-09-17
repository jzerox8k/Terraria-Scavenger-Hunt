using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TerrariaAssets;
using Newtonsoft.Json;
using System;
using System.Linq;

using PolyAndCode.UI;

public class ItemListController : MonoBehaviour
{
    public Transform content;
    public TextAsset itemDataFile;

    public static event Action OnDatasetLoaded;

    // Start is called before the first frame update
    void Start()
    {
        Verify();

        // get item data from assets
        Dictionary<int, ItemData> itemJsonData = JsonConvert.DeserializeObject<Dictionary<int, ItemData>>(itemDataFile.text);

        Debug.Log(itemDataFile.text);
        Debug.Log($"{itemJsonData.Count} items found and parsed");

        // remove unobtainable items
        itemJsonData = itemJsonData.Where(x => !(x.Value.unobtainable ?? false)).ToDictionary(p => p.Key, p => p.Value);

        Debug.Log($"{itemJsonData.Count} items found after removing unobtainable items");

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData itemData in itemJsonData.Values)
        {
            /* OLD IMPLEMENTATION */
            /*
            // create a new item in the content game object
            GameObject newGameObject = Instantiate(itemPrefab.gameObject, content);
            */

            // retrieve the image for the item from the assets and insert it into the image dataset
            string path = "Image Data/" + itemData.imagefile.Split(".")[0];
            Sprite sprite = Resources.Load<Sprite>(path);
            itemJsonData[itemData.itemid].sprite = sprite;

            /* OLD IMPLEMENTATION */
            /*
            // update the new child item of the content game object
            ItemListElement elem = newGameObject.GetComponent<ItemListElement>();

            elem.itemData = itemData;
            elem.itemImage.sprite = sprite;
            elem.itemImage.SetNativeSize();
            */

            /* NEW IMPLEMENTATION */
            // insert the item list element into the item dataset
            ItemDataset.Instance.Items.Add(itemData.itemid, itemData);

            /*
            if (itemData.itemid == 250)
            {
                break;
            }
            */
        }

        // update the randomizer controller
        OnDatasetLoaded.Invoke();
    }

    private void Verify()
    {
        if (content == null)
        {
            content = transform.Find("Content");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
