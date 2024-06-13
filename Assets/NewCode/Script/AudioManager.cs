using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AudioManager : FishNet.Object.NetworkBehaviour
{
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioSource music;

    public Button audioButton;
    public Button musicButton;


    // Start is called before the first frame update
    void Start()
    {
        audioButton.onClick.AddListener(AudioManage);
        musicButton.onClick.AddListener(MusicManage);
        if (PlayerPrefs.GetInt("Music") == 0)
        {
            audio.volume = 1;
            musicButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            audio.volume = 0;
            musicButton.GetComponent<Image>().color = new Color32(190, 190, 190, 255);
        }
        if (PlayerPrefs.GetInt("Audio") == 0)
        {
            audio.volume = 1;
            audioButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            audio.volume = 0;
            audioButton.GetComponent<Image>().color = new Color32(190, 190, 190, 255);
        }
    }

    private void MusicManage()
    {
        if(PlayerPrefs.GetInt("Music")==0)
        {
            music.volume = 0;
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.Save();
            musicButton.GetComponent<Image>().color = new Color32(190, 190, 190, 255);
        } else
        {
            music.volume = 1;
            PlayerPrefs.SetInt("Music", 0);
            PlayerPrefs.Save();
            musicButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    private void AudioManage()
    {
        if (PlayerPrefs.GetInt("Audio") == 0)
        {
            audio.volume = 0;
            PlayerPrefs.SetInt("Audio", 1);
            PlayerPrefs.Save();
            audioButton.GetComponent<Image>().color = new Color32(190, 190, 190, 255);
        }
        else
        {
            audio.volume = 1;
            PlayerPrefs.SetInt("Audio", 0);
            PlayerPrefs.Save();
            audioButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
