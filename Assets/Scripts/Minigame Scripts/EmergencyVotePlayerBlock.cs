using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmergencyVotePlayerBlock : MonoBehaviour
{
    [SerializeField]
    private Image playerColor;
    [SerializeField]
    private TMP_Text playerName;
    public Button voteButton;
    [SerializeField]
    private GameObject deadPlayerBlackout;
    [SerializeField]
    private GameObject calledMeetingIndicator;

    public int playerID;

    //The votes against the player in question
    public GameObject voteCollection;

    public void SetupPlayerProfileBlock(PlayerManager _player, int meetingCallerPlayerId, EmergencyMeetingManager manager)
    {
        playerColor.color = _player.playerColor;
        playerName.text = _player.username;
        playerName.color = _player.GetNameplateColor();
        
        playerID = _player.id;

        if(meetingCallerPlayerId == playerID)
        {
            calledMeetingIndicator.SetActive(true);
        }

        if (_player.isAlive)
        {
            voteButton.gameObject.SetActive(true);
            voteButton.onClick.AddListener(() => manager.VoteForPlayer(playerID));
        }
        else
        {
            deadPlayerBlackout.SetActive(true);
            voteButton.gameObject.SetActive(false);
        }
    }

    public GameObject GetVoteButton()
    {
        return voteButton.gameObject;
    }

    
}
