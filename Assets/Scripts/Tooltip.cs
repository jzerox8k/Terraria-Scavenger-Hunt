using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TerrariaAssets;

public class Tooltip : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemTooltip;
    public static Tooltip Instance;
    public RectTransform infoTabRectTransform;
    public RectTransform canvasRectTransform;
    public Vector3 offset;

    public void Awake()
    {
        Instance = this;
        Instance.gameObject.SetActive(false);
        Instance.offset = new Vector3(20f, -20f);
        //Debug.Log($"canvasRectTransform: {canvasRectTransform.rect.position} {canvasRectTransform.rect.size}");
    }

    public void SetItemInfoTabText(ItemData itemData)
    {
        itemName.text = itemData.name;
        itemTooltip.text = itemData.tooltip;

        ResizeTMP_Text(itemName);
        ResizeTMP_Text(itemTooltip);
    }

    private void ResizeTMP_Text(TMP_Text TMP_Text)
    {
        Vector2 backgroundSize = new Vector2(TMP_Text.preferredWidth, TMP_Text.preferredHeight);
        TMP_Text.rectTransform.sizeDelta = backgroundSize;
    }

    public void ClearItemInfoTabText()
    {
        itemName.text = string.Empty;
        itemTooltip.text = string.Empty;

        ResizeTMP_Text(itemName);
        ResizeTMP_Text(itemTooltip);
    }

    private void _ShowTooltip(ItemData itemData)
    {
        SetItemInfoTabText(itemData);
        Debug.Log($"{itemName.text} is hovered");
        gameObject.SetActive(true);
    }

    private void _HideTooltip()
    {
        Debug.Log($"{itemName.text} is no longer hovered");
        ClearItemInfoTabText();
        gameObject.SetActive(false);
    }

    private void _MoveTooltipToCursor()
    {
        Canvas.ForceUpdateCanvases();

        /* this implementation if for the overlay canvas */
        //Vector3 targetPosition = (Input.mousePosition / canvasRectTransform.localScale.x) + offset + new Vector3(0, -infoTabRectTransform.rect.height);
        Vector3 targetPosition = (Input.mousePosition) + offset + new Vector3(0, -infoTabRectTransform.rect.height);

        Instance.infoTabRectTransform.anchoredPosition = targetPosition;

        float outOfBoundsX = Instance.infoTabRectTransform.anchoredPosition.x + Instance.infoTabRectTransform.rect.width - canvasRectTransform.rect.width;
        float outOfBoundsY = Instance.infoTabRectTransform.anchoredPosition.y;

        //Debug.Log($"out of bounds: ({outOfBoundsX}, {outOfBoundsY})");

        if (outOfBoundsX > 0)
        {
            targetPosition.x -= outOfBoundsX; 
        }
        if (outOfBoundsY < 0)
        {
            targetPosition.y = 0;
        }

        Instance.infoTabRectTransform.anchoredPosition = targetPosition;

    }

    public static void ShowTooltip(ItemData itemData)
    {
        Instance._ShowTooltip(itemData);
    }

    public static void HideTooltip()
    {
        Instance._HideTooltip();
    }

    public static void MoveTooltipToCursor()
    {
        Instance._MoveTooltipToCursor();
    }
}
