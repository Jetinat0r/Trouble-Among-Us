using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager instance;

    //MULTI-STAGE MINIGAMES MUST BE LISTED IN ORDER
    public MinigameStarter[] minigameStarters;
    //Global Minigames are emergencies
    public MinigameStarter[] globalMinigameStarters;

    //public int TEST = 0;
    //public int GLOBAL_TEST = 0;

    #region OnMinigameComplete Event
    public event EventHandler<OnMinigameCompleteEventArgs> OnMinigameComplete;
    public class OnMinigameCompleteEventArgs : EventArgs
    {
        public bool isEmergency;
        public int index;

        //isFirst is for checking whether or not to remove emergencies
        public bool isFirst;
        //isFinal is saying that the one that was completed was the final one in the series
        public bool isFinal;
        public string completeText;
    }
    #endregion

    #region OnMinigameAssign Event
    public event EventHandler<OnMinigameAssignArgs> OnMinigameAssign;
    public class OnMinigameAssignArgs
    {
        public bool isEmergency;
        public int index;
        public string incompleteText;
        public bool isFirst;
    }
    #endregion

    public List<MinigameStarter> currentMinigames = new List<MinigameStarter>();
    public List<MinigameStarter> currentGlobalMinigames = new List<MinigameStarter>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying!");
            Destroy(gameObject);
        }
    }

    // Dev Keys
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        AssignMinigame(minigameStarters[TEST]);
    //    }
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        //AssignGlobalMinigame(globalMinigameStarters[GLOBAL_TEST]);
    //        ClientSend.ClientEmergencyStartRequest(globalMinigameStarters[GLOBAL_TEST]);
    //    }
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        ClearMinigames();
    //    }
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        RemoteCloseMinigames();
    //    }
    //}

    public void AssignMinigame(MinigameStarter _ms)
    {
        OnMinigameAssign?.Invoke(this, new OnMinigameAssignArgs { isEmergency = false, index = _ms.index, incompleteText = _ms.incompleteText, isFirst = _ms.isFirst });

        _ms.gameObject.SetActive(true);
    }

    public void AssignGlobalMinigame(MinigameStarter _ms)
    {
        //First check if its the doors, and if so: don't alert the player's UI
        //The check for this will be the "isFirst" flag

        OnMinigameAssign?.Invoke(this, new OnMinigameAssignArgs { isEmergency = true, index = _ms.index, incompleteText = _ms.incompleteText, isFirst = _ms.isFirst });

        //Only one Emergency can take place at the same time

        _ms.gameObject.SetActive(true);
    }

    //TODO: CALL WHEN A MATCH IS RESET
    public void ClearMinigames()
    {
        foreach(MinigameStarter _ms in minigameStarters)
        {
            _ms.ClearMinigame();
        }

        foreach(MinigameStarter _ms in globalMinigameStarters)
        {
            _ms.ClearMinigame();
        }
    }

    //Used to forcefully drag people out of minigames
    public void RemoteCloseMinigames()
    {
        foreach (MinigameStarter _ms in minigameStarters)
        {
            _ms.RemoteCloseMinigame();
        }

        foreach (MinigameStarter _ms in globalMinigameStarters)
        {
            _ms.RemoteCloseMinigame();
        }
    }

    //Door tasks or emergencies
    public void GlobalMinigameCompleted(MinigameStarter _ms)
    {
        //DONE: Clear from UI
        //TODO: yell at every other player
        OnMinigameComplete?.Invoke(this, new OnMinigameCompleteEventArgs { isEmergency = true, index = _ms.index, isFinal = true, completeText = _ms.completeText, isFirst = _ms.isFirst });

        //To be handled server side
        ClientSend.ClientCompleteEmergency(_ms);
        //globalMinigameStarters[_ms.index].ClearMinigame();
    }

    public void MinigameCompleted(MinigameStarter _ms)
    {
        //DONE: calls player ui to complete minigame within minigame class

        OnMinigameComplete?.Invoke(this, new OnMinigameCompleteEventArgs { isEmergency = false, index = _ms.index, isFinal = _ms.isFinal, completeText = _ms.completeText, isFirst = _ms.isFirst });

        //no need to SetActive(false) bc minigameStarter does that already
        if (!_ms.isFinal)
        {
            AssignMinigame(minigameStarters[_ms.index + 1]);
        }
        else
        {
            ClientSend.ClientCompleteTask(_ms);
            //Debug.Log("Hooray?");
        }
    }

    public void StartRound(int _numTasks, bool _isInnocent)
    {
        if (!_isInnocent)
        {
            //Set task to be "kill everyone"

            return;
        }


        //Generates a list of valid starting tasks
        List<int> _availableTaskIndexes = new List<int>();
        foreach(MinigameStarter _ms in minigameStarters)
        {
            if (_ms.isFirst)
            {
                _availableTaskIndexes.Add(_ms.index);
            }
        }

        //Ensures no leftover tasks from previous rounds
        currentMinigames = new List<MinigameStarter>();
        currentGlobalMinigames = new List<MinigameStarter>();

        //Picks random starting task indexes and assigns the respective MinigameStarters to an array
        for(int i = 0; i < _numTasks; i++)
        {
            //Debug.Log("Count: " + _availableTaskIndexes.Count);
            int _random = UnityEngine.Random.Range(0, _availableTaskIndexes.Count);

            currentMinigames.Add(minigameStarters[_availableTaskIndexes[_random]]);
            _availableTaskIndexes.RemoveAt(_random);
        }

        for(int i = 0; i < currentMinigames.Count; i++)
        {
            //Debug.Log("Heya " + i);
            AssignMinigame(currentMinigames[i]);
        }
    }
}
