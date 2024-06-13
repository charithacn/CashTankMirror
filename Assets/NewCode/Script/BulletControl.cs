using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptBoy.Digable2DTerrain;
using FishNet.Object;

namespace BoomTanks.Multiplayer
{
    public class BulletControl : NetworkBehaviour
    {
        private Rigidbody2D rb;
        [SerializeField] private ParticleSystem bomb;
        [SerializeField] private GameObject shovel;

        public MultiplayPlayer multiplayPlayer;
        [SerializeField] private AudioSource shoot;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

        }

        void FixedUpdate()
        {
            // Rotate bullet based on velocity
            if (rb.velocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.Rotate(Vector3.forward * -90f);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if (!collision.tag.Equals("Missile"))
            {
                Transform objTransform = transform;

                // Loop through all parents until we reach the topmost parent (which has no parent)
                while (objTransform.parent != null)
                {
                    // Set the parent to null, effectively detaching the object
                    objTransform.SetParent(null);
                }
                StartCoroutine(DigWait());
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                //multiplayPlayer.SendMessage ("Attache");
                StartCoroutine(BulletDestroy());
            }
            if (collision.tag.Equals("Player"))
            {

                if (IsServer)
                {
                    SetMarksServerRpc();
                }
            }

            //Destroy(this.gameObject);
        }
       IEnumerator DigWait()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject obs;
            obs = GameObject.Find("Shovel");
            obs.transform.position = transform.GetChild(0).transform.position;
            obs.GetComponent<Shovel>().Dig();
            if (PlayerPrefs.GetInt("Music") == 0)
                shoot.Play();
        }
        [ServerRpc]
        void SetMarksServerRpc()
        {
            multiplayPlayer.SendMessage("FireToClientServerRpc");
        }
        IEnumerator BulletDestroy()
        {
            bomb.Play();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(0.9f);
            Destroy(this.gameObject);
        }
    }
}
