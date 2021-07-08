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
    private List<GameObject> minigameTaskListTasks = new List<GameObject>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            int _curTask = minigameTaskListTasks.Count + 1;
            AddTask("Task " + _curTask, false);
        }
    }

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

    public void AddTask(string _text, bool _isComplete)
    {
        GameObject _taskListTask = Instantiate(minigameTaskListTaskPrefab, minigameTaskListTaskParent.transform);
        //_taskListTask.transform.SetParent(minigameTaskListTaskParent.transform);

        _taskListTask.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * _taskListTask.GetComponent<RectTransform>().sizeDelta.y * minigameTaskListTasks.Count);
        minigameTaskListTasks.Add(_taskListTask);

        _taskListTask.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "   -   " + _text;
    }

    private void OnMinigameComplete(object _sender, MinigameManager.OnMinigameCompleteEventArgs _e)
    {
        if (!_e.isEmergency)
        {
            StartCoroutine(CompleteMinigameTextRoutine());
        }

        Debug.Log(_e.isFinal);
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
            AddTask(_e.incompleteText, false);
        }
        else
        {

        }
        
    }

    private void OnDestroy()
    {
        //Unsub from events
        MinigameManager.instance.OnMinigameComplete -= OnMinigameComplete;
        MinigameManager.instance.OnMinigameAssign -= OnMinigameAssign;
    }
}
