using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyButtonButton : MonoBehaviour
{
    [SerializeField]
    private EmergencyButtonCore core;

    private void OnMouseDown()
    {
        core.ButtonPress();
    }
}
