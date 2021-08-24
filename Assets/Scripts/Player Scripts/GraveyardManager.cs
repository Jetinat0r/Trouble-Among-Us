using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardManager : MonoBehaviour
{
    public static GraveyardManager instance;

    [SerializeField]
    private List<Transform> graves = new List<Transform>();
    private int curGrave = 0;

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

    public Transform GetNextGravePosition()
    {
        curGrave++;
        return graves[curGrave - 1];
    }

    public void ResetGraveyard()
    {
        curGrave = 0;
    }
}
