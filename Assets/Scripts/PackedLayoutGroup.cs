using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class PackedLayoutGroup : LayoutGroup
{
    // TODO: see if we can adjust child alignment
    // TODO: add padding

    public int rows;

    public bool childControlWidth;

    public float spacing;

    private float _preferredHeight;
    private float _preferredWidth;
    private float _minHeight;
    private float _minWidth;

    public override float preferredWidth { get { return _preferredWidth; } }
    public override float preferredHeight { get { return _preferredHeight; } }
    public override float minWidth { get { return _minWidth; } }
    public override float minHeight { get { return _minHeight; } }

    public void StartNewPackedRow(PackedRow packedRow)
    {
        // TODO: separate between currentRowMaxPreferredHeight and currentRowMaxMinHeight
        packedRow._totalMinHeight += packedRow.currentRowMaxHeight + spacing;
        packedRow._totalPreferredHeight += packedRow.currentRowMaxHeight + spacing;

        packedRow.currentRow++;
        packedRow.currentRowWidth = 0f;
        packedRow.currentHeightFromTop += packedRow.currentRowMaxHeight + spacing;
        packedRow.currentRowMaxHeight = 0f;
        packedRow.isRowEmpty = true;
    }

    public void MoveAlongExistingPackedRow(PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        packedRow.currentRowWidth += layoutElement.width + spacing;
        packedRow.isRowEmpty = false;
    }

    public void PlacePackedLayoutElement(RectTransform transform, PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        SetChildAlongAxis(transform, 0, packedRow.currentPosition.x, layoutElement.prefferedWidth);
        SetChildAlongAxis(transform, 1, packedRow.currentPosition.y, layoutElement.preferredHeight);

        packedRow.currentRowMaxHeight = Mathf.Max(packedRow.currentRowMaxHeight, layoutElement.preferredHeight);
        packedRow.isRowEmpty = false;

        packedRow._maxPreferredWidth = Mathf.Max(layoutElement.prefferedWidth, packedRow._maxPreferredWidth);
        packedRow._maxMinWidth = Mathf.Max(layoutElement.minWidth, packedRow._maxMinWidth);
    }

    public void ResizeAndPlacePackedLayoutElement(RectTransform transform, PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        if (childControlWidth)
        {
            layoutElement.width = Mathf.Max(layoutElement.minWidth, Mathf.Min(layoutElement.prefferedWidth, packedRow._parentWidth));
        }

        /// height resizing is a bit tricky
        /// it will depend on a number of factors for each layout element in a row
        /// for now just allow all elements their preferred height

        SetChildAlongAxis(transform, 0, packedRow.currentPosition.x, layoutElement.width);
        SetChildAlongAxis(transform, 1, packedRow.currentPosition.y, layoutElement.preferredHeight);

        packedRow._maxPreferredWidth = Mathf.Max(layoutElement.prefferedWidth, packedRow._maxPreferredWidth);
        packedRow._maxMinWidth = Mathf.Max(layoutElement.minWidth, packedRow._maxMinWidth);

        packedRow.currentRowMaxHeight = Mathf.Max(packedRow.currentRowMaxHeight, layoutElement.preferredHeight);
        packedRow.isRowEmpty = false;
    }

    public bool IsPackedLayoutElementOversizedForRow(PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        return packedRow.currenrRowRemainingWidth < layoutElement.prefferedWidth;
    }

    public PackedLayoutElement GetPackedLayoutElement(RectTransform child)
    {
        PackedLayoutElement layoutElement = new PackedLayoutElement();

        layoutElement.width = layoutElement.prefferedWidth = LayoutUtility.GetPreferredSize(child, 0);
        layoutElement.minWidth = LayoutUtility.GetMinSize(child, 0);

        layoutElement.height = layoutElement.preferredHeight = LayoutUtility.GetPreferredSize(child, 1);
        layoutElement.minHeight = LayoutUtility.GetMinSize(child, 1);

        return layoutElement;
    }

    public class PackedRow {
        public int currentRow = 0;

        public Vector2 currentPosition { get { return new Vector2(currentRowWidth, currentHeightFromTop); } }
        public float currentRowWidth = 0f;
        public float currentHeightFromTop = 0f;
         
        public float currenrRowRemainingWidth { get { return _parentWidth - currentRowWidth; } }
        public float currentRowMaxHeight = 0f;
         
        public bool isRowEmpty = true;

        public float _parentWidth;

        // TODO: when inserting elements into a row update the _maxPreferredWidth and _maxMinWidth in the whole layout
        public float _maxPreferredWidth;
        public float _maxMinWidth;

        public float _totalPreferredHeight;
        public float _totalMinHeight;
    }

    public class PackedLayoutElement
    {
        public float prefferedWidth = 0f;
        public float preferredHeight = 0f;
        public float minWidth = 0f;
        public float minHeight = 0f;
        public float width = 0f;
        public float height = 0f;
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal(); // allocates the children of this group that have active layout elements

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        PackedRow packedRow = new PackedRow();
        packedRow._parentWidth = parentWidth;

        foreach (RectTransform childRectTransform in rectChildren)
        {
            PackedLayoutElement layoutElement = GetPackedLayoutElement(childRectTransform);

            /// if (not oversized) 
            /// {
            ///     place-normal()
            ///     keep-row()
            ///     return
            /// }
            /// else if (oversized) 
            /// {
            ///     if (rowIsEmpty) 
            ///     {
            ///         place-resized()
            ///         new-row()
            ///         return
            ///     }
            ///     else if (not rowIsEmpty) 
            ///     {
            ///         new-row()
            ///         if (not oversized) 
            ///         {
            ///             place-normal()
            ///             keep-row()
            ///             return
            ///         }
            ///         else if (oversized) 
            ///         {
            ///             place-resized()
            ///             new-row()
            ///             return
            ///         }
            ///     }
            /// }
            /// 

            if (!IsPackedLayoutElementOversizedForRow(packedRow, layoutElement))
            {
                PlacePackedLayoutElement(childRectTransform, packedRow, layoutElement);
                MoveAlongExistingPackedRow(packedRow, layoutElement);
                continue;
            }
            else
            {
                if (packedRow.isRowEmpty)
                {
                    ResizeAndPlacePackedLayoutElement(childRectTransform, packedRow, layoutElement);
                    StartNewPackedRow(packedRow);
                    continue;
                }
                else
                {
                    StartNewPackedRow(packedRow);
                    if (!IsPackedLayoutElementOversizedForRow(packedRow, layoutElement))
                    {
                        PlacePackedLayoutElement(childRectTransform, packedRow, layoutElement);
                        MoveAlongExistingPackedRow(packedRow, layoutElement);
                        continue;
                    }
                    else
                    {
                        ResizeAndPlacePackedLayoutElement(childRectTransform, packedRow, layoutElement);
                        StartNewPackedRow(packedRow);
                        continue;
                    }
                }
            }
        }
    
        if (!packedRow.isRowEmpty)
        {
            StartNewPackedRow(packedRow);
        }

        _preferredHeight = packedRow.currentHeightFromTop - spacing;
        _preferredWidth = packedRow._maxPreferredWidth;
        _minHeight = preferredHeight;
        _minWidth = packedRow._maxMinWidth;
    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}