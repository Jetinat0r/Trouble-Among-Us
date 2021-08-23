using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShootBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerShoot playerShoot;

    private void Start()
    {
        playerShoot = Player.instance.GetPlayerShoot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        playerShoot.UIDisableShooting();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerShoot.UIAllowShooting();
    }
}
