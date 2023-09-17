using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TerrariaAssets;

public class RecycleItemGridContent : MonoBehaviour
{
    public RectTransform viewportRectTransform;
    public RectTransform contentRectTransform;
    public GridLayoutGroup contentGridLayoutGroup;
    public GameObject itemListElementPrefab;
    public ScrollRect scrollRect;
    public Scrollbar scrollbar;

    public List<ItemListElement> items = new List<ItemListElement>();

    public float viewportHeight;
    public float contentHeight;

    private void Start()
    {
        ItemListController.OnDatasetLoaded += OnDatasetLoaded;
    }

    private void OnDatasetLoaded()
    {
        SetScrollRectParams();
        scrollbar.onValueChanged.AddListener(OnScroll);
    }

    private void SetScrollRectParams()
    {
        /*
        {
            Debug.Log($"viewportRectTransform: ");
            Debug.Log($"rect.height: {viewportRectTransform.rect.height}");
            Debug.Log($"rect.position: {viewportRectTransform.rect.position}");

            Debug.Log($"contentRectTransform: ");
            Debug.Log($"rect.height: {contentRectTransform.rect.height}");
            Debug.Log($"rect.position: {contentRectTransform.rect.position}");

            contentRectTransform.ForceUpdateRectTransforms();

            var x1 = contentGridLayoutGroup.cellSize;
            var x2 = contentGridLayoutGroup.spacing;
            var x3 = contentGridLayoutGroup.constraintCount;
            var x4 = contentGridLayoutGroup.Size();

            Debug.Log($"cellSize: {x1}");
            Debug.Log($"spacing: {x2}");
            Debug.Log($"constraintCount: {x3}");
            Debug.Log($"Size: {x4}");
        }
        */

        foreach (ItemData itemData in ItemDataset.Instance.Items.Values)
        {
            GameObject gameObject = Instantiate(itemListElementPrefab, contentRectTransform);
            ItemListElement elem = gameObject.GetComponent<ItemListElement>();
            elem.ConfigureCell(itemData);
        }
        Debug.Log($"{contentRectTransform.childCount} items in contentRectTransform");

        Canvas.ForceUpdateCanvases();

        var x1 = contentRectTransform.childCount;
        var rectWidth = contentRectTransform.rect.width + contentGridLayoutGroup.spacing.x;
        var rectHeight = contentRectTransform.rect.height + contentGridLayoutGroup.spacing.y;

        var viewportHeight = viewportRectTransform.rect.height;

        var cellWidthWithSpacing = contentGridLayoutGroup.cellSize.x + contentGridLayoutGroup.spacing.x;
        var cellHeightWithSpacing = contentGridLayoutGroup.cellSize.y + contentGridLayoutGroup.spacing.y;

        Debug.Log($"rectWidth/cellWidth: {rectWidth/cellWidthWithSpacing}");
        Debug.Log($"rectHeight/cellHeightWithSpacing: {rectHeight/cellHeightWithSpacing}");

        Debug.Log($"viewportHeight/cellHeightWithSpacing: {viewportHeight/cellHeightWithSpacing}");

        // we need viewportHeight/cellHeightWithSpacing + 2
        // 1 row ahead and 1 row behind
        var maxRowsInViewport = Mathf.RoundToInt(viewportHeight / cellHeightWithSpacing) + 2;

        Debug.Log($"{contentGridLayoutGroup.Size()}");
    }

    public void OnScroll(float eventData)
    {
        Debug.Log($"eventData: {eventData}");
        Debug.Log($"rect.position: {contentRectTransform.anchoredPosition}");
    }
}
