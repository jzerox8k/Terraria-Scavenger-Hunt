using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaAssets;

public class FilterableItemData : TerrariaItemData
{
    public bool isFiltered { get; set; }

    public FilterableItemData(TerrariaItemData itemData) : base(itemData)
    {
        isFiltered = true;
    }
}
