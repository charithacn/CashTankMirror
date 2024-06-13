using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoomTanks.Multiplayer
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager instance;

        public Player player;
        [SerializeField]
        private BulletManager bulletManager;
        [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private GameObject lostPanel;
        [SerializeField]
        private LoadScene loadScene;

        [SerializeField]
        private Transform spawnPoint;

        void Start()
        {
            //transform.position = GameObject.Find("Position").transform.position;
            player = GameObject.Find("Player").GetComponent<Player>();
            bulletManager = GameObject.Find("BulletManager").GetComponent<BulletManager>();
            lostPanel = GameObject.Find("YOU LOST");
            loadScene = GameObject.Find("Load Scene").GetComponent<LoadScene>();
            lostPanel.SetActive(false);
            Application.targetFrameRate = 60;
            instance = GetComponent<GameManager>();
            playerController = new PlayerController(player);
            playerController.IsPlayer = true;
            if (IsServer)
            {
                //SpawnPlayer();
            }
        }
        void SpawnPlayer()
        {
            GameObject.Find("Player").transform.position = spawnPoint.position;
        }
        // Update is called once per frame
        void Update()
        {

        }

        //manage player
        void PlayerMovement(Vector2 movement)
        {
            playerController.Move(movement);
        }
        void ShootMove(Vector2 value)
        {
            playerController.ShootMove(value);
        }
        void bulletManage(int numberOfBullet)
        {
            bulletManager.SendMessage("LoadImage", numberOfBullet);
        }

        void Lost()
        {
            playerController.IsPlayer = false;
            lostPanel.SetActive(true);
        }
        void Restart()
        {
            loadScene.LoadSceneAsync("Start");
        }
    }
}
