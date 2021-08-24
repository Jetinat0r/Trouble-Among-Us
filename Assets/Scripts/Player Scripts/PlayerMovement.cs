using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private GameObject weaponPivot;

    private Vector2 movementVector;

    [SerializeField]
    private FOVMeshGenerator fovMeshGeneratorScript;

    private bool canMove = true;
    private bool inCutscene = false;
    private bool inMenu = false;
    private bool inMeeting = false;

    private void Start()
    {
        //Subscribe to events here
        SettingsUI.instance.OnSettingsOpen += OnSettingsOpen;
        SettingsUI.instance.OnSettingsClose += OnSettingsClose;
    }

    void Update()
    {
        //Input
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = Input.GetAxisRaw("Vertical");

        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 _directionToMouse = new Vector2(_mousePos.x - transform.position.x, _mousePos.y - transform.position.y);

        weaponPivot.transform.right = _directionToMouse;

        float _z = weaponPivot.transform.eulerAngles.z;
        if(_z == 0 && weaponPivot.transform.eulerAngles.y == 180)
        {
            _z = 180;
        }

        fovMeshGeneratorScript.SetPosRot(transform.position, _z);
    }

    //PUT THE "IF(CAN'T MOVE)" HERE
    private void FixedUpdate()
    {
        if (canMove && !inCutscene && !inMenu && !inMeeting)
        {
            //Movement
            rb.MovePosition(rb.position + (movementVector.normalized * movementSpeed * Time.fixedDeltaTime));

            //Failed Mesh Generator Idea
            //fovMeshGeneratorScript.SetPosRot(transform.position, weaponPivot.transform.eulerAngles.z);
            //fovMeshGeneratorScript.DrawFieldOfView();
        }

        SendPosRotToServer();
    }

    #region Movement enable/disable
    public void StopMovement()
    {
        canMove = false;
    }

    public void AllowMovement()
    {
        canMove = true;
    }

    public void StartCutscene()
    {
        inCutscene = true;
    }

    public void EndCutscene()
    {
        inCutscene = false;
    }

    public void StartMeeting()
    {
        inMeeting = true;
    }

    public void EndMeeting()
    {
        inMeeting = false;
    }

    private void OnSettingsOpen(object _sender, EventArgs _e)
    {
        inMenu = true;
    }

    private void OnSettingsClose(object _sender, EventArgs _e)
    {
        inMenu = false;
    }

#endregion

    private void SendPosRotToServer()
    {
        ClientSend.PlayerPosRot(transform.position, weaponPivot.transform.rotation);
    }

    public void SetGameplayVariables(float _playerSpeed, float _viewRadius, bool _isTraitor)
    {
        movementSpeed = _playerSpeed;

        if (_isTraitor)
        {
            fovMeshGeneratorScript.SetViewRadius(100);
        }
        else
        {
            fovMeshGeneratorScript.SetViewRadius(_viewRadius);
        }
    }

    public void SetViewRadius(float _viewRadius)
    {
        fovMeshGeneratorScript.SetViewRadius(_viewRadius);
    }

    private void OnDestroy()
    {
        //Unsubscribe from events here
        SettingsUI.instance.OnSettingsOpen -= OnSettingsOpen;
        SettingsUI.instance.OnSettingsClose -= OnSettingsClose;
    }
}
