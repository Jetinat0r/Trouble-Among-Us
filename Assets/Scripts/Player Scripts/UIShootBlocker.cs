using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShootBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private PlayerShoot playerShoot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        playerShoot.UIDisableShooting();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerShoot.UIAllowShooting();
    }
}
