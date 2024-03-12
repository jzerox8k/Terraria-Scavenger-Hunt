using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerrariaAssets;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecycleItemGridContent : MonoBehaviour
{
    public RectTransform viewportRectTransform;
    public RectTransform contentRectTransform;
    public GridLayoutGroup contentGridLayoutGroup;
    public GameObject itemListElementPrefab;
    public Scrollbar scrollbar;

    public List<RecyclableScrollRectContentElement> items =
        new List<RecyclableScrollRectContentElement>();

    public IRecyclableScrollRectDataSource datasource;

    public Vector2 viewportDimensions = new Vector2();
    public Vector2 contentDimensions = new Vector2();
    public Vector2Int viewportDimensionsInCells = new Vector2Int();

    // TODO: The TabsController sets this script is attached to inactive.
    // We need better event handling to make sure the grid is populated before this object is deactivated.

    // TODO: Rework the dataset loading events across all classes that use them.

    // TODO: we don't need to update the cell dimensions and spacing on every frame update
    // but if we want to make them resizeable in the future we will need to listen to a UI event
    public Vector2 cellDimensions = new Vector2();
    public Vector2 cellSpacing = new Vector2();

    public (int, int) indexRangeBeingRendered;

    private void Awake()
    {
        SelectedItemListController.OnSelectedItemsChanged += OnDatasetLoaded;
    }

    private void OnDatasetLoaded()
    {
        StartCoroutine(DestroyContentChildrenAndInitializeScrollRect());
    }

    IEnumerator DestroyContentChildrenAndInitializeScrollRect()
    {
        Canvas.ForceUpdateCanvases();

        // update the grid and cell dimension information so we can calculate viewport grid conversions
        UpdateGridAndCellDimensions();
        UpdateViewportDimensions();

        foreach (Transform child in contentRectTransform.transform)
        {
            Destroy(child.gameObject);
        }

        yield return new WaitForEndOfFrame();

        // update the dataset items
        datasource = new FilterableDataset(TerrariaItemDataset.Instance);

        indexRangeBeingRendered = GetFirstAndLastIndicesToRender(
            scrollbar.value,
            viewportDimensionsInCells,
            datasource
        );

        RenderAndPadGridLayout(indexRangeBeingRendered);

        scrollbar.onValueChanged.AddListener(OnScroll);
    }

    private void Update()
    {
        if (viewportRectTransform.hasChanged)
        {
            UpdateViewportDimensions();

            viewportRectTransform.hasChanged = false;

            OnScroll(scrollbar.value);
        }
    }

    public void OnScroll(float eventData)
    {
        // compare current first index to render with the first index already rendered
        (int, int) indexRangeToRender = GetFirstAndLastIndicesToRender(
            eventData,
            viewportDimensionsInCells,
            datasource
        );
        if (indexRangeBeingRendered != indexRangeToRender)
        {
            indexRangeBeingRendered = indexRangeToRender;
            RenderAndPadGridLayout(indexRangeBeingRendered);
        }
    }

    private void UpdateViewportDimensions()
    {
        viewportDimensions = viewportRectTransform.rect.size;
        viewportDimensionsInCells = ConvertRectDimensionsToDimensionsInCells(
            viewportDimensions,
            cellDimensions,
            cellSpacing
        );
    }

    private void UpdateGridAndCellDimensions()
    {
        cellDimensions = contentGridLayoutGroup.cellSize;
        cellSpacing = contentGridLayoutGroup.spacing;
    }

    private Vector2Int ConvertRectDimensionsToDimensionsInCells(
        Vector2 rectDimensions,
        Vector2 cellDimensions,
        Vector2 cellSpacing
    )
    {
        // length = n cells and n-1 spaces between them
        // l = n*x + (n-1)*s
        // solve for n:
        // l = n*x + n*s - s
        // l + s = n*(x + s)
        // (l + s)/(x + s) = n

        var viewportWidthInCells = Mathf.FloorToInt(
            (rectDimensions.x + cellSpacing.x) / (cellDimensions.x + cellSpacing.x)
        );
        var viewportHeightInCells = Mathf.FloorToInt(
            (rectDimensions.y + cellSpacing.y) / (cellDimensions.y + cellSpacing.y)
        );

        return new Vector2Int(viewportWidthInCells, viewportHeightInCells);
    }

    public (int, int) GetFirstAndLastIndicesToRender(
        float scrollbarValue,
        Vector2Int viewportDimensionsInCells,
        IRecyclableScrollRectDataSource dataSource
    )
    {
        float correctedScrollbarValue = 1 - scrollbarValue;

        int firstCellIndex =
            Mathf.CeilToInt(
                correctedScrollbarValue
                    * (
                        Mathf.Ceil(dataSource.GetItemCount() / viewportDimensionsInCells.x)
                        - viewportDimensionsInCells.y
                    )
            ) * viewportDimensionsInCells.x;
        int lastCellIndex = Mathf.Min(
            dataSource.GetItemCount(),
            firstCellIndex + viewportDimensionsInCells.x * viewportDimensionsInCells.y
        );

        int firstCellIndexToRender = Mathf.Max(0, firstCellIndex - viewportDimensionsInCells.x);
        int lastCellIndexToRender = Mathf.Min(
            dataSource.GetItemCount(),
            lastCellIndex + viewportDimensionsInCells.x
        );

        return (firstCellIndexToRender, lastCellIndexToRender);
    }

    private void RenderAndPadGridLayout((int, int) indexRangeBeingRendered)
    {
        // re-render the grid and adjust padding accordingly
        RenderRecycledGrid(indexRangeBeingRendered, contentGridLayoutGroup, datasource);

        // pad the rows before the first rendered index
        int rowsToPadBefore = GetRowsToPadBeforeFirstIndex(
            indexRangeBeingRendered.Item1,
            viewportDimensionsInCells.x
        );
        int rowsToPadAfter = GetRowsToPadAfterLastIndex(
            indexRangeBeingRendered.Item2,
            viewportDimensionsInCells.x,
            datasource.GetItemCount()
        );

        PadRecycledGrid(
            rowsToPadBefore,
            rowsToPadAfter,
            contentGridLayoutGroup,
            cellDimensions,
            cellSpacing
        );
    }

    private void RenderRecycledGrid(
        (int, int) indexRangeBeingRendered,
        GridLayoutGroup contentGridLayoutGroup,
        IRecyclableScrollRectDataSource dataSource
    )
    {
        int indexToRender = indexRangeBeingRendered.Item1;
        int lastIndexToRender = indexRangeBeingRendered.Item2;

        int itemsToRenderCount = lastIndexToRender - indexToRender;

        while (itemsToRenderCount > contentGridLayoutGroup.transform.childCount)
        {
            GameObject gameObject = Instantiate(
                itemListElementPrefab,
                contentRectTransform.transform
            );
            items.Add(gameObject.GetComponentInChildren<RecyclableScrollRectContentElement>());
        }

        if (itemsToRenderCount < items.Count)
        {
            foreach (
                var item in items.GetRange(itemsToRenderCount, items.Count - itemsToRenderCount)
            )
            {
                Destroy(item.gameObject);
            }
            items.RemoveRange(itemsToRenderCount, items.Count - itemsToRenderCount);
        }

        foreach (RecyclableScrollRectContentElement element in items)
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

    private void PadRecycledGrid(
        int rowsToPadBeforeFirstIndex,
        int rowsToPadAfterLastIndex,
        GridLayoutGroup contentGridLayoutGroup,
        Vector2 cellDimensions,
        Vector2 cellSpacing
    )
    {
        int paddingTop = Mathf.RoundToInt(
            rowsToPadBeforeFirstIndex * (cellDimensions.y + cellSpacing.y)
        );
        int paddingBottom = Mathf.RoundToInt(
            rowsToPadAfterLastIndex * (cellDimensions.y + cellSpacing.y)
        );

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
        // the ternary modulus for rowMax (i % r == 0 ? 0 : 1) is important to keep the scroll flow smooth at the end of the list
        int rowMax = indexMax / elementsPerRow + (indexMax % elementsPerRow == 0 ? 0 : 1);

        // the ternary modulus for rowCurrent (i % r == 0 ? 0 : 1) is important to keep the scroll position corrected at the end of the list
        // otherwise, having a non-multiple of elementsPerRow items results in an extra row of padding to be added
        int rowCurrent = indexLast / elementsPerRow + (indexLast % elementsPerRow == 0 ? 0 : 1);

        return rowMax - rowCurrent;
    }
}
