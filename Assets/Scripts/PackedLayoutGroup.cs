using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PackedLayoutGroup : MonoBehaviour
{
    // TODO: we might want to implement a packed layout using simpler Unity UI components
    // it may be more effective to implement the MonoBehaviour class and use that as a base
    // and build the PackedLayoutGroup script using the other layout groups or simpler code
    // rather than implementing a whole new class inheriting from LayoutGroup

    // TODO: we want avoid implementations that use other layout groups
    // if possible we should force the layout through the use of the code alone
    // the implementation rewrite should occur in a separate class

    // TODO: the current implementation works but has a few noticeable flaws
    // - it is laggy during a window resize
    // - it requires a very specific configuration of layout elements in the scene
    //   - it specifically requires a workaround to limit the parent from stretching too far 
    //     using a large negative right padding on the parent
    // - it is not very stable when removing or adding items in bulk
    // - the inspector is constantly emitting warnings about undefined behaviour
    //   when using content size fitters on the children of layout groups
    // the stability of the build with adding or removing multiple items is a drawback 
    // of the current implementation using existing layout groups...
    // a much better implementation would be to custom fit the objects
    // under a parent transform and update it accordingly

    // INFO: a link with information on Unity Layout Groups and their interactions
    // with Content Size Fitters can be found here:
    // https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/HOWTO-UIFitContentSize.html#make-children-of-a-layout-group-fit-their-respective-sizes

    public RectTransform contentRectTransform;
    public RectTransform widthLimitRectTransform;
    List<Transform> contentRectTransformList = new List<Transform>();

    public int contentRectTransformElementCount { get { return CountLayoutElements(); } }

    public VerticalLayoutGroup rowsVerticalLayoutGroup;
    List<HorizontalLayoutGroup> rowsVerticalLayoutGroupList = new List<HorizontalLayoutGroup>();
    HorizontalLayoutGroup horizontalLayoutGroupRow;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = new GameObject();
        ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        horizontalLayoutGroupRow = gameObject.AddComponent<HorizontalLayoutGroup>();

        foreach (Transform horizontalLayoutGroupRowTransform in rowsVerticalLayoutGroup.transform)
        {
            HorizontalLayoutGroup horizontalLayoutGroup;
            if (horizontalLayoutGroupRowTransform.TryGetComponent(out horizontalLayoutGroup))
            {
                rowsVerticalLayoutGroupList.Add(horizontalLayoutGroup);
            }
        }

        StartCoroutine(UpdateCanvases());
    }

    IEnumerator UpdateCanvases()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
    }

    // Update is called once per frame
    void Update()
    {
        if (contentRectTransformElementCount != contentRectTransformList.Count || widthLimitRectTransform.hasChanged)
        {
            Debug.Log($"contentRectTransformElementCount: {contentRectTransformElementCount}");
            Debug.Log($"contentRectTransformList.Count: {contentRectTransformList.Count}");
            Debug.Log($"{widthLimitRectTransform.hasChanged}");

            UpdateContentRectTransformList();
            widthLimitRectTransform.hasChanged = false;
        }
    }

    int CountLayoutElements()
    {
        int elementsCurrentlyRendered = 0;

        foreach (Transform horizontalLayoutGroupTransform in rowsVerticalLayoutGroup.transform)
        {
            HorizontalLayoutGroup horizontalLayoutGroup = horizontalLayoutGroupTransform.GetComponent<HorizontalLayoutGroup>();
            elementsCurrentlyRendered += horizontalLayoutGroup.transform.childCount;
        }

        foreach (Transform child in contentRectTransform)
        {
            // only count a child if it has no VerticalLayoutGroup component
            // if the children have VerticalLayoutGroup components...
            // only count them if they are not the rowsVerticalLayoutGroup being managed by this script
            VerticalLayoutGroup childLayout;
            if (!child.TryGetComponent(out childLayout) || childLayout != rowsVerticalLayoutGroup)
            {
                elementsCurrentlyRendered++;
            }
        }

        return elementsCurrentlyRendered;
    }

    void UpdateContentRectTransformList()
    {
        contentRectTransformList.Clear();

        foreach (Transform horizontalLayoutGroupTransform in rowsVerticalLayoutGroup.transform)
        {
            HorizontalLayoutGroup horizontalLayoutGroup = horizontalLayoutGroupTransform.GetComponent<HorizontalLayoutGroup>();
            foreach (Transform layoutElementTransform in horizontalLayoutGroup.transform)
            {
                contentRectTransformList.Add(layoutElementTransform);
            }
        }

        foreach (Transform child in contentRectTransform)
        {
            // only add a child if it has no VerticalLayoutGroup component
            // if the children have VerticalLayoutGroup components...
            // only add them if they are not the rowsVerticalLayoutGroup being managed by this script
            VerticalLayoutGroup childLayout;
            if (!child.TryGetComponent(out childLayout) || childLayout != rowsVerticalLayoutGroup)
            {
                contentRectTransformList.Add(child.transform);
            }
        }

        //Debug.Log($"contentRectTransformElementCount: {contentRectTransformElementCount}");
        //Debug.Log($"contentRectTransformList.Count: {contentRectTransformList.Count}");

        RenderPackedLayoutGroup(contentRectTransformList);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
        Canvas.ForceUpdateCanvases();
    }

    private void RenderPackedLayoutGroup(List<Transform> contentRectTransformList)
    {
        foreach (Transform child in contentRectTransformList)
        {
            child.SetParent(contentRectTransform);
        }

        foreach (HorizontalLayoutGroup horizontalLayoutGroup in rowsVerticalLayoutGroupList)
        {
            Destroy(horizontalLayoutGroup.gameObject);
        }

        rowsVerticalLayoutGroupList.Clear();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

        foreach (Transform child in contentRectTransformList)
        {
            AddLayoutElementToPackedLayoutGroup(child, rowsVerticalLayoutGroup, rowsVerticalLayoutGroupList);
        }
    }

    private void AddLayoutElementToPackedLayoutGroup(Transform child, VerticalLayoutGroup rowsLayoutGroup, List<HorizontalLayoutGroup> rowsLayoutGroupList)
    {
        HorizontalLayoutGroup lastRowInLayout;
        
        // get the last row in the packed layout
        if (rowsLayoutGroupList.Count < 1)
        {
            lastRowInLayout = Instantiate(horizontalLayoutGroupRow, rowsLayoutGroup.transform);
            rowsLayoutGroupList.Add(lastRowInLayout);
        }
        else
        {
            lastRowInLayout = rowsLayoutGroupList[rowsLayoutGroupList.Count - 1];
        }

        // if this is the only element in the row then add it without any restrictions
        if (lastRowInLayout.transform.childCount == 0)
        {
            child.transform.SetParent(lastRowInLayout.transform, false);
            return;
        }

        // otherwise find the total width of the row and calculate the available width from the other elements in the row
        float rowPreferredWidth = widthLimitRectTransform.rect.width;
        float rowAvailableWidth = rowPreferredWidth;
        
        foreach (Transform rowElementTransform in lastRowInLayout.transform)
        {
            ILayoutElement rowElement = rowElementTransform.GetComponent<ILayoutElement>();
            rowAvailableWidth -= rowElement.preferredWidth;
        }

        // if it can fit in the current row then add it
        ILayoutElement layoutGroup = child.GetComponent<ILayoutElement>();
        if (layoutGroup.preferredWidth <= rowAvailableWidth)
        {
            child.transform.SetParent(lastRowInLayout.transform, false);
            return;
        }
        // otherwise create a new row and add it there
        else
        {
            lastRowInLayout = Instantiate(horizontalLayoutGroupRow, rowsLayoutGroup.transform);
            rowsLayoutGroupList.Add(lastRowInLayout);
            child.transform.SetParent(lastRowInLayout.transform, false);
            return;
        }
    }
}
