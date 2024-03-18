using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecyclableItemGridLayoutGroup : MonoBehaviour
{
    // These objects are to be populated in the constructor.
    public RectTransform ViewportRectTransform;
    public RectTransform ContentRectTransform;
    public GridLayoutGroup ContentGridLayoutGroup;
    public GameObject ItemListElementPrefab;
    public Scrollbar Scrollbar;
    public IRecyclableScrollRectDataSource DataSource;
    public event Action OnRecyclableScrollRectDataSourceLoaded;

    // These objects are internal to the implementation of the RecycleItemGridContent class.
    private List<RecyclableScrollRectContentElement> RecyclableScrollRectItems =
        new List<RecyclableScrollRectContentElement>();

    private Vector2 ViewportDimensions = new Vector2();
    private Vector2Int ViewportDimensionsInCells = new Vector2Int();

    // TODO: We don't need to update the cell dimensions and spacing on every frame update.
    // If we want to make them resizeable in the future we will need to listen to a UI event.
    private Vector2 cellDimensions = new Vector2();
    private Vector2 cellSpacing = new Vector2();

    private (int, int) indexRangeBeingRendered;

    private void Awake()
    {
        Scrollbar.onValueChanged.AddListener(OnScroll);
    }

    public void LoadDataSource(IRecyclableScrollRectDataSource dataSource)
    {
        // Update the datasource reference.
        DataSource = dataSource;

        // When this is complete, any objects awaiting the dataset to complete
        // loading will recive an event trigger.
        StartCoroutine(InitializeScrollRect());
    }

    private IEnumerator InitializeScrollRect()
    {
        // This forces the grid to render once, so any initial formatting
        // occurs first.
        Canvas.ForceUpdateCanvases();

        // Update the grid and cell dimension information so we can calculate
        // viewport-to-grid dimension conversions.
        UpdateGridAndCellDimensions();
        UpdateViewportDimensions();

        // Clear any existing elements from the editor. These will be filled
        // in from the dataset provided.
        foreach (Transform child in ContentRectTransform.transform)
        {
            Destroy(child.gameObject);
        }

        // Destroy takes a frame of time to remove GameObjects.
        yield return new WaitForEndOfFrame();

        RenderScrollRect();

        // Let all objects subscribed to this dataset know that it has
        // finished loading.
        OnRecyclableScrollRectDataSourceLoaded.Invoke();
    }

    private void RenderScrollRect()
    {
        indexRangeBeingRendered = GetFirstAndLastIndicesToRender(
            Scrollbar.value,
            ViewportDimensionsInCells,
            DataSource
        );

        RenderAndPadGridLayout(indexRangeBeingRendered);
    }

    private void Update()
    {
        if (ViewportRectTransform.hasChanged)
        {
            UpdateViewportDimensions();

            ViewportRectTransform.hasChanged = false;

            OnScroll(Scrollbar.value);
        }
    }

    private void OnScroll(float eventData)
    {
        // compare current first index to render with the first index already rendered
        (int, int) indexRangeToRender = GetFirstAndLastIndicesToRender(
            eventData,
            ViewportDimensionsInCells,
            DataSource
        );
        if (indexRangeBeingRendered != indexRangeToRender)
        {
            indexRangeBeingRendered = indexRangeToRender;
            RenderAndPadGridLayout(indexRangeBeingRendered);
        }
    }

    private void UpdateViewportDimensions()
    {
        ViewportDimensions = ViewportRectTransform.rect.size;
        ViewportDimensionsInCells = ConvertRectDimensionsToDimensionsInCells(
            ViewportDimensions,
            cellDimensions,
            cellSpacing
        );
    }

    private void UpdateGridAndCellDimensions()
    {
        cellDimensions = ContentGridLayoutGroup.cellSize;
        cellSpacing = ContentGridLayoutGroup.spacing;
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
            (rectDimensions.x + cellSpacing.x)
                / (cellDimensions.x + cellSpacing.x)
        );
        var viewportHeightInCells = Mathf.FloorToInt(
            (rectDimensions.y + cellSpacing.y)
                / (cellDimensions.y + cellSpacing.y)
        );

        return new Vector2Int(viewportWidthInCells, viewportHeightInCells);
    }

    private (int, int) GetFirstAndLastIndicesToRender(
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
                        Mathf.Ceil(
                            dataSource.GetItemCount()
                                / viewportDimensionsInCells.x
                        ) - viewportDimensionsInCells.y
                    )
            ) * viewportDimensionsInCells.x;
        int lastCellIndex = Mathf.Min(
            dataSource.GetItemCount(),
            firstCellIndex
                + viewportDimensionsInCells.x * viewportDimensionsInCells.y
        );

        int firstCellIndexToRender = Mathf.Max(
            0,
            firstCellIndex - viewportDimensionsInCells.x
        );
        int lastCellIndexToRender = Mathf.Min(
            dataSource.GetItemCount(),
            lastCellIndex + viewportDimensionsInCells.x
        );

        return (firstCellIndexToRender, lastCellIndexToRender);
    }

    private void RenderAndPadGridLayout((int, int) indexRangeBeingRendered)
    {
        // re-render the grid and adjust padding accordingly
        RenderRecycledGrid(
            indexRangeBeingRendered,
            ContentGridLayoutGroup,
            DataSource
        );

        // pad the rows before the first rendered index
        int rowsToPadBefore = GetRowsToPadBeforeFirstIndex(
            indexRangeBeingRendered.Item1,
            ViewportDimensionsInCells.x
        );
        int rowsToPadAfter = GetRowsToPadAfterLastIndex(
            indexRangeBeingRendered.Item2,
            ViewportDimensionsInCells.x,
            DataSource.GetItemCount()
        );

        PadRecycledGrid(
            rowsToPadBefore,
            rowsToPadAfter,
            ContentGridLayoutGroup,
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
                ItemListElementPrefab,
                ContentRectTransform.transform
            );
            RecyclableScrollRectItems.Add(
                gameObject.GetComponentInChildren<RecyclableScrollRectContentElement>()
            );
        }

        if (itemsToRenderCount < RecyclableScrollRectItems.Count)
        {
            foreach (
                var item in RecyclableScrollRectItems.GetRange(
                    itemsToRenderCount,
                    RecyclableScrollRectItems.Count - itemsToRenderCount
                )
            )
            {
                Destroy(item.gameObject);
            }
            RecyclableScrollRectItems.RemoveRange(
                itemsToRenderCount,
                RecyclableScrollRectItems.Count - itemsToRenderCount
            );
        }

        foreach (
            RecyclableScrollRectContentElement element in RecyclableScrollRectItems
        )
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRectTransform);
    }

    private int GetRowsToPadBeforeFirstIndex(int indexFirst, int elementsPerRow)
    {
        return indexFirst / elementsPerRow;
    }

    private int GetRowsToPadAfterLastIndex(
        int indexLast,
        int elementsPerRow,
        int indexMax
    )
    {
        // the ternary modulus for rowMax (i % r == 0 ? 0 : 1) is important to keep the scroll flow smooth at the end of the list
        int rowMax =
            indexMax / elementsPerRow
            + (indexMax % elementsPerRow == 0 ? 0 : 1);

        // the ternary modulus for rowCurrent (i % r == 0 ? 0 : 1) is important to keep the scroll position corrected at the end of the list
        // otherwise, having a non-multiple of elementsPerRow items results in an extra row of padding to be added
        int rowCurrent =
            indexLast / elementsPerRow
            + (indexLast % elementsPerRow == 0 ? 0 : 1);

        return rowMax - rowCurrent;
    }
}
