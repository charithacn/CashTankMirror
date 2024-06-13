using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Button solobutton;
    [SerializeField] private Button vsbutton;
    [SerializeField] LoadScene loadScene;
    [SerializeField] private AudioSource mainAudioSource;

    [SerializeField] private Image soundButton;

    [SerializeField] private InputField nameField;
    bool audio;
    
    
    void Start()
    {

        nameField.text = PlayerPrefs.GetString("Name");
        solobutton.onClick.AddListener(SoloButtonClick);
        vsbutton.onClick.AddListener(VsButtonClick);
    }

    private void VsButtonClick()
    {
        PlayerPrefs.SetString("GameStatus", "vs");
        PlayerPrefs.Save();
        loadScene.SendMessage("LoadSceneAsync", "Server");
    }

    private void SoloButtonClick()
    {
        PlayerPrefs.SetString("GameStatus", "solo");
        PlayerPrefs.Save();
        loadScene.SendMessage("LoadSceneAsync", "Server");
        
    }
    public void OnInputValueChange()
    {
        PlayerPrefs.SetString("Name", nameField.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AudioManage()
    {
        //if (PlayerPrefs.GetInt("audio", 0) == 0)
        //{
        //    PlayerPrefs.SetInt("audio", 1);
        //    PlayerPrefs.Save();
        //    mainAudioSource.volume = 0;
        //    soundButton.sprite = soundOff;
        //}
        //else
        //{
        //    PlayerPrefs.SetInt("audio", 0);
        //    PlayerPrefs.Save();
        //    mainAudioSource.volume = 1;
        //    soundButton.sprite = soundOn;
        //}
    }
    
}
