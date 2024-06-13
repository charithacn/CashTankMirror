using FishNet;
using FishNet.Managing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] Button serverBtn;
    [SerializeField] Button hostBtn;
    [SerializeField] Button clientBtn;

    void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.ServerManager.StartConnection();
        });
        hostBtn.onClick.AddListener(() =>
        {
           // NetworkManager.Singleton.StartHost();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.ClientManager.StartConnection();
        });
    }
}