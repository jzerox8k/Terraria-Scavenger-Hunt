using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TerrariaAssets;
using System.Linq;
using UnityEditor;


public class RecycleItemGridContent : MonoBehaviour
{
    public RectTransform viewportRectTransform;
    public RectTransform contentRectTransform;
    public GridLayoutGroup contentGridLayoutGroup;
    public GameObject itemListElementPrefab;
    public Scrollbar scrollbar;

    public List<ItemFilterElement> items = new List<ItemFilterElement>();

    public Vector2 viewportDimensions = new Vector2();
    public Vector2 contentDimensions = new Vector2();
    public Vector2Int viewportDimensionsInCells = new Vector2Int();

    // TODO: we don't need to update the cell dimensions and spacing on every frame update
    // but if we want to make them resizeable in the future we will need to listen to a UI event 
    public Vector2 cellDimensions = new Vector2();
    public Vector2 cellSpacing = new Vector2();

    public (int, int) indexRangeBeingRendered;

    private void Awake()
    {
        Debug.Log($"subscribing to event ItemListController.OnDatasetLoaded");

        ItemListController.OnDatasetLoaded += OnDatasetLoaded;
    }

    // yield return new WaitForEndOfFrame();

    private void OnDatasetLoaded()
    {
        StartCoroutine(AwaitContentChildDestruction());
    }

    IEnumerator AwaitContentChildDestruction()
    {
        Debug.Log($"test OnDatasetLoaded method invoked");

        Canvas.ForceUpdateCanvases();

        // update the grid and cell dimension information so we can calculate viewport grid conversions
        UpdateGridAndCellDimensions();
        UpdateViewportDimensions();

        scrollbar.onValueChanged.AddListener(OnScroll);

        Debug.Log($"{contentRectTransform.childCount} items in contentRectTransform");

        foreach (Transform child in contentRectTransform.transform)
        {
            Destroy(child.gameObject);
        }

        yield return new WaitForEndOfFrame();

        Debug.Log($"{contentRectTransform.childCount} items in contentRectTransform");

        indexRangeBeingRendered = GetFirstAndLastIndicesToRender(scrollbar.value, viewportDimensionsInCells, ItemDataset.Instance);

        RenderAndPadGridLayout(indexRangeBeingRendered);
    }


    private void Update()
    {
        if (viewportRectTransform.hasChanged)
        {
            print($"viewportRectTransform has changed");
            UpdateViewportDimensions();
            UpdateContentRectDimensions(ItemDataset.Instance);
            viewportRectTransform.hasChanged = false;
        }
    }

    private void UpdateContentRectDimensions(IRecyclableScrollRectDataSource dataSource)
    {
        float totalHeight = Mathf.CeilToInt(dataSource.GetItemCount() / viewportDimensionsInCells.x) * (cellDimensions.y + cellSpacing.y);
        Debug.Log($"totalHeight: {totalHeight}");
    }

    private void UpdateViewportDimensions()
    {
        viewportDimensions = viewportRectTransform.rect.size;
        viewportDimensionsInCells = ConvertRectDimensionsToDimensionsInCells(viewportDimensions, cellDimensions, cellSpacing);
        Debug.Log($"viewportDimensions: {viewportDimensions}");
        Debug.Log($"viewportDimensionsInCells: {viewportDimensionsInCells}");
    }

    private void UpdateGridAndCellDimensions()
    {
        cellDimensions = contentGridLayoutGroup.cellSize;
        cellSpacing = contentGridLayoutGroup.spacing;
        Debug.Log($"cellDimensions: {cellDimensions}");
        Debug.Log($"cellSpacing: {cellSpacing}");
    }

    private Vector2Int ConvertRectDimensionsToDimensionsInCells(Vector2 rectDimensions, Vector2 cellDimensions, Vector2 cellSpacing)
    {
        Debug.Log($"rectDimensions: {rectDimensions}");
        Debug.Log($"cellDimensions: {cellDimensions}");
        Debug.Log($"cellSpacing: {cellSpacing}");

        /* length = n cells and n-1 spaces between them
         * l = n*x + (n-1)*s
         * solve for n:
         * l = n*x + n*s - s
         * l + s = n*(x + s)
         * (l + s)/(x + s) = n 
         */

        var viewportWidthInCells = Mathf.FloorToInt((rectDimensions.x + cellSpacing.x) / (cellDimensions.x + cellSpacing.x));
        var viewportHeightInCells = Mathf.FloorToInt((rectDimensions.y + cellSpacing.y) / (cellDimensions.y + cellSpacing.y));

        return new Vector2Int(viewportWidthInCells, viewportHeightInCells);
    }

