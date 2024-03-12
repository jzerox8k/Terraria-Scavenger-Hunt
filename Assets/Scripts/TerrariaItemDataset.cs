using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyAndCode.UI;

namespace TerrariaAssets
{
    public class TerrariaItemDataset : IRecyclableScrollRectDataSource
    {
        public static TerrariaItemDataset Instance = new TerrariaItemDataset();
        public Dictionary<int, TerrariaItemData> Items = new Dictionary<int, TerrariaItemData>();

        public int GetItemCount()
        {
            return Instance.Items.Count;
        }

        public void SetContentElementData(RecyclableScrollRectContentElement element, int index)
        {
            ItemListElement itemListElement = element as ItemListElement;
            itemListElement.ConfigureElement(Instance.Items.Values.ToList()[index]);
        }
    }
}
