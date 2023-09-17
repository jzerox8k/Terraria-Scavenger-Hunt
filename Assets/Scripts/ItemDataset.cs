using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyAndCode.UI;

namespace TerrariaAssets
{
    public class ItemDataset
    {
        public static ItemDataset Instance = new ItemDataset();
        public Dictionary<int, ItemData> Items = new Dictionary<int, ItemData>();

        public int GetItemCount()
        {
            return Instance.Items.Count;
        }

        public void SetCell(ItemListElement itemListElement, int index)
        {
            itemListElement.ConfigureCell(Instance.Items.Values.ToList()[index]);
        }
    }
}
