using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererSortingLayer : MonoBehaviour
{
    public int sortLayer = 0;
    public string sortingLayerName;

    [SerializeField]
    private Renderer objectRenderer;

    // Start is called before the first frame update
    void Start()
    {
        int sortingLayerID = SortingLayer.NameToID(sortingLayerName);

        objectRenderer.sortingLayerID = sortingLayerID;
        objectRenderer.sortingOrder = sortLayer;
    }
}
