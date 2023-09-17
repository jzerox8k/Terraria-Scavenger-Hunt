using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TerrariaAssets;

using PolyAndCode.UI;

public class ItemListElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Image itemImage;
    public ItemData itemData;
    public RectTransform itemRectTransform;
    private bool hovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        Tooltip.ShowTooltip(itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip();
        hovered = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (hovered)
        {
            Tooltip.MoveTooltipToCursor();
        }
    }

    public void ConfigureCell(ItemData _itemData)
    {
        itemData = _itemData;
        itemImage.sprite = _itemData.sprite;
        itemImage.SetNativeSize();
    }
}
