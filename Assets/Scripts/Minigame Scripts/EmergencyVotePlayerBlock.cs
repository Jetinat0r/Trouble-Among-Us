using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmergencyVotePlayerBlock : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer playerColor;
    [SerializeField]
    private TMP_Text playerName;
    [SerializeField]
    private Button voteButton;

    private int playerID;

    //The votes against the player in question
    [SerializeField]
    private GameObject voteCollection;

    public void SetupPlayerProfileBlock(EmergencyMeetingManager eMM, Color color, string username, int _playerID, bool isAlive)
    {
        PlayerManager _player = GameManager.players[_playerID];
        playerColor.color = _player.playerColor;
        playerName.text = _player.username;
        playerName.color = _player.GetNameplateColor();
        
        playerID = _playerID;

        if (isAlive)
        {
            voteButton.onClick.AddListener(() => VoteForPlayer(_playerID));
        }
    }

    private void VoteForPlayer(int _playerID)
    {
        //Server Send "VoteForPlayer(_playerID)"
    }
}
