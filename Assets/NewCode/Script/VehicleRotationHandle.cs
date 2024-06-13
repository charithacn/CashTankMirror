using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoomTanks.Multiplayer
{
    public class VehicleRotationHandle : MonoBehaviour
    {
        bool ground;
        public float groundCheckRadius;
        public LayerMask groundLayer;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ground = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
            GameObject.Find("Player").GetComponent<Player>().SendMessage("ChangeRotation", ground);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // ground = false;
            //GameObject.Find("Player").GetComponent<Player>().SendMessage("ChangeRotation", ground);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            //ground = true;
            // GameObject.Find("Player").GetComponent<Player>().SendMessage("ChangeRotation", ground);
        }
    }
}
