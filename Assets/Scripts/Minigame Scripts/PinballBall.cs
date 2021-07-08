using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballBall : MonoBehaviour
{
    public PinballCore pinballCore;

    public string goalTag;
    public string failTag;
    public float timeToGoal;
    private bool isInGoal = false;

    public void BallSetup(PinballCore core)
    {
        pinballCore = core;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(goalTag) && !isInGoal)
        {
            isInGoal = true;
            collision.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(EnterGoal(collision.transform));
        }
        else if (collision.CompareTag(failTag) && !isInGoal)
        {
            isInGoal = true;
            pinballCore.SpawnPinball();
            Destroy(gameObject);
        }
        
    }

    private IEnumerator EnterGoal(Transform goalPos)
    {
        float t = 0;
        Vector3 startPos = transform.position;

        //Makes it so that the ball completely stops forever
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        rbody.isKinematic = true;
        rbody.velocity = Vector2.zero;
        rbody.angularVelocity = 0;

        //Ensures that balls that reach their goal don't collide with anything ever again
        GetComponent<Collider2D>().enabled = false;

        while(t < 1)
        {
            t += Time.deltaTime / timeToGoal;
            transform.position = Vector3.Lerp(startPos, goalPos.position, t);

            yield return null;
        }

        //Stuff down here still gets called!
        pinballCore.CheckWin();
        //pinballCore.SpawnPinball();
    }
}
