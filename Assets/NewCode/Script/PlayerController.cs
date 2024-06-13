using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoomTanks.Multiplayer
{
    public class PlayerController
    {
        private Player player;
        private bool isPlay;
        public bool IsPlayer
        {
            get { return isPlay; }
            set { isPlay = value; }
        }

        public PlayerController(Player player)
        {
            this.player = player;
        }


        public void Move(Vector2 position)
        {

            if (isPlay)
                player.Move(position);

        }
        public void ShootMove(Vector2 value)
        {
            if (isPlay)
                player.ShootRotation(value);
        }
    }
}
