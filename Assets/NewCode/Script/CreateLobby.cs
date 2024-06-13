using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;
using UnityEngine.UI;

namespace BoomTanks.Multiplayer
{
    public class CreateLobby : NetworkBehaviour
    {
        string lobbyName = "new lobby";
        int maxPlayers = 4;

        public Button createLobby;
        public Button joinLobby;

      


    }      
}
