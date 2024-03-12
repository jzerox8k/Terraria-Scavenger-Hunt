using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TerrariaAssets;

public abstract class RecyclableScrollRectContentElement : MonoBehaviour
{
    public abstract void ConfigureElement(IElementConfiguration elementConfiguration); 
}

public interface IElementConfiguration
{

}

public interface IRecyclableScrollRectDataSource
{
    public abstract void SetContentElementData(RecyclableScrollRectContentElement element, int index);

    public abstract int GetItemCount();
}

public class ItemListElement : 
    RecyclableScrollRectContentElement,
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerMoveHandler
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

    public override void ConfigureElement(IElementConfiguration itemDataConfiguration)
    {
        if (itemDataConfiguration is TerrariaItemData item)
        {
            this.itemData = item;
            itemImage.sprite = item.sprite;
            itemImage.SetNativeSize();
        }
    }
}
