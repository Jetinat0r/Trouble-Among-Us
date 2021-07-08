using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIObjectHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.NonSerialized]
    public bool isHovered = false;
    [SerializeField]
    public GameObject calledObject;

    //Used for objects already in scene (may cause redundancies)
    private void Awake()
    {
        if(calledObject != null)
        {
            calledObject.GetComponent<IUIObjectHover>().AddHoverableObject(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        calledObject.GetComponent<IUIObjectHover>().HoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        calledObject.GetComponent<IUIObjectHover>().HoverExit();
    }
}

public interface IUIObjectHover
{
    void AddHoverableObject(UIObjectHover _object);

    void HoverEnter();
    void HoverExit();
}