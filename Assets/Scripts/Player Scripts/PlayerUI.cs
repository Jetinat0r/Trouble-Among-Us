using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour, IUIObjectHover
{
    #region Player Bars
    [Header("Player Bars")]

    [SerializeField]
    private RectTransform healthBarFill;
    private float healthBarFillMaxWidth;
    [SerializeField]
    private TextMeshProUGUI healthBarText;
    [SerializeField]
    private GameObject reloadBar;
    [SerializeField]
    private RectTransform reloadBarFill;
    private float reloadBarFillMaxWidth;
    #endregion

    #region Minigame Complete Stuff
    [Header("Minigame Complete Stuff")]

    [SerializeField]
    private RectTransform minigameCompleteText;
    [SerializeField]
    private RectTransform minigameCompleteTextStartPos;
    [SerializeField]
    private RectTransform minigameCompleteTextEndPos;
    [SerializeField]
    private float minigameCompleteTextTimer;
    #endregion

    #region Minigame Task List Stuff
    [Header("Minigame Task List Stuff")]
    [SerializeField]
    private GameObject minigameTaskListMainObject;
    

    [SerializeField]
    private float minigameTaskListMoveTimer = 0.5f;

    [SerializeField]
    private RectTransform minigameTaskListStartPos;
    [SerializeField]
    private RectTransform minigameTaskListEndPos;

    private Coroutine activeTaskListMoveCoroutine = null;


    //Used for everything that has a hoverable component
    private List<UIObjectHover> minigameTaskListHoverableObjects = new List<UIObjectHover>();

    [SerializeField]
    private GameObject minigameTaskListTaskParent;
    [SerializeField]
    private GameObject minigameTaskListTaskPrefab;
    //Used for the tasks under the header
    private List<TaskListMinigame> minigameTaskListTasks = new List<TaskListMinigame>();
    private List<TaskListMinigame> minigameTaskListEmergencies = new List<TaskListMinigame>();

    [SerializeField]
    private Color minigameTaskListPartialCompleteColor;
    [SerializeField]
    private Color minigameTaskListCompletedColor;

    [SerializeField]
    private Color minigameTaskListEmergencyColor;

    public struct TaskListMinigame
    {
        public GameObject minigame;
        public int index;
    }
    #endregion

    #region Misc Called Objects
    [Header("Misc Called Objects")]

    [SerializeField]
    private Player player;

    [SerializeField]
    private WeaponManager weaponManager;
    [SerializeField]
    private List<WeaponSelectComponents> weaponSelectComponents;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        healthBarFillMaxWidth = healthBarFill.rect.width;
        reloadBarFillMaxWidth = reloadBarFill.rect.width;

        MinigameManager.instance.OnMinigameComplete += OnMinigameComplete;
        MinigameManager.instance.OnMinigameAssign += OnMinigameAssign;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        int _curTask = minigameTaskListTasks.Count + 1;
    //        AddTask("Task " + _curTask, false);
    //    }
    //}

    private void SelectWeapon(int _weapon)
    {
        if (player.isAlive)
        {
            weaponManager.SetWeapon(_weapon);
        }
    }

    public void SetSelectedWeapon(int _weaponNumber)
    {
        //Sets each weapon to the deselected state so I don't have to track the current weapon
        foreach (WeaponSelectComponents _weapon in weaponSelectComponents)
        {
            _weapon.buttonImage.color = _weapon.inactiveBackgroundColor;
            _weapon.numberText.color = _weapon.inactivePrimaryColor;
            _weapon.weaponText.color = _weapon.inactivePrimaryColor;
            _weapon.weaponImage.color = _weapon.inactivePrimaryColor;
        }

        weaponSelectComponents[_weaponNumber].buttonImage.color = weaponSelectComponents[_weaponNumber].activeBackgroundColor;
        weaponSelectComponents[_weaponNumber].numberText.color = weaponSelectComponents[_weaponNumber].activePrimaryColor;
        weaponSelectComponents[_weaponNumber].weaponText.color = weaponSelectComponents[_weaponNumber].activePrimaryColor;
        weaponSelectComponents[_weaponNumber].weaponImage.color = weaponSelectComponents[_weaponNumber].activePrimaryColor;
    }

    public void TakeDamage(float health, float maxHealth)
    {
        //health is a float, but display as int
        healthBarFill.sizeDelta = new Vector2((health / maxHealth) * healthBarFillMaxWidth, healthBarFill.sizeDelta.y);

        healthBarText.SetText("" + (int) health);
    }

    public void Reload(float timer)
    {
        StartCoroutine(ReloadBarFillRoutine(timer));
    }

    public void AddTask(TaskListMinigame _minigameStruct, string _text, int _index, bool _isComplete, bool _isFirst)
    {
        if(_minigameStruct.minigame == null)
        {
            GameObject _taskListTask = Instantiate(minigameTaskListTaskPrefab, minigameTaskListTaskParent.transform);
            //_taskListTask.transform.SetParent(minigameTaskListTaskParent.transform);

            _taskListTask.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * _taskListTask.GetComponent<RectTransform>().sizeDelta.y * minigameTaskListTasks.Count);
            _minigameStruct = new TaskListMinigame { minigame = _taskListTask, index = _index };

            minigameTaskListTasks.Add(_minigameStruct);
        }
        else
        {
            _minigameStruct.index = _index;
        }

        _minigameStruct.minigame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "   -   " + _text;

        if (!_isFirst)
        {
            _minigameStruct.minigame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = minigameTaskListPartialCompleteColor;
        }

        //Overrides the yellow if it is complete
        if (_isComplete)
        {
            _minigameStruct.minigame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = minigameTaskListCompletedColor;
        }
    }

    public void AddEmergency(TaskListMinigame _minigameStruct, string _text, int _index)
    {
        if (_minigameStruct.minigame == null)
        {
            GameObject _taskListTask = Instantiate(minigameTaskListTaskPrefab, minigameTaskListTaskParent.transform);
            //_taskListTask.transform.SetParent(minigameTaskListTaskParent.transform);

            //_taskListTask.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * _taskListTask.GetComponent<RectTransform>().sizeDelta.y * (minigameTaskListEmergencies.Count + minigameTaskListTasks.Count));
            _minigameStruct = new TaskListMinigame { minigame = _taskListTask, index = _index };

            minigameTaskListEmergencies.Add(_minigameStruct);
        }
        else
        {
            _minigameStruct.index = _index;
            Debug.LogWarning("Pretty sure something went wrong...");
        }

        _minigameStruct.minigame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "   -   " + _text;
        _minigameStruct.minigame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = minigameTaskListEmergencyColor;

        PlaceEmergencies();
    }

    public void PlaceEmergencies()
    {
        for(int i = 0; i < minigameTaskListEmergencies.Count; i++)
        {
            GameObject _listObject = minigameTaskListEmergencies[i].minigame;

            _listObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * _listObject.GetComponent<RectTransform>().sizeDelta.y * (minigameTaskListTasks.Count + i));
        }
    }

    public void Die()
    {
        //StopAllCoroutines();
        reloadBar.SetActive(false);
    }

    private IEnumerator ReloadBarFillRoutine(float timer)
    {
        float t = 0;

        reloadBar.SetActive(true);

        while (t < timer)
        {
            t += Time.deltaTime;

            reloadBarFill.sizeDelta = new Vector2((t / timer) * reloadBarFillMaxWidth, reloadBarFill.sizeDelta.y);
            yield return null;
        }

        reloadBar.SetActive(false);
    }

    private IEnumerator CompleteMinigameTextRoutine()
    {
        float t = 0;

        while(t < minigameCompleteTextTimer)
        {
            t += Time.deltaTime;

            if(t < minigameCompleteTextTimer * (1f / 8f))
            {
                minigameCompleteText.anchoredPosition = Vector3.Lerp(minigameCompleteTextStartPos.anchoredPosition, minigameCompleteTextEndPos.anchoredPosition, (t / minigameCompleteTextTimer) * (8f));
            }

            
            yield return null;
        }

        minigameCompleteText.anchoredPosition = minigameCompleteTextStartPos.anchoredPosition;
    }

    public void SetRole(Player.Role _role)
    {
        //SetSelectedWeapon(0);

        if (_role == Player.Role.Traitor)
        {
            weaponSelectComponents[2].container.SetActive(true);
        }
        else
        {
            weaponSelectComponents[2].container.SetActive(false);
        }
    }

    public void AddHoverableObject(UIObjectHover _hoverObjectScript)
    {
        minigameTaskListHoverableObjects.Add(_hoverObjectScript);
    }

    public void HoverEnter()
    {
        //Debug.Log("Hover Enter");
        //Sliiiide to the right

        if(activeTaskListMoveCoroutine != null)
        {
            StopCoroutine(activeTaskListMoveCoroutine);
        }
        
        activeTaskListMoveCoroutine = StartCoroutine(MoveTaskList(true));
    }

    public void HoverExit()
    {
        //Debug.Log("Hover Exit");
        //Check all of the list, then determine whether or not to slide to the left
        foreach(UIObjectHover _uiObjectHover in minigameTaskListHoverableObjects)
        {
            if (_uiObjectHover.isHovered)
            {
                return;
            }
        }

        if (activeTaskListMoveCoroutine != null)
        {
            StopCoroutine(activeTaskListMoveCoroutine);
        }

        activeTaskListMoveCoroutine = StartCoroutine(MoveTaskList(false));
    }

    private IEnumerator MoveTaskList(bool _isGoRight)
    {
        float _t = 0;
        RectTransform _rectTransform = minigameTaskListMainObject.GetComponent<RectTransform>();
        Vector2 _currentPos = _rectTransform.anchoredPosition;

        float _totalDistance = Vector2.Distance(minigameTaskListStartPos.anchoredPosition, minigameTaskListEndPos.anchoredPosition); ;

        if (_isGoRight)
        {
            float _distToEnd = Vector2.Distance(_currentPos, minigameTaskListEndPos.anchoredPosition);
            float _percentComplete = 1 - (_distToEnd / _totalDistance);

            _t = _percentComplete;

            while (_t < 1)
            {
                _t += Time.deltaTime / minigameTaskListMoveTimer;

                _rectTransform.anchoredPosition = Vector3.Lerp(minigameTaskListStartPos.anchoredPosition, minigameTaskListEndPos.anchoredPosition, _t);
                yield return null;
            }

            activeTaskListMoveCoroutine = null;
        }
        else
        {
            
            float _distToEnd = Vector2.Distance(_currentPos, minigameTaskListStartPos.anchoredPosition);
            float _percentComplete = 1 - (_distToEnd / _totalDistance);

            _t = _percentComplete;

            while (_t < 1)
            {
                _t += Time.deltaTime / minigameTaskListMoveTimer;

                _rectTransform.anchoredPosition = Vector3.Lerp(minigameTaskListEndPos.anchoredPosition, minigameTaskListStartPos.anchoredPosition, _t);
                yield return null;
            }

            activeTaskListMoveCoroutine = null;
        }
    }

    private void OnMinigameAssign(object _sender, MinigameManager.OnMinigameAssignArgs _e)
    {
        if (!_e.isEmergency)
        {
            TaskListMinigame _minigameStruct = new TaskListMinigame { minigame = null, index = -1 };

            foreach (TaskListMinigame _minigameStructs in minigameTaskListTasks)
            {
                if (_minigameStructs.index == _e.index)
                {
                    _minigameStruct = _minigameStructs;
                }
            }

            AddTask(_minigameStruct, _e.incompleteText, _e.index, false, _e.isFirst);
        }
        else
        {
            //TDOD: AddEmergency()
            //if isFirst == false, don't add to UI 
            //used for things like the door tasks

            if (!_e.isFirst)
            {
                return;
            }

            TaskListMinigame _minigameStruct = new TaskListMinigame { minigame = null, index = -1 };

            foreach (TaskListMinigame _minigameStructs in minigameTaskListEmergencies)
            {
                if (_minigameStructs.index == _e.index)
                {
                    _minigameStruct = _minigameStructs;
                }
            }

            AddEmergency(_minigameStruct, _e.incompleteText, _e.index);
        }
        
    }

    private void OnMinigameComplete(object _sender, MinigameManager.OnMinigameCompleteEventArgs _e)
    {
        if (!_e.isEmergency)
        {
            StartCoroutine(CompleteMinigameTextRoutine());

            TaskListMinigame _minigameStruct = new TaskListMinigame { minigame = null, index = -1 };
            int _minigameStructIndex = -1;
            int _minigameIndex = -1;

            for (int i = 0; i < minigameTaskListTasks.Count; i++)
            {
                if (minigameTaskListTasks[i].index == _e.index)
                {
                    _minigameIndex = i;
                    _minigameStructIndex = _e.index;
                    _minigameStruct = minigameTaskListTasks[_minigameIndex];
                }
            }

            if (_e.isFinal)
            {
                AddTask(_minigameStruct, _e.completeText, _minigameStruct.index, true, false);
            }
            else
            {
                minigameTaskListTasks[_minigameIndex] = new TaskListMinigame { minigame = _minigameStruct.minigame, index = _minigameStruct.index + 1 };
            }
        }
        else
        {
            //if isFirst is true, then its a door-like task and is not on the player's UI 
            if (!_e.isFirst)
            {
                return;
            }

            TaskListMinigame _minigameStruct = new TaskListMinigame { minigame = null, index = -1 };
            int _minigameStructIndex = -1;
            int _minigameIndex = -1;

            for (int i = 0; i < minigameTaskListEmergencies.Count; i++)
            {
                if (minigameTaskListEmergencies[i].index == _e.index)
                {
                    _minigameIndex = i;
                    _minigameStructIndex = _e.index;
                    _minigameStruct = minigameTaskListEmergencies[_minigameIndex];
                }
            }

            //Destroy specific emergency
            Destroy(_minigameStruct.minigame);
            minigameTaskListEmergencies.RemoveAt(_minigameIndex);

            //Replace existing emergencies
            PlaceEmergencies();
        }
    }

    private List<TaskListMinigame> ClearMinigameTaskList(List<TaskListMinigame> _taskList)
    {
        foreach(TaskListMinigame _minigameStruct in _taskList)
        {
            Destroy(_minigameStruct.minigame);
        }

        return new List<TaskListMinigame>();
    }

    private void OnDestroy()
    {
        //Unsub from events
        MinigameManager.instance.OnMinigameComplete -= OnMinigameComplete;
        MinigameManager.instance.OnMinigameAssign -= OnMinigameAssign;
    }

    
}
