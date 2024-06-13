using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BoomTanks.Multiplayer
{
    public class InputManager : NetworkBehaviour
    {
        [SerializeField]
        private InputActionReference playerMovementAction;
        [SerializeField]
        private InputActionReference playerShootPreference;
        [SerializeField]
        private Button tryAgainButton;

        void Start()
        {
            //tryAgainButton.onClick.AddListener(TryAgain);
        }

        private void TryAgain()
        {
            GameManager.instance.SendMessage("Restart");
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsOwner) return;

            Vector2 rightStickValue = playerShootPreference.action.ReadValue<Vector2>();
            Vector2 leftStickValue = playerMovementAction.action.ReadValue<Vector2>();
            MultiplayPlayer player = FindObjectOfType<MultiplayPlayer>();
            if (player != null)
            {
                player.SendMessage("Move", leftStickValue);
                player.SendMessage("ShootRotation", rightStickValue);
            }

            if (IsClient)
            {
                //GameManager.instance.SendMessage("PlayerMovement", leftStickValue);

                //GameManager.instance.SendMessage("ShootMove", rightStickValue);
            }

        }
    }
}