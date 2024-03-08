using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TerrariaAssets;

public interface IRecyclableScrollRectContentElement
{

}

public interface IRecyclableScrollRectDataSource
{
    public abstract void SetContentElementData(IRecyclableScrollRectContentElement element, int index);

    public abstract int GetItemCount();
}

public class ItemListElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IRecyclableScrollRectContentElement
{
    public Image itemImage;
    public TerrariaItemData itemData;
    public RectTransform itemRectTransform;
    private bool hovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        Tooltip.ShowTooltip(itemData);
        Tooltip.MoveTooltipToCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        Tooltip.HideTooltip();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (hovered)
        {
            Tooltip.MoveTooltipToCursor();
        }
    }

    public void ConfigureElement(TerrariaItemData itemDataConfiguration)
    {
        this.itemData = itemDataConfiguration;
        itemImage.sprite = itemDataConfiguration.sprite;
        itemImage.SetNativeSize();
    }
}
