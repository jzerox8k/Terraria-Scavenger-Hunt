using System;
using TerrariaAssets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IElementConfiguration { }

public abstract class RecyclableScrollRectContentElement : MonoBehaviour
{
    public abstract void ConfigureElement(
        IElementConfiguration elementConfiguration
    );
}

public interface IRecyclableScrollRectDataSource
{
    public abstract void SetContentElementData(
        RecyclableScrollRectContentElement element,
        int index
    );

    public abstract int GetItemCount();

    /// <summary>
    /// A class uses to transfer data source event information beween <see cref="RecyclableItemGridLayoutGroup"/>s and the outside world.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An object that emits these events should derive from this class and implement overrides for the events.
    /// </para>
    /// <para>
    /// A <see cref="RecyclableItemGridLayoutGroup"/> that subscribes to these events should have an internal member that references the object that will
    /// emit these events.
    /// </para>
    /// </remarks>
    public class EventArguments
    {
        public IRecyclableScrollRectDataSource DataSource { get; set; }

        public EventArguments(IRecyclableScrollRectDataSource dataSource)
        {
            DataSource = dataSource;
        }
    }

    /// <summary>
    /// Used to inform subscribers that the data source they are subscribed to has changed.
    /// </summary>
    public event Action<EventArguments> OnDataSourceChanged;

    /// <summary>
    /// Used to inform subscribers that the data source they are subscribed to has been loaded.
    /// </summary>
    public event Action<EventArguments> OnDataSourceLoaded;
}

public class ItemListElement
    : RecyclableScrollRectContentElement,
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

    public override void ConfigureElement(
        IElementConfiguration itemDataConfiguration
    )
    {
        if (itemDataConfiguration is TerrariaItemData item)
        {
            this.itemData = item;
            itemImage.sprite = item.sprite;
            itemImage.SetNativeSize();
        }
    }
}
