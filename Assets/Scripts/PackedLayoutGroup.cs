using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackedLayoutGroup : LayoutGroup
{
    // TODO: we might want to implement a packed layout using simpler Unity UI components
    // it may be more effective to implement the MonoBehaviour class and use that as a base
    // and build the PackedLayoutGroup script using the other layout groups or simpler code
    // rather than implementing a whole new class inheriting from LayoutGroup

    public override void CalculateLayoutInputVertical()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
