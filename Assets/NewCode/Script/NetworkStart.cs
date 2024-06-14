
using FishNet.Managing;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BoomTanks.Multiplayer
{
    public class NetworkStart : NetworkBehaviour
    {
        [SerializeField] private  Statues networkStatus;
        bool isGameStart = false;

        public GameObject gameManager;
        public Button serverButton;
        public Button clientButton;
        public Button hostButton;
        public InputField ipAddress;

       public  NetworkManager networkManager;
        void Start()
        {

            serverButton.onClick.AddListener(ServerStartAction);
            clientButton.onClick.AddListener(ClientStartAction);
            hostButton.onClick.AddListener(HostStartAction);
            print("Start service " + networkStatus);
           // NetworkManager.Singleton.StartServer();
            //switch (networkStatus)
            //{
            //    case Statues.client:
            //        NetworkManager.Singleton.StartClient();
            //        break;
            //    case Statues.server:
            //        NetworkManager.Singleton.StartServer();
            //        break;
            //    case Statues.host:
            //        NetworkManager.Singleton.StartHost();
            //        break;
            //}
            isGameStart = false;
            }

        private void HostStartAction()
        {
            //NetworkManager.Singleton.StartHost();
            //NetworkManager.host
        }

        private void LoadSceneAction()
        {
           // NetworkManager.Sc.LoadScene("Server", LoadSceneMode.Additive);
        }

        private void ServerStartAction()
        {

            NetworkManager.ServerManager.StartConnection();
            StartCoroutine(LoadScenes());
        }

        private void ClientStartAction()
        {
            NetworkManager.ClientManager.StartConnection();
            StartCoroutine(LoadScenes());
        }
        IEnumerator LoadScenes()
        {
            yield return new WaitForSeconds(3f);
            
        }
        void StartTheGame()
        {
            //GameObject manager = Instantiate(gameManager);
            //GameManager gameManage = manager.GetComponent<GameManager>();
        }
        private void Update()
        {
            
        }
        public enum Statues
        {
            client,
            server,
            host
        }
    }
    
}


