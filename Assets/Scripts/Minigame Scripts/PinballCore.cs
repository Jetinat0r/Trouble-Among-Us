using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballCore : Minigame
{
    [SerializeField]
    private HingeJoint2D lFlipper;
    //private JointMotor2D lFlipperMotor;
    [SerializeField]
    private HingeJoint2D rFlipper;
    //private JointMotor2D rFlipperMotor;

    [SerializeField]
    private int flipperMotorSpeed = 750;

    [SerializeField]
    private GameObject pinballPrefab;

    [SerializeField]
    private Transform pinballSpawn;

    public int numGoals = 3;
    private int curCompletedGoals = 0;

    private void Start()
    {
        SpawnPinball();
    }

    // Update is called once per frame
    void Update()
    {
        //Left Flipper
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            JointMotor2D lFlipperMotor = lFlipper.motor;
            lFlipperMotor.motorSpeed = -flipperMotorSpeed;
            lFlipper.motor = lFlipperMotor;
        }
        else
        {
            JointMotor2D lFlipperMotor = lFlipper.motor;
            lFlipperMotor.motorSpeed = flipperMotorSpeed;
            lFlipper.motor = lFlipperMotor;
        }

        //Right Flipper
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            JointMotor2D rFlipperMotor = rFlipper.motor;
            rFlipperMotor.motorSpeed = flipperMotorSpeed;
            rFlipper.motor = rFlipperMotor;
        }
        else
        {
            JointMotor2D rFlipperMotor = rFlipper.motor;
            rFlipperMotor.motorSpeed = -flipperMotorSpeed;
            rFlipper.motor = rFlipperMotor;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            CloseMinigame();
        }
    }

    public void SpawnPinball()
    {
        if(curCompletedGoals != numGoals)
        {
            GameObject _pinball = Instantiate(pinballPrefab, pinballSpawn.transform.position, pinballSpawn.transform.rotation);

            //Makes the core the ball's parent, did this jic
            _pinball.transform.SetParent(transform);

            _pinball.GetComponent<PinballBall>().pinballCore = this;
        }
    }

    //This is called whenever a ball enters a goal, so I just put the increase goal amount here
    public void CheckWin()
    {
        curCompletedGoals++;

        if(curCompletedGoals == numGoals)
        {
            CompleteMinigame();
        }
        else
        {
            SpawnPinball();
        }
    }
}
