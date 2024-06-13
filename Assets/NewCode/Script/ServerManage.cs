using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace BoomTanks.Multiplayer
{
    public class ServerManage : NetworkBehaviour
    {
        public static ServerManage instance;

        public readonly SyncVar<int> numberOfClientsValue = new SyncVar<int>();

        public readonly SyncVar<int> numberOfClients = new SyncVar<int>();

        public readonly SyncVar<int> randomNumbers = new SyncVar<int>();
        public readonly SyncVar<bool> gameStartBool = new SyncVar<bool>();
        public readonly SyncVar<int> clientNetworkId = new SyncVar<int>();

        public int localClientNetworkId = 0;

       // public NetworkVariable<string> matchId = new NetworkVariable<string>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        // NetworkVariable<List<int>> winList = new NetworkVariable<List<int>>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public List<int> winList;
        public List<MultiplayPlayer> playerlist;
        [SerializeField] private Text playerCount;
        [SerializeField] private GameObject[] bgs;

        [SerializeField] private Text enterPlayText;
        [SerializeField] private int maximumCount;
        [SerializeField] private GameObject enterScreen;

        private bool startGameToEnterScreen;
        [SerializeField] private GameObject winObject;
        [SerializeField] private GameObject lostObject;

        //public int ID;
        public List<string> winnerList;
        int numberOfPlayers = 0;

        public int place;

        private void Awake()
        {
           // NetworkManager.Singleton.StartServer(); 
           // DateTime now = DateTime.Now;
            //if(IsServer)
           // matchId.Value = now.ToString("yyyyMMddHHmmss");
        }
        public void SetScore()
        {

        }
        void SetPlace(int val) { 
        }
        private void Start()
        {
            startGameToEnterScreen = false;
            instance = GetComponent<ServerManage>();
            winList = new List<int>();
            playerlist = new List<MultiplayPlayer>();
            winnerList = new List<string>();
           
            numberOfClientsValue.Value = 0;
            numberOfClients.Value = 0;
            randomNumbers.Value = 0;
            gameStartBool.Value = false;
            clientNetworkId.Value = 0;
            localClientNetworkId = 0;
        }
        void SetPlayerCount(string value)
        {
            //playerCount.text = value;
        }
        
        public override void OnStartClient()
        {
            if (IsServer)
            {
                int val = UnityEngine.Random.Range(0, 5);
                randomNumbers.Value = val;
                print("Loading bg " + val);
                DateTime now = DateTime.Now;

                // Generate a unique ID using date and time
                string uniqueID = string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D3}",
                                                 now.Year, now.Month, now.Day,
                                                 now.Hour, now.Minute, now.Second,
                                                 now.Millisecond);
                localClientNetworkId = clientNetworkId.Value;
                ++clientNetworkId.Value;
            }

            for (int i = 0; i < bgs.Length; i++)
            {
                bgs[i].SetActive(false);
            }
            bgs[randomNumbers.Value].SetActive(true);
        }
        private void Update()
        {
            if(IsServer)
            {
                try
                {
                    numberOfClients.Value = ClientManager.Clients.Count;
                } catch(Exception ex)
                {

                }
                
                print("in server value ;" + numberOfClients.Value);
                enterPlayText.text ="Players " +numberOfClients.Value + "/" + maximumCount;
                if (numberOfClients.Value == maximumCount)
                {
                   enterScreen.SetActive(false);
                    gameStartBool.Value = true;
                    
                }
            }
            if(IsClient)
            {
                print("in server value ;" + numberOfClients.Value);
                
                enterPlayText.text = "Players " + numberOfClients.Value + "/" + maximumCount;
                if (numberOfClients.Value == maximumCount && startGameToEnterScreen==false)
                {
                    startGameToEnterScreen = true;
                    StartCoroutine(StartScreenEnabled());
                }
            }
            playerCount.text = numberOfClients.Value.ToString();
        }
        IEnumerator StartScreenEnabled()
        {
            yield return new WaitForSeconds(2.5f);
            enterScreen.SetActive(false);
        }
        void ClientCount()
        {
            if(IsServer)
            ++numberOfClientsValue.Value;
        }
        void ClientCountRemove()
        {
            if (IsServer) --numberOfClientsValue.Value;
        }
        void SpawnManage()
        {
            print("Instantiate the bullet position" );
        }
 
        void PlayerList(string value)
        {
           

        }
        void AddClient(int val)
        {
            print("Add client");
           // List<int> clients = winList.Value;
           winList.Add(val);
            //ID = val;
        }
        public void AddPlayer(MultiplayPlayer player)
        {
            playerlist.Add(player);
        }
        public void RemovePlayer(MultiplayPlayer player)
        {
            ClientCountRemove();
            winnerList.Add(player.username);
            print("Player username " + player.username);
            //playerlist.Remove(player);
            //Destroy(player.gameObject);
            //playerlist.RemoveAll(item => item == null);
            //foreach(MultiplayPlayer val in  playerlist) {
            //    if(val!=null & numberOfClientsValue.Value==1)
            //    {
            //        if(IsServer)
            //            val.winner.Value = true;
            //        break;
            //    }
            //}

        }
        public void AddwinnerListPref(MultiplayPlayer player)
        {
           // winnerList.Add(player.username);
        }
        void DestroyClient(int val)
        {
            // List<int> clients = winList.Value;
            print("Remove Client " + val);
            
            winList.Remove(val);
            winList.RemoveAll(item => item == 0);
            print("Winner list Client " + val);
            //if (winList.Count==1)
            //{
            //    if(IsClient)
            //    {
            //        if(ID==val)
            //        {
            //            //WinSetup();
            //        }
            //        print("You are winner client");
            //    }
            //    if(IsServer)
            //    {
            //        playerlist[0].winner.Value = true;
            //        print("You are winner in server");
            //    }

            //}
        }
        void WinSetup()
        {           
            winObject.SetActive(true);
            lostObject.transform.GetChild(0).GetComponent<Text>().text = "Winner";

            //for (int i = 0; i < winnerList.Count; i++)
            //{
            //    winObject.transform.GetChild(2).transform.GetChild(0).transform.GetChild(i).GetComponent<Text>().text = winnerList[i];
            //}

        }
        void RestartServer ()
        {
            StartCoroutine(ReatartScene());
        }
        IEnumerator ReatartScene()
        {
            yield return new WaitForSeconds(3f);
            //NetworkManager.Singleton.Shutdown();
            yield return new WaitForSeconds(2f);
            Awake();
           Start();
            OnStartClient();
          //GameObject.Find("SceneManager").GetComponent<LoadScene>().LoadSceneAsync("empty");
        }

        void Lost()
        {
            if(numberOfClients.Value>2)
            {
                lostObject.SetActive(true);
                lostObject.transform.GetChild(0).GetComponent<Text>().text = "You are lost";
            } else if(numberOfClients.Value==2)
            {
                lostObject.SetActive(true);
                lostObject.transform.GetChild(0).GetComponent<Text>().text = "Runner Up";
            } 
            else if(numberOfClients.Value==1)
            {
               // WinSetup();
            }
            
        }
        void SecondPlace()
        {
            
        }
    }


    public struct PositionList
    {
        public List<Vector2> positions;
    }

}



