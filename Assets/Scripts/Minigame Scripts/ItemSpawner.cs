using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private float t = 0;
    public GameObject minigameCore;

    public GameObject itemPrefab;
    public float objsPerSecond;
    public float force = 10f;
    private float rate;

    private void Start()
    {
        rate = 1 / objsPerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if(t >= rate)
        {
            GameObject _object = Instantiate(itemPrefab, gameObject.transform.position, gameObject.transform.rotation);
            _object.transform.SetParent(minigameCore.transform);

            Vector2 _forceVector = new Vector2(_object.transform.right.x, _object.transform.right.y) * force;

            _object.GetComponent<Rigidbody2D>().AddForce(_forceVector, ForceMode2D.Impulse);

            t = 0;
        }
    }
}
