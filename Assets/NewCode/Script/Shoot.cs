using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoomTanks.Multiplayer
{
    public class Shoot : NetworkBehaviour
    {
        bool isFireToGround;
        GameObject instance;
        [SerializeField] private GameObject bullet;
        [SerializeField] private Transform bulletParent;
        public Queue<GameObject> bulletList;
        public Transform bulletPosition;

        [SerializeField] private MultiplayPlayer multiplayPlayer;
        public int numberOfBullet;
        public bool startFireV;
        bool startFire = false;
        public bool isFires;

        float gunAngle;
        int bulletCount = 0;
        int destroyedBilletNumber;
        Vector2 gunPos;
        public int loadOfMissile = 3;
        public float duration = 3f;
        public AudioSource source;
        public AudioSource nozzle;


        private bool isRotate; public bool IsRotate
        {
            get { return isRotate; }
            set { isRotate = value; }
        }
        void Start()
        {

            instance = this.gameObject;
 

        }
        private void Update()
        {

        }
        public void OnNetworkSpawns()
        {
            instance = this.gameObject;
            //for (int i = 0; i < numberOfBullet; i++)
            //{
            //    GameObject ob = Instantiate(bullet);
            //    ob.GetComponent<NetworkObject>().Spawn(true);
            //    //ob.transform.parent = GameObject.Find("Gun").transform;
            //    ob.transform.localPosition = GameObject.Find("Gun").transform.position;
            //    ob.transform.localRotation = Quaternion.Euler(0, 0, 90);
            //    bulletList.Enqueue(ob);
            //    ob.SetActive(false);
            //    ++bulletCount;
            //}
            startFire = false;
            // multiplayPlayer.SendMessage("OnFireServerRpc", startFire);
        }
        void LoadMisile()
        {
            //if (bulletCount < 3)
            // StartCoroutine(ReloadMisileWait());
        }
        void Count()
        {
            int value = 0;
            for (int i = 0; i < 10; i++)
            {
                Debug.Log("Number is : " + value);
                ++value;
            }
        }

        IEnumerator ReloadMisileWait()
        {
            ++bulletCount;
            yield return new WaitForSeconds(3f);
            GameObject ob = Instantiate(bullet);
            ob.SetActive(true);
            var instanceNetworkObject = ob.GetComponent<NetworkObject>();

            //instanceNetworkObject.Spawn();
            ob.transform.parent = transform;
            ob.transform.localPosition = new Vector2(-5.15f, -0.01f);
            ob.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            bulletList.Enqueue(ob);


        }
        IEnumerator LoadMissile(string name)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            Image image = GameObject.Find(name).transform.GetChild(0).GetComponent<Image>();
            while (Time.time < endTime)
            {
                // Calculate the progress
                float progress = 1 - ((endTime - Time.time) / duration);

                // Set the fill amount based on the progress
                image.fillAmount = Mathf.Lerp(1f, 0f, progress);

                // Wait for the next frame
                yield return null;
            }

            // Ensure the fill amount is exactly 0 at the end
            image.fillAmount = 0f;
            //yield return new WaitForSeconds(3f);
            ++loadOfMissile;
           
        }
        void ShooterRotate(Vector2 rotationCount)
        {
            gunPos = rotationCount;
            if(!nozzle.isPlaying)
            {
                if (PlayerPrefs.GetInt("Music") == 0)
                    nozzle.Play();
            }

            //222 and 302
            if (rotationCount == Vector2.zero && loadOfMissile>0)
            {
                if (startFire)
                {
                    --loadOfMissile;
                    if (loadOfMissile == 0)
                    {
                        StartCoroutine(LoadMissile("bullet 1"));
                        GameObject.Find("bullet 1").transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
                    }
                    else if (loadOfMissile == 2)
                    {
                        GameObject.Find("bullet 3").transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
                        StartCoroutine(LoadMissile("bullet 3"));
                    }
                    else if (loadOfMissile == 1)
                    {
                        GameObject.Find("bullet 2").transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
                        StartCoroutine(LoadMissile("bullet 2"));
                    }
                    //multiplayPlayer.SendMessage("SetBulletPosition", bulletPosition);
                    print("Fire from ShooterRotate if" + startFire);
                   // Fire(rotationCount);
                    startFire = false;
                    
                    multiplayPlayer.SendMessage("LoadMissile");
                    // multiplayPlayer.SendMessage("LoadPlayerClientRpc");
                    if (multiplayPlayer.isGravity)
                    {
                        if (transform.localEulerAngles.z > 222f & transform.localEulerAngles.z < 303)
                        {
                            multiplayPlayer.SendMessage("BackFire");
                        }
                    } else
                    {
                        multiplayPlayer.SendMessage("BackFire");
                    }
                    
                    //ClientShhotClientRpc();
                    //multiplayPlayer.SendMessage("OnFireServerRpc", false);
                    //ClientShhotClientRpc();
                }

            }
            else
            {
                print("Fire from ShooterRotatecelse " + startFire);
                gunAngle = Mathf.Atan2(rotationCount.y, rotationCount.x) * 180 / Mathf.PI;

                // Apply rotation to the object
                instance.transform.rotation = Quaternion.Euler(0, 0, (gunAngle + 180f));
                startFire = true;
                
                //multiplayPlayer.SendMessage("OnFireServerRpc", startFire);
            }

        }
       
        void Fire(Vector2 rotationCount)
        {
            nozzle.Stop();
            if (!IsClient) return;
            if (PlayerPrefs.GetInt("Music") == 0)
                source.Play();
            GameObject ob = Instantiate(bullet);
            ob.SetActive(true);
          // ob.GetComponent<NetworkObject>().NetworkObjectId
            ob.transform.localPosition = bulletPosition.position;
            //ob.transform.rotation = GameObject.Find("bulletPosition").transform.rotation;
            Quaternion originalRotation = bulletPosition.transform.rotation;

            // Add 90 degrees to the z-axis rotation
            Quaternion rotatedQuaternion = originalRotation * Quaternion.Euler(0f, 0f, 90f);

            // Assign the new rotated rotation to the object's transform
            ob.transform.rotation = rotatedQuaternion;

            Rigidbody2D rb = ob.GetComponent<Rigidbody2D>();
            //FireBullerServerRpc();
            rb.velocity = ob.transform.up * 30;

            if (rb != null)
            {

                //Vector2 gravityDirec = GameObject.Find("Player").GetComponent<MultiplayPlayer>().gravityDirection;
                // Apply force to the bullet in the direction of its forward vector
            
                --bulletCount;
                //GameManager.instance.SendMessage("bulletManage", bulletList.Count);
                // LoadMisile();
                //}
                //else
                //{
                //    Debug.LogError("Rigidbody2D component not found on bullet prefab!");
                //}
            }
        }

    }
}
