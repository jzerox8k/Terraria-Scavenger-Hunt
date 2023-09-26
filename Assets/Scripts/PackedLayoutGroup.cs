using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class PackedLayoutGroup : HorizontalOrVerticalLayoutGroup
{
    public int rows;

    public void StartNewPackedRow(PackedRow packedRow)
    {
        packedRow.currentRow++;
        packedRow.currentRowWidth = 0f;
        packedRow.currentHeightFromTop += packedRow.currentRowMaxHeight + spacing;
        packedRow.currentRowMaxHeight = 0f;
        packedRow.firstChildInRow = true;
    }

    public void MoveAlongExistingPackedRow(PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        packedRow.currentRowWidth += layoutElement.width + spacing;
        packedRow.firstChildInRow = false;
    }

    public void PlacePackedLayoutElement(RectTransform transform, PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        SetChildAlongAxis(transform, 0, packedRow.currentPosition.x, layoutElement.prefferedWidth);
        SetChildAlongAxis(transform, 1, packedRow.currentPosition.y, layoutElement.preferredHeight);

        packedRow.currentRowMaxHeight = Mathf.Max(packedRow.currentRowMaxHeight, layoutElement.preferredHeight);
        packedRow.firstChildInRow = false;
    }

    public void PlaceResizedPackedLayoutElement(RectTransform transform, PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        if (childControlWidth)
        {
            layoutElement.width = Mathf.Max(layoutElement.minWidth, Mathf.Min(layoutElement.prefferedWidth, packedRow.parentWidth));
        }

        /// height resizing is a bit tricky
        /// it will depend on a number of factors for each layout element in a row
        /// for now just allow all elements their preferred height

        SetChildAlongAxis(transform, 0, packedRow.currentPosition.x, layoutElement.width);
        SetChildAlongAxis(transform, 1, packedRow.currentPosition.y, layoutElement.preferredHeight);

        packedRow.currentRowMaxHeight = Mathf.Max(packedRow.currentRowMaxHeight, layoutElement.preferredHeight);
        packedRow.firstChildInRow = false;
    }

    public bool IsPackedLayoutElementOversizedForRow(PackedRow packedRow, PackedLayoutElement layoutElement)
    {
        return packedRow.currenrRowRemainingWidth < layoutElement.prefferedWidth;
    }

    public class PackedRow {
        public int currentRow = 0;

        public float currentRowWidth = 0f;
        public float currentHeightFromTop = 0f;
        public Vector2 currentPosition { get { return new Vector2(currentRowWidth, currentHeightFromTop); } }
         
        public float currentRowMaxHeight = 0f;
        public float currenrRowRemainingWidth { get { return parentWidth - currentRowWidth; } }
         
        public bool firstChildInRow = true;

        public float parentWidth;
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
        base.CalculateLayoutInputHorizontal();

        var x = childControlHeight;
        var y = childControlWidth;

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        PackedRow packedRow = new PackedRow();
        packedRow.parentWidth = parentWidth;

        foreach (RectTransform childRectTransform in rectChildren)
        {
            PackedLayoutElement layoutElement = new PackedLayoutElement();

            ILayoutElement currentLayoutElement;
            if (childRectTransform.TryGetComponent(out currentLayoutElement))
            {
                layoutElement.width = layoutElement.prefferedWidth = currentLayoutElement.preferredWidth;
                layoutElement.minWidth = currentLayoutElement.minWidth;

                layoutElement.height = layoutElement.preferredHeight = currentLayoutElement.preferredHeight;
                layoutElement.minHeight = currentLayoutElement.minHeight;
            }
            else
            {
                layoutElement.width = layoutElement.prefferedWidth = childRectTransform.rect.width;
                layoutElement.height = layoutElement.preferredHeight = childRectTransform.rect.height;
            }

            /// if (not oversized) 
            /// {
            ///     place-normal()
            ///     keep-row()
            ///     return
            /// }
            /// else if (oversized) 
            /// {
            ///     if (first-element-in-row) 
            ///     {
            ///         place-resized()
            ///         new-row()
            ///         return
            ///     }
            ///     else if (not first-element-in-row) 
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
                if (packedRow.firstChildInRow)
                {
                    PlaceResizedPackedLayoutElement(childRectTransform, packedRow, layoutElement);
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
                        PlaceResizedPackedLayoutElement(childRectTransform, packedRow, layoutElement);
                        StartNewPackedRow(packedRow);
                        continue;
                    }
                }
            }
        }
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