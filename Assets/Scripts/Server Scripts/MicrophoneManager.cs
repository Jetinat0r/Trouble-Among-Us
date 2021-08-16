using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    public static MicrophoneManager instance;

    private string currentMicrophone = null;
    private int minFreq = 44100;
    private int maxFreq = 44100;

    private AudioClip heldAudioClip;
    public AudioSource tempAudioPlayer;

    private bool isRecording = false;
    private bool hasClip = false;
    private bool isInGame = false;

    private float timer = 0;

    #region Local Player Specifics
    public bool isRadioActive = false;
    #endregion

    //[SerializeField, Range(0f, 10f)]
    //private float m_gain = 1f;
    //private float m_volumeRate;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying!");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Client.instance.OnServerConnect += OnServerConnect;
        Client.instance.OnServerDisconnect += OnServerDisconnect;
    }

    #region Unused Mic Logic
    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        isRadioActive = !isRadioActive;
    //    }

    //    if (Input.GetKey(KeyCode.V))
    //    {

    //        if (isRecording)
    //        {
    //            if(timer >= 1)
    //            {
    //                SendClip(true);
    //            }
    //        }
    //        else
    //        {
    //            isRecording = true;
    //            hasClip = true;

    //            timer = 0;

    //            heldAudioClip = Microphone.Start(currentMicrophone, false, 1, maxFreq);
    //        }

    //        timer += Time.deltaTime;
    //    }

    //    if(Input.GetKeyUp(KeyCode.V) && hasClip)
    //    {
    //        isRecording = false;

    //        SendClip(false);
    //    }
    //}
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isRadioActive = !isRadioActive;
        }

        if (Input.GetKey(KeyCode.V))
        {

            if (isRecording)
            {
                //SendClip(true);
            }
            else
            {
                isRecording = true;
                hasClip = true;

                timer = 0;

                heldAudioClip = Microphone.Start(currentMicrophone, false, 1, maxFreq);
            }

            //timer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            isRecording = false;

            if (hasClip)
            {
                SendClip(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isRecording && timer >= 0.2)
        {
            timer = 0;

            SendClip(true);
        }

        timer += Time.fixedDeltaTime;
    }

    private void SendClip(bool _keepRecording)
    {   
        int position = Microphone.GetPosition(currentMicrophone);

        Microphone.End(currentMicrophone);

        if(isInGame && position > 0)
        {
            float[] heldData = new float[heldAudioClip.samples * heldAudioClip.channels];
            heldAudioClip.GetData(heldData, 0);

            float[] samplesToSend = new float[position * heldAudioClip.channels];
            for (int i = 0; i < samplesToSend.Length; i++)
            {
                samplesToSend[i] = heldData[i];
            }

            ClientSend.ClientSendVoiceChat(samplesToSend, position, heldAudioClip.channels, maxFreq, isRadioActive);
        }




        #region Unused Send Logic
        //float[] samplesToSend = new float[heldAudioClip.samples * heldAudioClip.channels];
        //heldAudioClip.GetData(samplesToSend, 0);

        //  ---  HANDLE IN PLAYER MANAGER  ---
        //for (int i = 0; i < samplesToSend.Length; i++)
        //{
        //    samplesToSend[i] = samplesToSend[i] * 25;
        //}
        //
        //heldAudioClip.SetData(samplesToSend, 0);

        //AudioClip voiceClip = AudioClip.Create("Voice", heldAudioClip.samples, heldAudioClip.channels, maxFreq, false);
        //voiceClip.SetData(samplesToSend, 0);

        //ClientSend.ClientSendVoiceChat(samplesToSend, heldAudioClip.samples, heldAudioClip.channels, maxFreq, isRadioActive);


        //tempAudioPlayer.clip = voiceClip;
        //tempAudioPlayer.clip.SetData(samplesToSend, 0);
        //tempAudioPlayer.clip.SetData(clipData, 0);
        //tempAudioPlayer.Play();
        #endregion

        hasClip = false;

        if (_keepRecording)
        {
            heldAudioClip = Microphone.Start(currentMicrophone, false, 1, 44100);

            timer = 0;
        }
    }

    public void SetCurrentMicrophone(string _microphoneName)
    {
        currentMicrophone = _microphoneName;

        Microphone.GetDeviceCaps(currentMicrophone, out minFreq, out maxFreq);
    }

    //NOTE: Fairly certain that a script that contains this must be attatched to an object with an audio source to function properly
    //
    //private void OnAudioFilterRead(float[] data, int channels)
    //{
    //    float sum = 0f;

    //    for(int i = 0; i < data.Length; i++)
    //    {
    //        sum += Mathf.Abs(data[i]);
    //    }

    //    m_volumeRate = Mathf.Clamp01(sum * m_gain / (float)data.Length);
    //}

    public string GetCurrentMicrophone()
    {
        return currentMicrophone;
    }

    private void OnServerConnect(object sender, EventArgs e)
    {
        isInGame = true;
    }

    private void OnServerDisconnect(object sender, EventArgs e)
    {
        isInGame = false;
    }

    private void OnDestroy()
    {
        Client.instance.OnServerConnect -= OnServerConnect;
        Client.instance.OnServerDisconnect -= OnServerDisconnect;
    }
}
