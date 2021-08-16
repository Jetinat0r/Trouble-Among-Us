using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//[CreateAssetMenu(fileName = "New Slider/Input Field Connector", menuName = "My Scriptable Objects/Slider_Input Field Connector")]
public class SliderAndInputFieldConnector : MonoBehaviour
{
    public TMP_Text playerName;
    public int playerID = -1;
    public Slider slider;
    public TMP_InputField inputField;

    public float lastInput = 1;

    //Returns true if the slider is used, and false if the input field is used
    public bool DetermineInputDevice()
    {
        if (float.Parse(inputField.text) != lastInput)
        {
            return false;
        }

        return true;
    }

    public void SetPlayerManagerVolume()
    {
        if(playerID == -1)
        {
            return;
        }

        PlayerManager _player = GameManager.players[playerID];
        if(_player != null)
        {
            _player.SetVCVolume(lastInput);
        }
    }
}
