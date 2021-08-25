using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevRoundVarEditor : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;

    [SerializeField]
    private TMP_InputField playerSpeedField;
    [SerializeField]
    private TMP_InputField visionRadiusField;
    [SerializeField]
    private TMP_InputField viewAngleField;
    [SerializeField]
    private TMP_InputField startingMeetingsField;
    [SerializeField]
    private TMP_InputField radioChargeTimeField;
    [SerializeField]
    private TMP_InputField emergencyMeetingTimerField;
    [SerializeField]
    private TMP_InputField playersPerTraitorField;
    [SerializeField]
    private TMP_InputField tasksPerPlayerField;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            mainPanel.SetActive(!mainPanel.activeSelf);
        }
    }

    public void SendDataToServer()
    {
        float playerSpeed = float.Parse(playerSpeedField.text);
        float visionRadius = float.Parse(visionRadiusField.text);
        float viewAngle = float.Parse(viewAngleField.text);
        int startingMeetings = int.Parse(startingMeetingsField.text);
        int radioChargeTime = int.Parse(radioChargeTimeField.text);
        float emergencyMeetingTimer = float.Parse(emergencyMeetingTimerField.text);
        int playersPerTraitor = int.Parse(playersPerTraitorField.text);
        int tasksPerPlayer = int.Parse(tasksPerPlayerField.text);

        ClientSend.DevSetRoundVars(playerSpeed, visionRadius, viewAngle, startingMeetings, radioChargeTime, emergencyMeetingTimer, playersPerTraitor, tasksPerPlayer);
    }
}
