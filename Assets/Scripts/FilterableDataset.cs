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

    public delegate bool FilterExpression(
        TerrariaItemData itemData,
        string queryParameter
    );

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
                filterExpression = (itemData, queryParameter) =>
                    (
                        itemData.name.Contains(queryParameter)
                        || itemData.tooltip.Contains(queryParameter)
                    );
                break;

            case FilterType.ItemTag:
                filterExpression = FilterByTag;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(itemFilterType));
        }

        if (negateFilter)
        {
            filterExpression = (itemData, queryParameter) =>
                !filterExpression(itemData, queryParameter);
        }
    }

    public bool FilterByItemId(TerrariaItemData itemData, string queryParameter)
    {
        return itemData.itemid.ToString().Contains(queryParameter);
    }

    public bool FilterByTag(TerrariaItemData itemData, string queryParameter)
    {
        return itemData.tag.Contains(queryParameter)
            || itemData.listcat.Contains(queryParameter)
            || itemData.type.Contains(queryParameter);
    }
}
