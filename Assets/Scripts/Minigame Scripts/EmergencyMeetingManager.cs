using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmergencyMeetingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerProfileBlockPrefab;
    [SerializeField]
    private GameObject playerProfileBlockContainer;

    [SerializeField]
    private TMP_Text tasksCompleteText;

    [SerializeField]
    private GameObject skipButton;
    [SerializeField]
    private GameObject skipButtonRevealedVotes;
    [SerializeField]
    private GameObject skippedText;
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private string timerPreText = "Voting ends in: ";
    private float timer;
    private bool isTimerShown = true;

    [SerializeField]
    private GameObject playerVotePrefab;

    //True if any player is able to vote
    private bool isMeetingActive = false;

    //Set all inactive after voting or if the timer runs out
    private List<GameObject> voteButtons = new List<GameObject>();
    
    private List<EmergencyVotePlayerBlock> playerProfileBlocks = new List<EmergencyVotePlayerBlock>();

    private void Update()
    {
        if (isTimerShown)
        {
            timer -= Time.deltaTime;
            int _timeInSec = (int)timer;
            timerText.text = timerPreText + _timeInSec + "s";

            if (isMeetingActive && timer <= 0)
            {
                EndVoting();
                HideTimer();
            }
        }
    }

    public void StartMeeting(int _playerID, float _timer)
    {
        SetupMeetingLayout(_playerID);
        SetupCompletedTasks();

        Player.instance.StartMeeting();
        if (!Player.instance.isAlive)
        {
            EndVoting();
        }

        timer = _timer;
        isMeetingActive = true;
    }

    //Start Timer
    //Place all players that are in the game

    //                           _playerID is the player id of the player that started the meeting
    private void SetupMeetingLayout(int _playerID)
    {
        foreach(KeyValuePair<int, PlayerManager> pair in GameManager.players)
        {
            PlayerManager _player = pair.Value;

            if(_player == null)
            {
                return;
            }

            GameObject _playerBlock = Instantiate(playerProfileBlockPrefab, playerProfileBlockContainer.transform);
            _playerBlock.GetComponent<EmergencyVotePlayerBlock>().SetupPlayerProfileBlock(_player, _playerID, this);
            voteButtons.Add(_playerBlock.GetComponent<EmergencyVotePlayerBlock>().GetVoteButton());
            playerProfileBlocks.Add(_playerBlock.GetComponent<EmergencyVotePlayerBlock>());
        }
    }

    private void SetupCompletedTasks()
    {
        int curCompleted = 0;
        int total = 0;

        foreach(KeyValuePair<int, PlayerManager> pair in GameManager.players)
        {
            if(pair.Value != null)
            {
                // 1 = innocent
                if(pair.Value.gameRole == 1)
                {
                    curCompleted += pair.Value.completedTasks;
                    total += pair.Value.totalTasks;
                }
            }
        }

        tasksCompleteText.text = "Completed Tasks\n" + curCompleted + "/" + total;
    }

    private void HideTimer()
    {
        isTimerShown = false;

        timerText.gameObject.SetActive(false);
    }

    public void VoteForPlayer(int targetPlayerID)
    {
        ClientSend.ClientSendMeetingVote(targetPlayerID);

        EndVoting();
    }

    //Called when either the player votes (via ClientHandle) or when the time runs out
    public void EndVoting()
    {
        isMeetingActive = false;

        foreach(GameObject button in voteButtons)
        {
            button.SetActive(false);
        }

        skipButton.SetActive(false);
    }

    //                      i.e. target: 1; from: green
    public void DisplayVotes(int[] playerIDs, Color[] fromPlayers)
    {
        isMeetingActive = false;

        EndVoting();
        HideTimer();


        //Display the votes
        for(int i = 0; i < playerIDs.Length; i++)
        {
            if(playerIDs[i] == 0)
            {
                GameObject vote = Instantiate(playerVotePrefab, skipButtonRevealedVotes.transform);
                vote.GetComponent<Image>().color = fromPlayers[i];
                continue;
            }

            foreach(Transform child in playerProfileBlockContainer.transform)
            {
                if(child.GetComponent<EmergencyVotePlayerBlock>().playerID == playerIDs[i])
                {
                    GameObject vote = Instantiate(playerVotePrefab, child.GetComponent<EmergencyVotePlayerBlock>().voteCollection.transform);
                    vote.GetComponent<Image>().color = fromPlayers[i];
                }
            }
        }

        #region Determine who was voted for
        //Determine who got voted for
        //Default is "skip"
        int mostFrequentID = 0;
        int maxCount = 0;
        int curCount = 0;

        for(int i = 0; i < playerIDs.Length; i++)
        {
            for(int j = i; j < playerIDs.Length; j++)
            {
                if(playerIDs[i] == playerIDs[j])
                {
                    curCount++;
                }
            }

            if(curCount > maxCount)
            {
                mostFrequentID = playerIDs[i];
                maxCount = curCount;
            }
            else if (curCount == maxCount)
            {
                mostFrequentID = 0;
            }

            curCount = 0;
        }
        #endregion

        //Display the death marker on whoever got voted for
        if (mostFrequentID != 0)
        {
            foreach (EmergencyVotePlayerBlock playerProfile in playerProfileBlocks)
            {
                playerProfile.MarkForDeath(mostFrequentID);
            }
        }        
    }

    public void EndMeeting(float time)
    {
        isTimerShown = true;

        skippedText.SetActive(true);

        timerText.gameObject.SetActive(true);
        timerPreText = "Ending in: ";
        timer = time;

        StartCoroutine(EndMeetingCoroutine(time));
    }

    private IEnumerator EndMeetingCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        Player.instance.EndMeeting();

        Destroy(gameObject);
    }
}
