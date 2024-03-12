using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaAssets;

internal interface ITerrariaItemListController
{
    public Dictionary<int, TerrariaItemData> itemDictionary { get; set; }
}
