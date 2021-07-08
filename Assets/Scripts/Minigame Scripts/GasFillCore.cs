using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasFillCore : Minigame
{
    public bool isFill = true;

    public int requiredGas = 100;
    public float speed = 1f;

    public GameObject gasBlock;
    public Transform bottomPoint;
    public Transform topPoint;
    public float maxYScale = 1f;
    public float timeToFill = 4.5f;
    private float fillTimer;

    public SpriteRenderer button;
    public Color unPressed;
    public Color pressed;

    private bool isHeld = false;

    private void Start()
    {
        if (isFill)
        {
            fillTimer = timeToFill;
        }
        else
        {
            fillTimer = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseMinigame();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHeld = false;
            button.color = unPressed;
        }

        if (isHeld)
        {
            if (isFill)
            {
                fillTimer -= Time.deltaTime;
            }
            else
            {
                fillTimer += Time.deltaTime;
            }


            //STEP 1: DIST BETWEEN TOP AND BOTTOM POINTS [DIDN'T ACTUALLY HAPPEN]
            //STEP 2: LERP gasBlock BETWEEN 0 AND 0.5 OF SAID DIST
            gasBlock.transform.position = new Vector3(gasBlock.transform.position.x, Mathf.Lerp(bottomPoint.position.y, topPoint.position.y, (fillTimer / timeToFill) / 2), gasBlock.transform.position.z);
            //STEP 3: SCALE gasBlock Y TO 2x CURRENT LERP RATIO * maxYScale
            gasBlock.transform.localScale = new Vector3(gasBlock.transform.localScale.x, (fillTimer / timeToFill) * maxYScale, gasBlock.transform.localScale.z);

            if (isFill)
            {
                if(fillTimer <= 0)
                {
                    CompleteMinigame();
                }
            }
            else
            {
                if(fillTimer >= timeToFill)
                {
                    CompleteMinigame();
                }
            }
        }
    }

    public void CheckWin()
    {
        requiredGas--;

        if (requiredGas <= 0)
        {
            CompleteMinigame();
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHeld = true;

            button.color = pressed;
        }
    }
}
