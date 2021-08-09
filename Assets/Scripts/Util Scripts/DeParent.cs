using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeParent : MonoBehaviour
{
    [SerializeField]
    private bool resetPosition = false;

    void Start()
    {
        transform.SetParent(null);

        if (resetPosition)
        {
            transform.position = Vector3.zero;
        }
    }
}