    public (int, int) GetFirstAndLastIndicesToRender(float scrollbarValue, Vector2Int viewportDimensionsInCells, IRecyclableScrollRectDataSource dataSource)
    {
        float correctedScrollbarValue = 1 - scrollbarValue;

        int firstCellIndex = Mathf.FloorToInt(correctedScrollbarValue * (Mathf.Ceil(dataSource.GetItemCount() / viewportDimensionsInCells.x) - viewportDimensionsInCells.y)) * viewportDimensionsInCells.x;
        int lastCellIndex = Mathf.Min(dataSource.GetItemCount(), firstCellIndex + viewportDimensionsInCells.x * viewportDimensionsInCells.y);

        int firstCellIndexToRender = Mathf.Max(0, firstCellIndex - viewportDimensionsInCells.x);
        int lastCellIndexToRender = Mathf.Min(dataSource.GetItemCount(), lastCellIndex + viewportDimensionsInCells.x);

        return (firstCellIndexToRender, lastCellIndexToRender);
    }

    public void OnScroll(float eventData)
    {
        //Debug.Log($"eventData: {eventData}");
        //Debug.Log($"rect.position: {contentRectTransform.anchoredPosition}");

        // compare current first index to render with the first index already rendered
        (int, int) indexRangeToRender = GetFirstAndLastIndicesToRender(eventData, viewportDimensionsInCells, ItemDataset.Instance);
        //Debug.Log($"indexRangeToRender: {indexRangeToRender}");

        // if they are different
        if (indexRangeBeingRendered != indexRangeToRender)
        {
            indexRangeBeingRendered = indexRangeToRender;
            RenderAndPadGridLayout(indexRangeBeingRendered);
        }
    }

    private void RenderAndPadGridLayout((int, int) indexRangeBeingRendered)
    {
        // re-render the grid and adjust padding accordingly
        Debug.Log($"indexRangeBeingRendered: {indexRangeBeingRendered}");
        RenderRecycledGrid(indexRangeBeingRendered, contentGridLayoutGroup, ItemDataset.Instance);

        // pad the rows before the first rendered index
        int rowsToPadBefore = GetRowsToPadBeforeFirstIndex(indexRangeBeingRendered.Item1, viewportDimensionsInCells.x);
        int rowsToPadAfter = GetRowsToPadAfterLastIndex(indexRangeBeingRendered.Item2, viewportDimensionsInCells.x, ItemDataset.Instance.GetItemCount());

        Debug.Log($"rowsToPadBefore: {rowsToPadBefore}");
        Debug.Log($"rowsToPadAfter: {rowsToPadAfter}");

        PadRecycledGrid(rowsToPadBefore, rowsToPadAfter, contentGridLayoutGroup, cellDimensions, cellSpacing);
    }

    private void RenderRecycledGrid((int, int) indexRangeBeingRendered, GridLayoutGroup contentGridLayoutGroup, IRecyclableScrollRectDataSource dataSource)
    {
        int indexToRender = indexRangeBeingRendered.Item1;
        int lastIndexToRender = indexRangeBeingRendered.Item2;

        int itemsToRenderCount = lastIndexToRender - indexToRender;
        Debug.Log($"itemsToRenderCount: {itemsToRenderCount}, contentGridLayoutGroup.transform.childCount, {contentGridLayoutGroup.transform.childCount}");

        while (itemsToRenderCount > contentGridLayoutGroup.transform.childCount)
        {
            GameObject gameObject = Instantiate(itemListElementPrefab, contentRectTransform.transform);
            items.Add(gameObject.GetComponent<ItemFilterElement>());
            Debug.Log($"itemsToRenderCount: {itemsToRenderCount}, contentGridLayoutGroup.transform.childCount, {contentGridLayoutGroup.transform.childCount}");
        }

        foreach (ItemFilterElement element in items)
        {
            if (indexToRender < lastIndexToRender)
            { 
                if (!element.enabled)
                {
                    element.enabled = true;
                    element.gameObject.SetActive(true);
                }
                dataSource.SetContentElementData(element, indexToRender);
                indexToRender++;
            }
            else
            {
                if (!element.enabled)
                {
                    element.enabled = false;
                    element.gameObject.SetActive(false);
                }
            }
        }
    }

    private void PadRecycledGrid(int rowsToPadBeforeFirstIndex, int rowsToPadAfterLastIndex, GridLayoutGroup contentGridLayoutGroup, Vector2 cellDimensions, Vector2 cellSpacing)
    {
        int paddingTop = Mathf.RoundToInt(rowsToPadBeforeFirstIndex * (cellDimensions.y + cellSpacing.y));
        int paddingBottom = Mathf.RoundToInt(rowsToPadAfterLastIndex * (cellDimensions.y + cellSpacing.y));

        contentGridLayoutGroup.padding.top = paddingTop;
        contentGridLayoutGroup.padding.bottom = paddingBottom;

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
    }

    private int GetRowsToPadBeforeFirstIndex(int indexFirst, int elementsPerRow)
    {
        return indexFirst / elementsPerRow;
    }

    private int GetRowsToPadAfterLastIndex(int indexLast, int elementsPerRow, int indexMax)
    {
        int rowMax = indexMax / elementsPerRow;
        int rowCurrent = indexLast / elementsPerRow;
        return rowMax - rowCurrent;
    }
}
