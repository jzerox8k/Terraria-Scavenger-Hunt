using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaAssets;

public class ItemFilter
{
    /* ENUMERATIONS AND DELEGATE DECLARATIONS */
    public enum FilterType
    {
        ItemId,
        ItemName,
        ItemNameOrTooltip,
        ItemTag,
    }

    public delegate bool FilterExpression(ItemData itemData, string queryParameter);

    /* FIELDS AND PROPERTIES */
    public FilterType filterType;
    public FilterExpression filterExpression;

    public ItemFilter(FilterType itemFilterType, bool negateFilter = false)
    {
        filterType = itemFilterType;

        switch (itemFilterType)
        {
            case FilterType.ItemId:
                filterExpression = FilterByItemId;
                break;

            case FilterType.ItemName:
                filterExpression = (itemData, queryParameter) =>
                itemData.name.Contains(queryParameter);
                break;

            case FilterType.ItemNameOrTooltip:
                filterExpression = (itemData, queryParameter) => (
                itemData.name.Contains(queryParameter) ||
                itemData.tooltip.Contains(queryParameter));
                break;

            case FilterType.ItemTag:
                filterExpression = FilterByTag;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(itemFilterType));
        }

        if (negateFilter)
        {
            filterExpression = (itemData, queryParameter) => !filterExpression(itemData, queryParameter);
        }
    }

    public bool FilterByItemId(ItemData itemData, string queryParameter)
    {
        return itemData.itemid.ToString().Contains(queryParameter);
    }

    public bool FilterByTag(ItemData itemData, string queryParameter)
    {
        return 
            itemData.tag.Contains(queryParameter) ||
            itemData.listcat.Contains(queryParameter) ||
            itemData.type.Contains(queryParameter);
    }
}

public class FilterableDataset : IRecyclableScrollRectDataSource
{
    public Dictionary<int, FilterableItemData> ItemDatabase = new Dictionary<int, FilterableItemData>();
    public Dictionary<int, FilterableItemData> filteredItemDataset = new Dictionary<int, FilterableItemData>();

    ItemFilter itemIdFilter = new ItemFilter(ItemFilter.FilterType.ItemId);
    ItemFilter itemNameFilter = new ItemFilter(ItemFilter.FilterType.ItemName);
    ItemFilter itemNameOrTooltipFilter = new ItemFilter(ItemFilter.FilterType.ItemNameOrTooltip);
    List<ItemFilter> itemTagFilterList = new List<ItemFilter>();

    ItemFilter.FilterType currentFilterType;

    string searchParameter;

    public FilterableDataset(ItemDataset itemDataset)
    {
        ItemDatabase = itemDataset.Items.ToDictionary(item => item.Key, item => new FilterableItemData(item.Value));
        filteredItemDataset = ItemDatabase;
    }

    public int GetItemCount()
    {
        return filteredItemDataset.Where(x => x.Value.isFiltered).Count();
    }

    public void SetContentElementData(IRecyclableScrollRectContentElement element, int index)
    {
        ItemListElement itemListElement = element as ItemListElement;
        itemListElement.ConfigureElement(filteredItemDataset.Values.ToList()[index]);
    }

    public void ApplyFilters()
    {
        switch (currentFilterType)
        {
            case ItemFilter.FilterType.ItemId:
                filteredItemDataset = ApplyFilter(ItemDatabase, itemIdFilter);                
                break;
            case ItemFilter.FilterType.ItemName:
                filteredItemDataset = ApplyFilter(ItemDatabase, itemNameFilter);
                break;
            case ItemFilter.FilterType.ItemNameOrTooltip:
                filteredItemDataset = ApplyFilter(ItemDatabase, itemNameOrTooltipFilter);
                break;
            case ItemFilter.FilterType.ItemTag:
                filteredItemDataset = ItemDatabase;
                foreach (ItemFilter itemTagFilter in itemTagFilterList)
                {
                    filteredItemDataset = ApplyFilter(filteredItemDataset, itemTagFilter);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentFilterType));
        }
    }

    public Dictionary<int, FilterableItemData> ApplyFilter(Dictionary<int, FilterableItemData> dataset, ItemFilter itemFilter)
    {
        return dataset
            .Where(p => itemFilter.filterExpression(p.Value, searchParameter))
            .ToDictionary(p => p.Key, p => p.Value);
    }
}
