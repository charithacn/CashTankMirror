using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviour
{
    public GameObject playerObject;
    Rigidbody2D rg;
    public bool grounded;
    public LayerMask mask;
    public GameObject missile;
    public Explosions explosionManager;
    float shortestRay;
    public float PointingTowards;
    float goAway;
    public Lava lava;
    public BlowUpTiles tiles;
    public GameObject enemy;
    public SpriteRenderer thisRenderer;
    public CircleCollider2D thisCollider;

    public bool ISPLAYING;

    public float speed = 2;
    // public float speedCap = 24;

    // UI
    public GameObject menu;
    public Slider healthbar;
    public GameObject healthobject;
    public PlayerHandler playerHandler;
    public SlideIn winScreen;
    public SlideIn loseScreen;
    public ManageRefills refills;

    // BACK TO ARENA

    public GameObject backToArena;
    public TMP_Text backText;
    public TMP_Text timeLeftText;
    public float timeLeft;

    // STATS
    public float maxhealth;
    public GameObject LastTargetedBy;
    public float LastTargeted;

    // HEALTH
    public float Health;
    public string DeathCause;

    // Tank Parts
    // bool left;
    GameObject Front;
    GameObject Back;
    public GameObject Gun;
    public Vector2 aim;
    GameObject Socket;
    GameObject Wheel1;
    GameObject Wheel2;
    GameObject Window;

    // SKINS //
    public List<Sprite> fronts = new List<Sprite>();
    public List<Sprite> backs = new List<Sprite>();
    public List<Sprite> guns = new List<Sprite>();
    public List<Sprite> sockets = new List<Sprite>();
    public List<Sprite> wheels = new List<Sprite>();
    public List<Sprite> windows = new List<Sprite>();

    float timer;

    // Multiplayer
    public PlayerNetwork onlinePlayer;
    public bool currentlyShooting;
    bool trytolaunch;

    void Start()
    {
        playerObject.transform.position = new Vector2(0, 0);
        rg = playerObject.GetComponent<Rigidbody2D>();
        rg.Sleep();
        explosionManager = GameObject.Find("ExplosionManager").GetComponent<Explosions>();
        menu.SetActive(true);
        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(false);
        thisRenderer.enabled = true;
        thisCollider.enabled = true;
        playerHandler = GameObject.Find("PlayerHandler").GetComponent<PlayerHandler>();
        Health = 1;
        ISPLAYING = false;

        // Find all parts
        Front = transform.Find("Front").gameObject;
        Back = transform.Find("Back").gameObject;
        Gun = transform.Find("Gun").gameObject;
        Socket = transform.Find("Socket").gameObject;
        Wheel1 = transform.Find("Wheel1").gameObject;
        Wheel2 = transform.Find("Wheel2").gameObject;
        Window = transform.Find("Window").gameObject;
    }

    void Update()
    {
        if (ISPLAYING)
        {
            if (Health <= 0)
            {
                Health = 0;
                if (playerHandler.GAMESTATE == "GAMEPLAY")
                {
                    //menu.GetComponent<SlideIn>().FromTop(1, true);
                    playerHandler.CalculatePlayerEarnings();
                    loseScreen.FromTop(1, 3);
                    ISPLAYING = false;
                }

            }
            else if (playerHandler.GAMESTATE != "MENU")
            {
                // === PLAYER MOVEMENT === //

                // Moving player in that direction
                if (grounded)
                {
                    // rg.AddForce(new Vector2(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime), ForceMode2D.Impulse);
                    float deltaTime = Time.deltaTime;
                    if (deltaTime > 0.05f)
                        deltaTime = 0.05f;

                    transform.position += new Vector3(playerHandler.Movement.x, playerHandler.Movement.y, 0).normalized
                    * speed * deltaTime * (Mathf.Max(Mathf.Abs(playerHandler.Movement.x), Mathf.Abs(playerHandler.Movement.y)));

                    if (playerHandler.Movement.x != 0 || playerHandler.Movement.y != 0)
                    {
                        Wheel1.transform.Rotate(new Vector3(0, 0, Quaternion.Angle(Quaternion.LookRotation(-transform.right), Quaternion.LookRotation(new Vector3(playerHandler.Movement.x, playerHandler.Movement.y).normalized))) / 3);
                        Wheel2.transform.Rotate(new Vector3(0, 0, Quaternion.Angle(Quaternion.LookRotation(-transform.right), Quaternion.LookRotation(new Vector3(playerHandler.Movement.x, playerHandler.Movement.y).normalized))) / 3);
                    }

                    rg.gravityScale = 0;
                    if (goAway == 0)
                        rg.velocity = new Vector2(rg.velocity.x / 1.5f, rg.velocity.y / 1.5f);
                    rg.angularVelocity = rg.angularVelocity / 2;

                    // Thanks so much to Ketra Games on YouTube for this code :)

                    /*if (Input.GetAxis("Horizontal") < 0)
                        left = true;
                    else if (Input.GetAxis("Horizontal") > 0)
                        left = false;*/

                    transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Vector3 relativePos = new Vector3(-Mathf.Sin(PointingTowards), -Mathf.Cos(PointingTowards));
                    Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, relativePos);
                    if (Quaternion.Angle(toRotation, transform.rotation) < 90)
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 350);
                    else
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 150);

                    //rg.isKinematic = true;
                }
                else
                {
                    rg.gravityScale = 0.9f;
                    //rg.isKinematic = false;
                }

                if ((Input.GetKeyDown(KeyCode.Space) || trytolaunch) && (Health > 0) && refills.AMMO >= 1)
                {
                    currentlyShooting = true;
                    Launch();
                }

                if (trytolaunch)
                    trytolaunch = false;

                // important stuff //

                // ===== BLAST PROCESSING AND LOTS OF IMPORTANT STUFF, ONE OF THE MOST IMPORTANT PARTS OF THE GAME ===== //

                // important stuff //

                if (explosionManager.BlastFling(transform.position) != new Vector2())
                {
                    LastTargetedBy = explosionManager.GetTarget(transform.position, gameObject);
                    LastTargeted = 0;

                    if (grounded)
                        rg.AddForce(explosionManager.BlastFling(transform.position), ForceMode2D.Impulse);
                    else
                        rg.AddForce(explosionManager.BlastFling(transform.position) * 0.75f, ForceMode2D.Impulse);
                    grounded = false;
                    goAway = 0.1f;

                    Health -= explosionManager.BlastDamage(transform.position, gameObject) / 2;

                    if (Health < 0)
                    {
                        if (Vector2.Distance(LastTargetedBy.transform.position, transform.position) > 15 && LastTargetedBy.transform.position.x != 9999)
                        {
                            DeathCause = "Sniped by " + LastTargetedBy.name;
                            playerHandler.AddPlayerDeath(new KillData(gameObject, "Sniped"), LastTargetedBy);
                        }
                        else if (LastTargetedBy.GetComponent<Enemy>().grounded)
                        {
                            if (grounded)
                            {
                                DeathCause = "Killed by " + LastTargetedBy.name;
                                playerHandler.AddPlayerDeath(new KillData(gameObject, "Killed"), LastTargetedBy);
                            }
                            else
                            {
                                DeathCause = "Duckhunted by " + LastTargetedBy.name;
                                playerHandler.AddPlayerDeath(new KillData(gameObject, "Duckhunted"), LastTargetedBy);
                            }
                        }
                        else
                        {
                            if (grounded)
                            {
                                DeathCause = "Midaired by " + LastTargetedBy.name;
                                playerHandler.AddPlayerDeath(new KillData(gameObject, "Midaired"), LastTargetedBy);
                            }
                            else
                            {
                                DeathCause = "Aerially bombed by " + LastTargetedBy.name;
                                playerHandler.AddPlayerDeath(new KillData(gameObject, "Aerialed"), LastTargetedBy);
                            }
                        }
                    }
                }

                // == RAYCASTING == //

                if (goAway == 0)
                {
                    float shortestDistance = 0.35f;
                    shortestRay = -1;

                    RaycastHit2D temp = Physics2D.CircleCast(transform.position + (Vector3.up * 0.0001f), 0.4f, Vector2.down * 0.0001f, 0.0001f, mask);
                    if (temp == false)
                    {
                        for (float i = 0; i <= 360; i += 0.3f)
                        {
                            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.6f, new Vector2(Mathf.Sin(i), Mathf.Cos(i)), 0.35f, mask);
                            if (hit == true && hit.collider.tag == "Ground" && hit.distance < shortestDistance)
                            {
                                shortestRay = i;
                                shortestDistance = hit.distance;
                            }
                        }
                    }
                    else
                    {
                        for (float i = 0; i <= 360; i += 0.3f)
                        {
                            RaycastHit2D hit = Physics2D.CircleCast(transform.position + new Vector3(Mathf.Sin(i) * -0.35f, Mathf.Cos(i) * -0.35f),
                                0.6f, new Vector2(Mathf.Sin(i), Mathf.Cos(i)), 0.35f, mask);
                            if (hit == true && hit.collider.tag == "Ground" && hit.distance < shortestDistance)
                            {
                                shortestRay = i;
                                shortestDistance = hit.distance;
                            }
                        }
                    }

                    if (shortestRay > -1)
                    {
                        transform.position = transform.position + (new Vector3(Mathf.Sin(shortestRay) * shortestDistance, Mathf.Cos(shortestRay) * shortestDistance));
                        if (Health <= 0)
                            grounded = false;
                        else
                            grounded = true;
                    }
                }

                goAway -= Time.deltaTime;
                if (goAway < 0)
                    goAway = 0;

                LastTargeted += Time.deltaTime;
                if (LastTargeted > 10)
                    LastTargetedBy = null;


                float sd = 0.35f;
                for (float i = 0; i < 360; i+=2)
                {
                    RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f, new Vector2(Mathf.Sin(i), Mathf.Cos(i)), 0.35f, mask);
                    if (hit == true && hit.collider.tag == "Ground" && hit.distance < sd)
                    {
                        PointingTowards = i;
                        sd = hit.distance;
                    }
                }

                // Win?

                if (playerHandler.PLAYERSLEFT == 0 && playerHandler.GAMESTATE == "GAMEPLAY")
                {
                    winScreen.FromTop(1, 3);
                    ISPLAYING = false;
                }

                Vector3 diff = /*Camera.main.ScreenToWorldPoint(Input.mousePosition) - Gun.transform.position*/aim;
                diff.Normalize();

                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                Gun.transform.rotation = Quaternion.Euler(0, 0, rot_z - 180);
            }
        }
        else
        {
            rg.gravityScale = 0.9f;
        }

        // Update health bar

        healthobject.transform.position = Camera.main.WorldToScreenPoint(transform.position + (1 * Vector3.up));
        healthbar.value = Health / maxhealth;

        // BACK TO ARENA

        if (transform.position.x < -50 || transform.position.y > 50 || transform.position.x > 50)
        {
            backToArena.SetActive(true);
            if (timeLeft == 0)
            {
                if (Health > 0)
                    timeLeft = 5;
            }
            else
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    Health = 0;
                    DeathCause = "Bounded";
                    playerHandler.AddPlayerDeath(new KillData(gameObject, "Bounds"), null);
                    timeLeft = 0;
                }
                timeLeftText.text = (Mathf.Round(timeLeft / 0.01f) * 0.01f).ToString();
                
                backText.color = new Color((Mathf.Sin(timer) + 9) / 9 * 229.4f / 256, (Mathf.Sin(timer) + 9) / 9 * 35 / 256, (Mathf.Sin(timer) + 9) / 9 * 35 / 256);
                timeLeftText.color = backText.color;
            }
        }
        else
        {
            backToArena.SetActive(false);
            timeLeft = 0;
        }

        timer += Time.deltaTime;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<BoxCollider2D>().tag == "Lava" && (Health > 0))
        {
            Health -= 45 * Time.deltaTime;
            if (Health < 0)
                DeathCause = "Lava";
        }
    }

    public void Launch()
    {
        Vector2 mousePosition = /*Camera.main.ScreenToWorldPoint(Input.mousePosition)*/aim;
        Vector2 direction = aim.normalized;
        GameObject newMissile = Instantiate(missile, transform.position + ((Vector3)direction * 0.5f), missile.transform.rotation);
        newMissile.GetComponent<Missile>().Parent = gameObject;

        // Credit to Octet on discussions.unity.com for some code :)

        newMissile.transform.up = direction;
        newMissile.GetComponent<Missile>().Impact((Vector3)direction * 200);

        if (!grounded)
        {
            rg.velocity = new Vector2(rg.velocity.x / 8, rg.velocity.y / 8);
            rg.AddForce(((Vector2)transform.position - mousePosition).normalized * 10, ForceMode2D.Impulse);
            rg.AddTorque(transform.position.x - mousePosition.x);
        }

        refills.AMMO--;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Missile")
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
    }

    public void StartPlayer()
    {
        menu.SetActive(false);
        rg.WakeUp();
        lava.moving = true;

        playerHandler.SetGameState("GAMEPLAY");

        ISPLAYING = true;
    }

    public void EndPlayer()
    {
        ISPLAYING = false;
        Health = 0;
        rg.velocity = new Vector2(0, 0);
        rg.Sleep();
        playerObject.transform.position = new Vector2(0, 0);
        lava.moving = false;
        foreach (GameObject g in playerHandler.Enemies)
        {
            Destroy(g.GetComponent<Enemy>().healthobject);
            //playerHandler.Enemies.Remove(g);
            Destroy(g);
        }

        playerHandler.SetGameState("MENU");
        playerHandler.AddRewards();
        playerHandler.ResetRewards();
        playerHandler.mapOnline = false;
    }

    public void EarlyStart(int level)
    {
        // GameObject.Find("Tilemap").GetComponent<BlowUpTiles>().LoadLevel(level);

        lava.ResetLava();

        refills.ResetObjectCount();

        rg.velocity = new Vector2(0, 0);
        playerObject.transform.position = new Vector2(0, 0);
        playerHandler.EarlyStart();

        Health = maxhealth;
        DeathCause = "";

        playerHandler.AddPlayerKillDataUser(gameObject);
        playerHandler.AddToPlayers(gameObject);

        ISPLAYING = true;

        // Skin
        Front.GetComponent<SpriteRenderer>().sprite = fronts[playerHandler.skinNumber];
        Back.GetComponent<SpriteRenderer>().sprite = backs[playerHandler.skinNumber];
        Gun.GetComponent<SpriteRenderer>().sprite = guns[playerHandler.skinNumber];
        Socket.GetComponent<SpriteRenderer>().sprite = sockets[playerHandler.skinNumber];
        Sprite wheelSprite = wheels[playerHandler.skinNumber];
        Wheel1.GetComponent<SpriteRenderer>().sprite = wheelSprite;
        Wheel2.GetComponent<SpriteRenderer>().sprite = wheelSprite;
        Window.GetComponent<SpriteRenderer>().sprite = windows[playerHandler.skinNumber];
    }

    public void LaunchInput()
    {
        trytolaunch = true;
    }
}