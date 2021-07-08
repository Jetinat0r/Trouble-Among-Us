using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponSelectComponents
{
    public string name;

    public GameObject container;
    public Image buttonImage;
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI weaponText;
    public Image weaponImage;

    public Color inactivePrimaryColor;
    public Color activePrimaryColor;

    public Color inactiveBackgroundColor;
    public Color activeBackgroundColor;
}
