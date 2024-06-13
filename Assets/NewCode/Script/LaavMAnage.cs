using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace BoomTanks.Multiplayer
{
    public class LaavMAnage : NetworkBehaviour
    {
        
        public float duration = 600f; // 5 minutes in seconds
        private float elapsedTime = 0f;

        private readonly SyncVar<Vector3> lavaValue = new SyncVar<Vector3>();
        void Update()
        {
            if (!IsServer) return;
            elapsedTime += Time.deltaTime;

            // Calculate the percentage of time elapsed
            float t = elapsedTime / duration;

            // Ensure t doesn't exceed 1
            t = Mathf.Clamp01(t);

            // Interpolate between bottom and top of the screen
            Vector3 startPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, -35, Camera.main.transform.position.z)); // Assumes object starts at bottom of the screen
            Vector3 endPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, Camera.main.transform.position.z)); // Assumes object ends at top of the screen
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);
            lavaValue.Value = newPosition;
            // Convert to 2D position
            transform.position = new Vector3(lavaValue.Value.x, lavaValue.Value.y, 0);

            // Check if duration has passed
            if (elapsedTime >= duration)
            {
                // Reset timer or handle end of movement
                elapsedTime = 0f;
            }
        }
    }
}
