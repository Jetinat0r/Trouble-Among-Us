using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeParent : MonoBehaviour
{
    void Start()
    {
        transform.SetParent(null);
    }
}
