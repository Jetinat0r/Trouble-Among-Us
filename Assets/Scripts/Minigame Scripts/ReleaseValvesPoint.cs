using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseValvesPoint : MonoBehaviour
{
    public ReleaseValvesCore core;
    public SpriteRenderer valveLight;
    public Color offLight;
    public Color onLight;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            core.UpdateGoals(1);

            valveLight.color = onLight;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            core.UpdateGoals(-1);

            valveLight.color = offLight;
        }
    }
}
