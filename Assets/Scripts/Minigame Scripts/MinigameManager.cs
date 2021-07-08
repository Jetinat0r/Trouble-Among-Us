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

    public int TEST = 0;
    public int GLOBAL_TEST = 0;

    #region OnMinigameComplete Event
    public event EventHandler<OnMinigameCompleteEventArgs> OnMinigameComplete;
    public class OnMinigameCompleteEventArgs : EventArgs
    {
        public bool isEmergency;
        public int index;
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AssignMinigame(minigameStarters[TEST]);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AssignGlobalMinigame(GLOBAL_TEST);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ClearMinigames();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            RemoteCloseMinigames();
        }
    }

    public void AssignMinigame(MinigameStarter _ms)
    {
        //TODO: ADD MINIGAME TO PLAYER UI (use minigameStarters[index].minigameName)

        OnMinigameAssign?.Invoke(this, new OnMinigameAssignArgs { isEmergency = false, index = _ms.index, incompleteText = _ms.incompleteText });

        _ms.gameObject.SetActive(true);
    }

    public void AssignGlobalMinigame(int _index)
    {
        globalMinigameStarters[_index].gameObject.SetActive(true);
    }

    //TODO: CALL WHEN A MATCH IS RESET
    public void ClearMinigames()
    {
        foreach(MinigameStarter _ms in minigameStarters)
        {
            _ms.ClearMinigame();
        }

        foreach(MinigameStarter ms in globalMinigameStarters)
        {
            ms.ClearMinigame();
        }
    }

    //Used to forcefully drag people out of minigames
    public void RemoteCloseMinigames()
    {
        foreach (MinigameStarter _ms in minigameStarters)
        {
            _ms.RemoteCloseMinigame();
        }

        foreach (MinigameStarter ms in globalMinigameStarters)
        {
            ms.RemoteCloseMinigame();
        }
    }

    //Door tasks or emergencies
    public void GlobalMinigameCompleted(int _index)
    {
        //TODO: Clear from UI and yell at every other player
        globalMinigameStarters[_index].ClearMinigame();
    }

    public void MinigameCompleted(int _index, bool _isFinal)
    {
        //DONE: calls player ui to complete minigame within minigame class

        OnMinigameComplete?.Invoke(this, new OnMinigameCompleteEventArgs { isEmergency = false, index = _index, isFinal = _isFinal, completeText = minigameStarters[_index].completeText });

        //no need to SetActive(false) bc minigameStarter does that already
        if (!_isFinal)
        {
            AssignMinigame(minigameStarters[_index + 1]);
        }
        else
        {
            Debug.Log("Hooray?");
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
            Debug.Log("Count: " + _availableTaskIndexes.Count);
            int _random = UnityEngine.Random.Range(0, _availableTaskIndexes.Count);

            currentMinigames.Add(minigameStarters[_availableTaskIndexes[_random]]);
            _availableTaskIndexes.RemoveAt(_random);
        }

        for(int i = 0; i < currentMinigames.Count; i++)
        {
            Debug.Log("Heya " + i);
            AssignMinigame(currentMinigames[i]);
        }
    }
}
