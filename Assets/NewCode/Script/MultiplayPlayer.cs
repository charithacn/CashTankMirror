using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace BoomTanks.Multiplayer
{
    public class MultiplayPlayer : NetworkBehaviour
    {
        public readonly SyncVar<int> coins = new SyncVar<int>();
        public readonly SyncVar<int> lifeVal = new SyncVar<int>();
        public readonly SyncVar<int> playerId = new SyncVar<int>();
        public readonly SyncVar<bool> winner = new SyncVar<bool>();
        public readonly SyncVar<int> place = new SyncVar<int>();

        public readonly SyncVar<int> gameStatus = new SyncVar<int>();

        private int killMarks = 0;
        public float moveSpeed = 5f;
        public float jumpForce = 100f;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;

        private Rigidbody2D rb;
        public bool isGrounded;

        private Vector2 lastGroundedPosition;

        public bool isGravity = false;
        public Shoot shoot;
        public Vector2 gravityDirection;
        public GameObject down;
        [SerializeField]
        private InputActionReference playerMovementAction;
        [SerializeField]
        private InputActionReference playerShootPreference;

        public bool isFire;
        Vector2 leftTrans;

        public readonly SyncVar<bool> fireNetwork = new  SyncVar<bool>();

        public readonly SyncVar<Vector2> missileStartPosition = new SyncVar<Vector2>();
        public readonly SyncVar<Quaternion> missileRotation = new SyncVar<Quaternion>();




        public Transform clientPositionMainObject;

        public Transform bulletPosition;

        [Header("Missile attributes")]
        public GameObject missile;
        public Transform backshootPosition;

        public int life = 5;


        public float groundCheckHeight = 0.2f;
        public float rotationSpeed = .1f;
        Vector2 forceDirection;

        public Vector2 livePosition;
        public Quaternion liveRotation;

        Vector2 floorDirection;
        public Transform backFireForce;

        public Slider lifeSlider;
        Collider2D thiscollision;

        bool ismoveStart = false;

        public List<string> nameList;
        public AudioSource source;
        public int clientId;
       
        static int localClientId = 0;
        public int instanceClientId = 0;
        public string username;
        private static bool win;

        public Queue<int> bulletQueue;

        string ownTag;

        [Header("assets")]
        [SerializeField] private SpriteRenderer bodyFront;
        [SerializeField] private SpriteRenderer bodyBack;
        [SerializeField] private SpriteRenderer tyreFront;
        [SerializeField] private SpriteRenderer tyreBack;
        [SerializeField] private SpriteRenderer gun;

        GameObject targetAccepted;
        void Start()
        {
            isFire = true;
            rb = GetComponent<Rigidbody2D>();
            GameObject ob = Instantiate(Resources.Load("sliderUI") as GameObject);
            lifeSlider = ob.transform.GetChild(0).GetComponent<Slider>();
            nameList = new List<string>();
            // username = PlayerPrefs.GetString("UserName");
            win = false;
            bulletQueue = new Queue<int>();

            for (int i = 0; i < 3; i++)
            {
                bulletQueue.Enqueue(1);
            }
            print("Player winner start");
        }

        [ServerRpc]
        public void TestServerRPC(NetworkConnection conn = null)
        {
            clientId = conn.ClientId;
            print("Server count the clients count " + clientId);
            ServerManage.instance.GetComponent<ServerManage>().SendMessage("SetPlayerCount", instanceClientId.ToString());

            ServerManage serverManage = ServerManage.instance.GetComponent<ServerManage>();
            //serverManage.SendMessage("SetClientCountServerRpc");
            clientPositionMainObject = GameObject.Find("positions").transform;
            print("Server client position " + clientId +"/ "+ clientPositionMainObject.transform.GetChild((int)clientId).transform.position);
            transform.position = clientPositionMainObject.transform.GetChild((int)instanceClientId).transform.position;

            lastGroundedPosition = clientPositionMainObject.transform.GetChild((int)instanceClientId).transform.position;
            isGravity = false;
            ServerManage.instance.SendMessage("AddClient", instanceClientId);
            //if(IsClient)
            print("Add TestServerRPC");
            LoadStartPositionClient((int)instanceClientId);
        }
        
        void LoadStartPositionClient(int clientId)
        {
            //var clientId = serverRpcParams.Receive.SenderClientId;
            clientPositionMainObject = GameObject.Find("positions").transform;
           
            isGravity = false;
            ServerManage.instance.GetComponent<ServerManage>().SendMessage("SetPlayerCount", instanceClientId.ToString());

            transform.position = clientPositionMainObject.transform.GetChild((int)instanceClientId).transform.position;

            if(IsOwner) { 
                GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().Follow = transform;
                GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().LookAt = transform;
            }
            // isFire = false;
        }

        public override void OnStartClient()
        {
            if(!PlayerPrefs.HasKey("Coins"))
            {
                PlayerPrefs.SetInt("Coins" + PlayerPrefs.GetString("FirebaseUser"), 1000);
                PlayerPrefs.Save();
            }
            //clientId = (int)ServerRpcParams.re
            instanceClientId = localClientId;
            ServerManage.instance.SendMessage("AddClient", instanceClientId);
            ++localClientId;
           if(IsServer)
            {
                coins.Value -= 1;
                PlayerPrefs.SetInt("Coins"+ PlayerPrefs.GetString("FirebaseUser"), coins.Value);
                PlayerPrefs.Save();

            }
            LoadStartPositionClient(OwnerId);
            TestServerRPC();
            isGravity = false;
            // fireNetwork.OnValueChanged += OnStateChanged;
            // winner.OnValueChanged += WinnerOnStageChange;
            OnStateChanged();
            WinnerOnStageChange();
            fireNetwork.Value = true;
            winner.Value = true;

            ServerManage.instance.AddPlayer(GetComponent<MultiplayPlayer>());
            ServerManage.instance.SendMessage("ClientCount");

            username = "Player " + instanceClientId;
            if(PlayerPrefs.GetString("GameStatus").Equals("vs"))
            {
                if (instanceClientId < 1)
                {
                    this.gameObject.tag = "TeamA";
                    ownTag = "TeamA";
                    
                } else
                {
                    this.gameObject.tag = "TeamB";
                    ownTag = "TeamB";
                    
                }
                if (IsOwner)
                {
                    AssetsObject obs = Resources.Load<AssetsObject>("Object");
                    bodyFront.sprite = obs.VSbody[0];
                    bodyBack.sprite = obs.VSbodyBack[0];
                    tyreBack.sprite = obs.VSrealTyre[0];
                    tyreFront.sprite = obs.VSfrontTyre[0];
                    gun.sprite = obs.VSgun[0];
                }
                else if(!IsOwner)
                {
                    AssetsObject obs = Resources.Load<AssetsObject>("Object");
                    int value = Random.Range(0, obs.body.Length - 1);
                    bodyFront.sprite = obs.VSbody[1];
                    bodyBack.sprite = obs.VSbodyBack[1];
                    tyreBack.sprite = obs.VSrealTyre[1];
                    tyreFront.sprite = obs.VSfrontTyre[1];
                    gun.sprite = obs.VSgun[1];
                }
            } else
            {
                if (IsOwner)
                {
                    AssetsObject obs = Resources.Load<AssetsObject>("Object");
                    bodyFront.sprite = obs.SelectionBody;
                    bodyBack.sprite = obs.selectionBodyBack;
                    tyreBack.sprite = obs.selectionRealTyre;
                    tyreFront.sprite = obs.selectionFrontTyre;
                    gun.sprite = obs.selectionGun;
                }
                else if (!IsOwner)
                {
                    AssetsObject obs = Resources.Load<AssetsObject>("Object");
                    int value = Random.Range(0, obs.body.Length - 1);
                    bodyFront.sprite = obs.body[value];
                    bodyBack.sprite = obs.bodyBack[value];
                    tyreBack.sprite = obs.realTyre[value];
                    tyreFront.sprite = obs.frontTyre[value];
                    gun.sprite = obs.gun[value];
                }
            }
            targetAccepted = GameObject.Find("TargetAccept");
            targetAccepted.SetActive(false);
        }

        public override void OnStopClient()
        {
            OnStateChanged();
            fireNetwork.Value = true;
            print("object removed!");
           // ServerManage.instance.SendMessage("SetPlace", place.Value);
            //ServerManage.instance.SendMessage("PlaceSet");
            if (IsServer)
            {
                place.Value = instanceClientId;
            }
            
            if (IsOwner)
            {
                ServerManage.instance.SendMessage("Lost");
            }
            

        }
        public void WinnerOnStageChange()
        {
            if(winner.Value)
            {
                //Winner(Winner(clientId);
            }
        }
        public void OnStateChanged()
        {
            // note: `State.Value` will be equal to `current` here
            if (fireNetwork.Value)
            {
                GameObject ob = Instantiate(missile);
                ob.SetActive(true);
                ob.transform.localPosition = bulletPosition.position;
                Quaternion originalRotation = bulletPosition.rotation;
                Quaternion rotatedQuaternion = originalRotation * Quaternion.Euler(0f, 0f, 90f);
                ob.transform.rotation = rotatedQuaternion;
                ob.GetComponent<BulletControl>().multiplayPlayer = GetComponent<MultiplayPlayer>();
                Rigidbody2D rb = ob.GetComponent<Rigidbody2D>();
                rb.velocity = ob.transform.up * 22;
                
                if (rb != null)
                {
                    if (shoot.transform.localEulerAngles.z > 222f && shoot.transform.localEulerAngles.z < 323)
                    {
                        isFire = true;
                        print("Send up " + transform.eulerAngles.z + " Eular " + transform.localEulerAngles.z);
                    }

                }
            }
            else
            {
                print("not shoot");
            }
        }

        private void Update()
        {
           //if (!IsClient) return;

            Vector2 rightStickValue = playerShootPreference.action.ReadValue<Vector2>();
            Vector2 leftStickValue = playerMovementAction.action.ReadValue<Vector2>();
            leftTrans = rightStickValue;
            Move(leftStickValue);
            ShootRotation(rightStickValue);
            Vector2 pos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
            lifeSlider.transform.position = new Vector2(pos.x, pos.y + 150f);
            // print("call to ShootRotation" + rightStickValue);

        }
        private void FixedUpdate()
        {
            print("FixedUpdate!" + winner.Value);
            /*if (ServerManage.instance.numberOfClients.Value == 1 && winner.Value)
            {
                print("Winner!" +winner.Value);
                if(IsOwner)
                    ServerManage.instance.SendMessage("WinSetup");
                if(IsServer)
                {
                    winner.Value = false;
                }
            }*/
            if(ServerManage.instance.numberOfClients.Value==1 && ServerManage.instance.gameStartBool.Value && !win)
            {
                print("You are winner");
                if (IsOwner)
                {
                    ServerManage.instance.SendMessage("WinSetup");
                    print("owner winner");
                    win = true;
                }
                if(IsServer)
                {
                    print("End the game");
                    win = true;
                    StartCoroutine(ReatartScene());
                    ServerManage.instance.SendMessage("RestartServer");
                    coins.Value += 8;
                    PlayerPrefs.SetInt("Coins" + PlayerPrefs.GetString("FirebaseUser"), coins.Value);
                    PlayerPrefs.Save();
                    //ServerManage.instance.gameStartBool.Value = false;
                    //NetworkManager.DisconnectClient((ulong)clientId);
                }
                //NetworkManager.DisconnectClient((ulong)clientId);
            } 
        }
        IEnumerator ReatartScene()
        {
            yield return new WaitForSeconds(3f);
            //NetworkManager.DisconnectClient((ulong)clientId);

            //GameObject.Find("SceneManager").GetComponent<LoadScene>().LoadSceneAsync("Server");
        }
        void SetBulletPosition(Transform pos)
        {
            //bulletPosition = pos;
        }
        void Move(Vector2 movement)
        {
           if (!IsClient) return;

           if(!isFire)
            {
                isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
                thiscollision = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
                
                if (!isGrounded)
                {
                    if (livePosition != null)
                    {      
                        if(!isFire)
                            rb.AddForce(transform.up * 450f);
                        transform.position = livePosition;
                        //transform.rotation = liveRotation;

                    }
                    isGrounded = false;
                    rb.gravityScale = 1f;
                }

                else if(isGrounded)
                {
                    if (!isFire)
                    {
                        livePosition = transform.position;
                        liveRotation = transform.rotation;
                        if (ismoveStart)
                        {
                            //Vector3 newPosition = rb.position + movement * moveSpeed * Time.deltaTime;

                            //Vector2 movements = new Vector2(movement.x, movement.y) * 30f;
                            rb.velocity = movement.normalized * 10f;
                            
                                
                           // rb.AddForce(movements);
                           if(movement!=Vector2.zero)
                            {
                                if(!source.isPlaying)
                                {
                                    if(PlayerPrefs.GetInt("Music")==0)
                                        source.Play();
                                }
                            } else
                            {
                                source.Stop();
                            }
                            // Rotate the vehicle
                            // rb.angularVelocity = -rotateInput * rotationSpeed;
                            //rb.MovePosition(newPosition);
                            //transform.Translate(movement.normalized * 10f);
                        }
                            
                        rb.gravityScale = 0f;
                        if (floorDirection != null)
                        {
                           //rb.AddForce(transform.up * 150f);
                            //rb.AddForce(floorDirection.normalized * 250f);
                        }

                        if(thiscollision!=null) { 
                            Vector2 direction = thiscollision.ClosestPoint(transform.position) - (Vector2)transform.position;
                            float angle = Vector2.Angle(direction, transform.right);
                            floorDirection = direction;

                            if(angle>85 && angle<96 )
                            {
                                ismoveStart = true;
                            } else
                            {
                                ismoveStart = false;
                            }
                            if (angle != 90f)
                            {
                                // Calculate the signed angle to determine the direction of rotation
                                Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);

                                // Smoothly rotate towards the target rotation
                                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                                Vector2 collisionNormal = thiscollision.ClosestPoint(transform.position) - (Vector2)transform.position; 
                                Vector2 forceDirection = Quaternion.Euler(0, 0, 180) * collisionNormal;

                                // Rigidbody2D rb = GetComponent<Rigidbody2D>();
                                if (rb != null)
                                {
                                    //rb.AddForce(forceDirection.normalized * 1f, ForceMode2D.Impulse);
                                    //rb.AddForce(forceDirection * 10f, ForceMode2D.Impulse);
                                }
                            }
                        }
                    }
                        
                } 
            } else
            {
                isGrounded = true;
                rb.gravityScale = 1f;
                if(movement==Vector2.zero)
                {
                    //rb.velocity = -transform.up * 100;
                    //isFire = false;
                }
            }
            lifeSlider.value = lifeVal.Value;

        }
        void ShootRotation(Vector2 value)
        {

            shoot.SendMessage("ShooterRotate", value);
        }

        void BackFire()
        {
            isFire = true;
            print("BackFireok");
            isGravity = false;

            
            rb.gravityScale = 1f;
            
            StartCoroutine(WaitForce());
        }

        IEnumerator WaitForce()
        {
            yield return new WaitForSeconds(0.2f);
            //rb.velocity = -transform.up* 50f;
            Vector3 forceDirection = (backshootPosition.transform.position - transform.position).normalized;
            //rb.AddForce(forceDirection * 150f, ForceMode2D.Force);
            rb.velocity = forceDirection * 20f;
        }
        IEnumerator WaitForForceUp()
        {
            isFire = true;
            print("Force up ok");
            isGravity = false;
            rb.gravityScale = 1f;
            yield return new WaitForSeconds(0.1f);
            //rb.velocity = -transform.up* 50f;
            Vector3 forceDirection = -transform.up;
            //rb.AddForce(forceDirection * 150f, ForceMode2D.Force);
            rb.velocity = forceDirection * 2.5f;
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Ground"))
            {
                isGravity = true;
                isFire = false;
            }          
           
           // rb.gravityScale = 0;
            //print("Detected collision name " + collision.gameObject.tag);  
        }
        void OnCollisionStay2D(Collision2D collision)
        {

            //thiscollision = collision;
            
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
           
        }
        [ServerRpc]
        void OnFireServerRpc(bool value)
        {
            fireNetwork.Value = value;
        }
        [ServerRpc]
        void ChangeRightPosiyionServerRpc(Vector2 pos)
        {
            missileStartPosition.Value = pos;
            print("--------Bullet Position " + pos);
        }
        [ServerRpc]
        void MissileRotationServerRpc(Quaternion qt)
        {
            missileRotation.Value = qt;
            print("-----Bullet rotation " + qt);
        }
        [ServerRpc]
        void LoadMissileServerRpc()
        {
            if(IsServer)
                fireNetwork.Value = true;
            print("server rpc ");
            ServerManage.instance.SendMessage("SpawnManage");
            StartCoroutine(WaitForStopPrefab());
        }
        void LoadMissile()
        {
            LoadMissileServerRpc();
        }
        IEnumerator WaitForStopPrefab()
        {
            yield return new WaitForSeconds(0.5f);
            if(IsServer)
                fireNetwork.Value = false;
        }
        [ObserversRpc]
        void LoadPlayerClientRpc()
        {
            print("Load client rpc");
        }

        [ServerRpc]
        public void ClienTmarksServerRpc()
        {
            //coins.Value = coins.Value + 3;
        }
        [ServerRpc]
        public void ClientLifeServerRpc()
        {
            if(IsServer)
            lifeVal.Value = lifeVal.Value - 1;
        }
        [ServerRpc]
        public void FireToClientServerRpc()
        {//
           // if(IsServer)
            //coins.Value = coins.Value + 3;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(PlayerPrefs.GetString("GameStatus").Equals("solo"))
            {
                if(collision.tag.Equals("Missile") || collision.tag.Equals("Lava")) {
                    livePosition = Vector2.zero;
                    print("Fire targeted to the player");
                    if(life>0)
                    {
                        --life;
                        if(IsServer)
                        {
                            --lifeVal.Value;
                        }
                       // ClientLifeServerRpc();
                        ++killMarks;
                    }
                
                    if(life==0)
                    {
                    
                        if (IsServer)
                        {
                            if (ServerManage.instance.numberOfClients.Value == 2)
                            {
                                print("User runner up");
                                coins.Value += 5;
                                PlayerPrefs.SetInt("Coins" + PlayerPrefs.GetString("FirebaseUser"), coins.Value);
                                PlayerPrefs.Save();
                            }
                            targetAccepted.SetActive(false);
                            targetAccepted.SetActive(true);
                            //NetworkManager
                            Destroy(this);
                            //nameList.Add("Name");
                        
                           // ServerManage.instance.RemovePlayer(GetComponent<MultiplayPlayer>()); 
                            // winner.Value = true;
                            print("Lost the match");


                        
                            //NetworkManager.Singleton.Shutdown(true);
                           //Destroy(this.gameObject);
                        }
                        if(IsClient)
                        {
                            //if(NetworkManager.IsConnectedClient)
                              // ServerManage.instance.SendMessage("WinSetup");
                            // NetworkManager.DisconnectClient((ulong)clientId);
                            //ServerManage.instance.SendMessage("DestroyClient", instanceClientId);
                            //ServerManage.instance.RemovePlayer(GetComponent<MultiplayPlayer>()); 
                            //if (winner.Value) Winner(clientId);
                        }
                           /// NetworkManager.Singleton.Shutdown();
                        //NetworkManager.Singleton.Shutdown(true);
                        //GameObject.Find("SceneManager").GetComponent<LoadScene>().LoadSceneAsync("Start");
                        //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
                    }
                }
            } else if(PlayerPrefs.GetString("GameStatus").Equals("vs"))
            {
                if (!ownTag.Equals(collision.tag))
                {
                    if (collision.tag.Equals("Missile") || collision.tag.Equals("Lava"))
                    {
                        livePosition = Vector2.zero;
                        print("Fire targeted to the player");
                        if (life > 0)
                        {
                            --life;
                            if (IsServer)
                            {
                                --lifeVal.Value;
                            }
                            // ClientLifeServerRpc();
                            ++killMarks;
                        }

                        if (life == 0)
                        {

                            if (IsServer)
                            {
                                if (ServerManage.instance.numberOfClients.Value == 2)
                                {
                                    print("User runner up");
                                    coins.Value += 5;
                                    PlayerPrefs.SetInt("Coins" + PlayerPrefs.GetString("FirebaseUser"), coins.Value);
                                    PlayerPrefs.Save();
                                }
                                targetAccepted.SetActive(false);
                                targetAccepted.SetActive(true);
                                // NetworkManager.DisconnectClient((ulong)clientId);
                                Destroy(this);
                                //nameList.Add("Name");
                                
                                // ServerManage.instance.RemovePlayer(GetComponent<MultiplayPlayer>()); 
                                // winner.Value = true;
                                print("Lost the match");



                                //NetworkManager.Singleton.Shutdown(true);
                                //Destroy(this.gameObject);
                            }
                            if (IsClient)
                            {
                                //if(NetworkManager.IsConnectedClient)
                                // ServerManage.instance.SendMessage("WinSetup");
                                // NetworkManager.DisconnectClient((ulong)clientId);
                                //ServerManage.instance.SendMessage("DestroyClient", instanceClientId);
                                //ServerManage.instance.RemovePlayer(GetComponent<MultiplayPlayer>()); 
                                //if (winner.Value) Winner(clientId);
                            }
                            /// NetworkManager.Singleton.Shutdown();
                            //NetworkManager.Singleton.Shutdown(true);
                            //GameObject.Find("SceneManager").GetComponent<LoadScene>().LoadSceneAsync("Start");
                            //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
                        }
                    }
                }
            }
            if (collision.tag.Equals("Missile"))
            {
                StartCoroutine(WaitForForceUp());
            }
        }
        void Winner(int winId)
        {
            
            if(IsClient)
            {
                print(clientId + " You are winner ");
                ServerManage.instance.SendMessage("DestroyClient", instanceClientId);
                ServerManage.instance.SendMessage("WinSetup" , instanceClientId);
                ServerManage.instance.AddwinnerListPref(GetComponent<MultiplayPlayer>());
                if (winId==instanceClientId)
                {
                    

                }
                // GameObject.Find("YOU WON").SetActive(true);
               // ServerManage.instance.SendMessage("WinSetup");
            }



        }


    }
   
}

