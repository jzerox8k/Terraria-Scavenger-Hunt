using System;
using System.Collections.Generic;
using System.Linq;
using TerrariaAssets;
using UnityEngine;
using UnityEngine.UI;

public class ItemRandomizerController : MonoBehaviour
{
    public Button randomize;
    public Button shuffle;

    /// <summary>
    /// The selected items data source controller.
    /// </summary>
    public SelectedItemListController SelectedItemList;

    /// <summary>
    /// The local reference of the items that the randomizer can pull from.
    /// </summary>
    TerrariaItemDictionary RandomizerItemlist;

    /// <summary>
    /// The item data that will populate the 5x5 grid.
    /// </summary>
    List<TerrariaItemData> itemData;

    /// <summary>
    /// The game objects that compose th 5x5 grid.
    /// </summary>
    List<ItemGridElement> itemGridElements;

    public event Action OnDataSourceLoaded;

    public SeedDisplay seedDisplay;

    public const int MaxItems = 25;
    private const string AlphaNumericCipher =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+";

    void Awake() { }

    void Start()
    {
        itemGridElements = new List<ItemGridElement>();
        itemData = new List<TerrariaItemData>();
        SelectedItemList.SelectedItems.OnDataSourceChanged +=
            OnDataSourceChanged;

        foreach (Transform child in transform)
        {
            GameObject childImageGameObject = child.Find("Image").gameObject;
            ItemGridElement itemGridElement =
                child.GetComponentInChildren<ItemGridElement>();
            itemGridElement.itemImage =
                childImageGameObject.GetComponent<Image>();
            itemGridElements.Add(itemGridElement);
        }

        Debug.Log($"{itemGridElements.Count} grid elements found");
    }

    private void OnDataSourceChanged(TerrariaItemDictionary dictionary)
    {
        RandomizerItemlist = dictionary;
        OnDataSourceLoaded.Invoke();
        RandomizeItems();
    }

    public void RandomizeItems()
    {
        List<int> randomItemIds = RandomizerItemlist.Keys.ToList();

        itemData.Clear();
        for (int i = 0; i < MaxItems; i++)
        {
            int r = UnityEngine.Random.Range(0, randomItemIds.Count);
            int itemid = randomItemIds[r];
            itemData.Add(RandomizerItemlist[itemid]);
            randomItemIds.Remove(itemid);
        }

        DisplayItems();
    }

    public void ShuffleItems()
    {
        List<TerrariaItemData> items = itemData.ConvertAll(x => x);
        itemData.Clear();

        for (int i = 0; i < MaxItems; i++)
        {
            int r = UnityEngine.Random.Range(0, items.Count);
            itemData.Add(items[r]);
            items.RemoveAt(r);
        }

        DisplayItems();
    }

    private void DisplayItems()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            ItemGridElement elem = itemGridElements[i];
            TerrariaItemData data = itemData[i];
            elem.itemData = data;
            elem.itemImage.sprite = RandomizerItemlist[data.itemid].sprite;
            elem.itemImage.SetNativeSize();
        }

        /*
        foreach (ItemGridElement gridElement in itemGridElements)
        {
            Debug.Log($"{gridElement.itemData.name}");
        }
        */

        seedDisplay.seedTextValue.text = GenerateSeedFromItemList(itemData);
    }

    public string GenerateSeedFromItemList(List<TerrariaItemData> itemList)
    {
        string seed = "";
        foreach (TerrariaItemData item in itemList)
        {
            string itemSeedString = ConvertItemIdToAlphaNumCode(item.itemid);
            //Debug.Log($"itemSeedString: {itemSeedString}");
            seed += $"{itemSeedString}";
        }
        return seed;
    }

    public string ConvertItemIdToAlphaNumCode(int itemid)
    {
        int idValue = itemid;
        string itemSeedString = "";

        //Debug.Log($"itemid: {itemid}");

        while (idValue > 0)
        {
            int r = idValue % AlphaNumericCipher.Length;
            idValue /= AlphaNumericCipher.Length;

            //Debug.Log($"q: {idValue}, r: {r}");

            itemSeedString = $"{AlphaNumericCipher[r]}" + itemSeedString;
        }

        if (itemSeedString.Length == 1)
        {
            itemSeedString = "0" + itemSeedString;
        }

        return itemSeedString;
    }

    public int ConvertAlphaNumCodeToItemID(string alphaNumCode)
    {
        int idValue = 0;

        foreach (char c in alphaNumCode)
        {
            int i = AlphaNumericCipher.IndexOf(c);
            //Debug.Log($"c: {c}, i: {i}");

            if (i == -1)
            {
                return -1;
            }
            idValue = idValue * AlphaNumericCipher.Length + i;
        }

        return idValue;
    }

    public List<int> ParseSeed(string seed)
    {
        //Debug.Log($"seed.Length: {seed.Length}");
        if (seed.Length != 2 * MaxItems)
            return null;

        List<string> list = new List<string>();
        List<int> result = new List<int>();

        for (int i = 0; i < MaxItems; i++)
        {
            string s = seed.Substring(i * 2, 2);
            list.Add(s);
        }

        foreach (string s in list)
        {
            int id = ConvertAlphaNumCodeToItemID(s);
            if (id < 1 || id > 5455)
                return null;
            result.Add(id);
            //Debug.Log($"s: {s}, id: {id}");
            //Debug.Log("");
        }

        if (result.Count != MaxItems)
            return null;

        return result;
    }

    public bool ValidateSeed(string seed)
    {
        List<int> result = ParseSeed(seed);

        if (result == null)
            return false;

        for (int i = 0; i < MaxItems; i++)
        {
            itemData[i] = RandomizerItemlist[result[i]];
        }

        DisplayItems();

        return true;
    }
}
