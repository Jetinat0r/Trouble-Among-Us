using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GhostCameraReParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = Camera.main.transform;

        UniversalAdditionalCameraData _cameraData = Camera.main.GetUniversalAdditionalCameraData();
        _cameraData.cameraStack.Add(gameObject.GetComponent<Camera>());
    }
}
