using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
// using Unity.Services.Multiplay;
using Unity.Services.Core;
using Unity.Services.Authentication; */

public class PlayerNetwork : NetworkBehaviour
{
    public GameObject target;
    public Movement playerScript;
    public Enemy enemyScript;
    public ManageRefills mr;
    public bool targetEnemy;
    public readonly SyncVar<int> currentlyShooting = new SyncVar<int>();
    public readonly SyncVar<float> GunRotationNetwork = new SyncVar<float>();
    public readonly SyncVar<float> playerX =  new SyncVar<float>();
    public readonly SyncVar<float> playerY = new SyncVar<float>();
    public readonly SyncVar<float> playerD = new SyncVar<float>();
    public readonly SyncVar<float> playerH = new SyncVar<float>();
    public readonly SyncVar<byte> map = new SyncVar<byte>();

    /*#if DEDICATED_SERVER
        bool alreadyAutoAllocated;
    #endif

        async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions io = new InitializationOptions();
                await UnityServices.InitializeAsync(io);

    #if !DEDICATED_SERVER
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
    #endif

    #if DEDICATED_SERVER
                MultiplayEventCallbacks mec = new MultiplayEventCallbacks();
                mec.Allocate += MultiplayEventCallbacks_Allocate;
                mec.Deallocate += MultiplayEventCallbacks_Deallocate;
                mec.Error += MultiplayEventCallbacks_Error;
                mec.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
                IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(mec);

                // IServerQueryHandler serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "MyServerName", "TanksForNothing", "Build01", "0");

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "")
                {
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
                }
    #endif

            }
        }

        void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj)
        {
            if (alreadyAutoAllocated)
            {
                return;
            }

            alreadyAutoAllocated = true;

            var serverConfig = MultiplayService.Instance.ServerConfig;
            // Debugs dot logs
            string ipv4Address = "0.0.0.0";
            ushort port = serverConfig.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

            // KitchenGameMultiplayer.Instance.StartServer();
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_Connection;
        }*/

    public override void OnStartNetwork()
    {
        target = GameObject.Find("Player");

        if (target != null)
        {
            if (target.GetComponent<Movement>() != null)
            {
                playerScript = target.GetComponent<Movement>();
                playerScript.onlinePlayer = this;
            }
        }
    }

    /*   void ServerUpdate()
       {
           if (ammo.Value + Time.fixedDeltaTime > 4)
               ammo.Value = 4;
           else
               ammo.Value = ammo.Value + Time.fixedDeltaTime;

           // transform.position = target.transform.position;
       }*/

    void Update()
    {
        //GunRotation = GunRotationNetwork.Value;

        if (IsOwner)
        {
            playerX.Value = target.transform.position.x;
            playerY.Value = target.transform.position.y;
            playerD.Value = target.transform.rotation.eulerAngles.z;
            playerH.Value = playerScript.Health;
            GunRotationNetwork.Value = playerScript.Gun.transform.rotation.eulerAngles.z;

            if (currentlyShooting.Value > 0)
                currentlyShooting.Value--;

            if (playerScript.currentlyShooting)
            {
                playerScript.currentlyShooting = false;
                currentlyShooting.Value = 4;
            }

            map.Value = (byte)playerScript.playerHandler.mapNumber;
        }
        else if (playerScript.playerHandler.Enemies.Count > (int)base.Owner.ClientId/* && playerScript.playerHandler.Present[(int)OwnerClientId]*/)
        {
            if (target != null)
            {
                if (targetEnemy)
                {
                    
                }
                else
                {
                    targetEnemy = true;
                    target = target.GetComponent<Movement>().playerHandler.Enemies[(int)base.Owner.ClientId];
                    enemyScript = playerScript.playerHandler.Enemies[(int)base.Owner.ClientId].GetComponent<Enemy>();
                    enemyScript.onlineController = gameObject;
                }

                target.transform.position = new Vector2(playerX.Value, playerY.Value);
                target.transform.rotation = Quaternion.Euler(0, 0, playerD.Value);
                enemyScript.Gun.transform.rotation = Quaternion.Euler(0, 0, GunRotationNetwork.Value);
                enemyScript.Health = playerH.Value;

                if (currentlyShooting.Value > 0 && enemyScript.ExternallyShooting == 0)
                {
                    enemyScript.ExternallyShooting = currentlyShooting.Value;
                }

                if (enemyScript.onlineController == null)
                    enemyScript.onlineController = gameObject;
            }
            else
            {
                target = playerScript.playerHandler.Enemies[(int)base.Owner.ClientId];
                targetEnemy = true;
                enemyScript = target.GetComponent<Enemy>();
            }
        }
        else
        {
            targetEnemy = false;
            playerScript.playerHandler.mapOnline = true;
            playerScript.playerHandler.mapNumber = (int)map.Value;
        }
    }
}