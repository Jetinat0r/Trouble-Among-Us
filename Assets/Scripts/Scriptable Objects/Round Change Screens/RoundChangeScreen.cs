using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Round Change Screen", menuName = "My Scriptable Objects/Round Change Screen")]
public class RoundChangeScreen : ScriptableObject
{
    public GameObject roundChangeScreenPrefab;
    private GameObject instantiatedCutscene;
    public float cutsceneTimer;

    public delegate void PreActionDelegate();
    public delegate void PostActionDelegate();

    public PreActionDelegate preActionDelegate;
    public PostActionDelegate postActionDelegate;

    public event EventHandler OnCutsceneEnd;

    //Use this for start/end round screen scripting?
    public void StartCutscene(MonoBehaviour caller)
    {
        instantiatedCutscene = Instantiate(roundChangeScreenPrefab, Camera.main.transform);

        if (preActionDelegate != null)
        {
            preActionDelegate();
        }

        caller.StartCoroutine(PlayCutscene(cutsceneTimer));

        Player.instance.StartCutscene();
    }

    private IEnumerator PlayCutscene(float cutsceneTimer)
    {
        while(cutsceneTimer > 0)
        {
            cutsceneTimer -= Time.deltaTime;
            yield return null;
        }

        EndCutscene();
    }

    private void EndCutscene()
    {
        if(postActionDelegate != null)
        {
            postActionDelegate();
        }

        OnCutsceneEnd?.Invoke(this, EventArgs.Empty);

        Player.instance.EndCutscene();

        Destroy(instantiatedCutscene);
    }
}
