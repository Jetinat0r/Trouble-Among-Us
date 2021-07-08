using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public Rigidbody2D rb;
    private bool isHeld = false;
    public float forceMultiplier = 50f;
    public ForceMode2D forceMode;
    private GameObject forcePoint;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isHeld = false;

            Destroy(forcePoint);
        }

        if (isHeld)
        {
            Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 _forceVector = new Vector2(_mousePos.x - forcePoint.transform.position.x, _mousePos.y - forcePoint.transform.position.y) * forceMultiplier * Time.deltaTime;
            rb.AddForceAtPosition(_forceVector, forcePoint.transform.position, forceMode);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHeld = true;

            Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            forcePoint = new GameObject("Force Point");
            forcePoint.transform.position = _mousePos;

            forcePoint.transform.SetParent(gameObject.transform);
        }
    }
}