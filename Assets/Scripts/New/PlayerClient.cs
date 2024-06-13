using FishNet.Object;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace BoomTanks.Multiplayer
{
    public class PlayerClient : NetworkBehaviour
    {
        public Transform clientPositionMainObject;

        [SerializeField]
        private InputActionReference playerMovementAction;
        [SerializeField]
        private InputActionReference playerShootPreference;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!IsClient) return;
            Vector2 rightStickValue = playerShootPreference.action.ReadValue<Vector2>();
            Vector2 leftStickValue = playerMovementAction.action.ReadValue<Vector2>();
            print(leftStickValue);
        }
        public override void OnStartClient()
        {
            ServerManage serverManage = GameObject.Find("ServerManager").GetComponent<ServerManage>();
            serverManage.SendMessage("SetClientCount", 1);
            clientPositionMainObject = GameObject.Find("positions").transform;
           // transform.position = clientPositionMainObject.transform.GetChild(serverManage.NumberOfCliens).transform.position;
        }
    }
}

