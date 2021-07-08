using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBreakersBreaker : MonoBehaviour
{
    [SerializeField]
    private DoorBreakersCore core;
    private bool isActive = false;
    [SerializeField]
    private Sprite activeSprite;
    [SerializeField]
    private Sprite inactiveSprite;

    public void ActivateBreaker()
    {
        isActive = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = activeSprite;
    }

    private void OnMouseDown()
    {
        if (isActive)
        {
            isActive = false;
            core.ActivateBreaker();
            gameObject.GetComponent<SpriteRenderer>().sprite = inactiveSprite;
        }
    }
}
