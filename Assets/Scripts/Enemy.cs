using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject enemyObject;
    Rigidbody2D rg;
    public bool grounded;
    public LayerMask mask;
    public GameObject missile;
    public Explosions explosionManager;
    float shortestRay;
    public float PointingTowards;
    float goAway;
    public Lava lava;
    public GameObject player;

    public float speed = 2;
    public int EnemyNumber;

    float Horizontal;
    float Vertical;
    // public float speedCap = 24;

    // UI
    public Slider healthbar;
    public GameObject healthobject;
    public GameObject healthPrefab;
    public SpriteRenderer thisRenderer;
    public CircleCollider2D thisCollider;
    public PlayerHandler playerHandler;

    // STATS
    public float maxhealth;
    public GameObject LastTargetedBy;
    public float LastTargeted;
    float internalAmmo;
    float internalMax;
    float internalReload;

    // HEALTH
    public float Health;
    public string DeathCause;
    bool hasDiedYet;

    // Tank Parts
    GameObject Front;
    GameObject Back;
    public GameObject Gun;
    GameObject Socket;
    GameObject Wheel1;
    GameObject Wheel2;
    GameObject Window;

    // Multiplayer
    public GameObject onlineController;
    public int ExternallyShooting;
    public Quaternion direction;
    float lastShoot;

    void Awake()
    {
        rg = enemyObject.GetComponent<Rigidbody2D>();
        explosionManager = GameObject.Find("ExplosionManager").GetComponent<Explosions>();
        lava = GameObject.Find("Lava").GetComponent<Lava>();
        player = GameObject.Find("Player");
        healthobject = Instantiate(healthPrefab, GameObject.Find("Canvas").transform);
        healthbar = healthobject.GetComponent<Slider>();
        Health = maxhealth;
        rg.WakeUp();
        thisRenderer.enabled = true;
        thisCollider.enabled = true;
        playerHandler = GameObject.Find("PlayerHandler").GetComponent<PlayerHandler>();
        playerHandler.AddToEnemies(gameObject);
        name = "Enemy " + playerHandler.Enemies.Count;

        // Find all parts
        Front = transform.Find("Front").gameObject;
        Back = transform.Find("Back").gameObject;
        Gun = transform.Find("Gun").gameObject;
        Socket = transform.Find("Socket").gameObject;
        Wheel1 = transform.Find("Wheel1").gameObject;
        Wheel2 = transform.Find("Wheel2").gameObject;
        Window = transform.Find("Window").gameObject;

        // Skin
        Front.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Movement>().fronts[Random.Range(0, 13)];
        Back.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Movement>().backs[Random.Range(0, 13)];
        Gun.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Movement>().guns[Random.Range(0, 13)];
        Socket.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Movement>().sockets[Random.Range(0, 13)];
        Sprite wheelSprite = player.GetComponent<Movement>().wheels[Random.Range(0, 13)];
        Wheel1.GetComponent<SpriteRenderer>().sprite = wheelSprite;
        Wheel2.GetComponent<SpriteRenderer>().sprite = wheelSprite;
        Window.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Movement>().windows[Random.Range(0, 13)];

        // Last Targeted By
        LastTargeted = 10;
        LastTargetedBy = null;
        internalMax = Random.Range(3, 5);
        internalReload = 5/*internalMax / 2 + 0.5f*/;
        internalAmmo = internalMax;
    }

    void Update()
    {
        if (onlineController != null)
        {
            if (ExternallyShooting > 0 && lastShoot > 0.25f)
            {
                ExternallyShooting = 0;
                lastShoot = 0;
                Launch();
            }

            lastShoot += Time.deltaTime;

            rg.Sleep();

            // transform.position = onlineController.transform.position;
            // transform.rotation = onlineController.transform.rotation;

            // Update health bar

            if (healthobject != null && healthbar != null)
            {
                healthobject.transform.position = Camera.main.WorldToScreenPoint(transform.position + (1 * Vector3.up));
                healthbar.value = Health / maxhealth;
            }

            return;
        }
        else if (Health <= 0)
        {
            Health = 0;
            rg.velocity = new Vector2();
            rg.Sleep();

            if (!hasDiedYet)
            {
                playerHandler.PLAYERSLEFT--;
                playerHandler.Present[playerHandler.Enemies.IndexOf(gameObject)] = false;
            }
            
            // We're dead, but deleting gameObjects would cause some weird stuff so the object is not destroyed yet

            if (healthobject != null)
                Destroy(healthobject);

            if (DeathCause != "Lava")
                transform.position = new Vector2(9999, 9999);
            else
                transform.position += Vector3.down * Time.deltaTime * 0.2f;

            hasDiedYet = true;
        }
        else
        {
            // === ENEMY MOVEMENT === //

            rg.WakeUp();
            // Moving player in that direction
            if (grounded)
            {
                // rg.AddForce(new Vector2(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime), ForceMode2D.Impulse);
                float deltaTime = Time.deltaTime;
                if (deltaTime > 0.05f)
                    deltaTime = 0.05f;

                Horizontal += Random.Range(-0.1f, 0.1f);
                Horizontal = Mathf.Clamp(Horizontal, -1, 1);
                Vertical += Random.Range(-0.1f, 0.1f);
                Vertical = Mathf.Clamp(Vertical, -1, 1);

                if (Health > 0)
                    transform.position += new Vector3(Horizontal, Vertical).normalized
                    * speed * deltaTime * Mathf.Max(Mathf.Abs(Horizontal), Mathf.Abs(Vertical));
                rg.gravityScale = 0;
                if (goAway == 0)
                    rg.velocity = new Vector2(rg.velocity.x / 1.5f, rg.velocity.y / 1.5f); rg.angularVelocity = rg.angularVelocity / 2;

                // Thanks so much to Ketra Games on YouTube for this code :)

                Vector3 relativePos = new Vector3(-Mathf.Sin(PointingTowards), -Mathf.Cos(PointingTowards));
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, relativePos);
                if (Quaternion.Angle(toRotation, transform.rotation) > 90)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 350);
                else
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 150);
            }
            else
            {
                rg.gravityScale = 0.9f;
                //rg.isKinematic = false;
            }

            if (Random.Range(0, 99) == 0 && (Health > 0) && internalAmmo > 0)
                Launch();

            // Move due to blasts
            if (explosionManager.BlastFling(transform.position) != new Vector2(0, 0))
            {
                if (explosionManager.GetTarget(transform.position, gameObject) != null)
                {
                    LastTargetedBy = explosionManager.GetTarget(transform.position, gameObject);
                    LastTargeted = 0;
                }

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

                RaycastHit2D temp = Physics2D.CircleCast(transform.position + (Vector3.up * 0.0001f), 0.6f, Vector2.down * 0.0001f, 0.0001f, mask);
                if (temp == false)
                {
                    for (float i = 0; i <= 360; i += 0.333f)
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
                    for (float i = 0; i <= 360; i += 0.333f)
                    {
                        RaycastHit2D hit = Physics2D.CircleCast(transform.position + new Vector3(Mathf.Sin(i) * -0.35f, Mathf.Cos(i) * -0.35f),
                            0.4f, new Vector2(Mathf.Sin(i), Mathf.Cos(i)), 0.35f, mask);
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
            for (float i = 0; i < 360; i+= 4)
            {
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.6f, new Vector2(Mathf.Sin(i), Mathf.Cos(i)), 0.35f, mask);
                if (hit == true && hit.collider.tag == "Ground" && hit.distance < sd)
                {
                    PointingTowards = i;
                    sd = hit.distance;
                }
            }

            // Update health bar

            if (healthobject != null && healthbar != null)
            {
                healthobject.transform.position = Camera.main.WorldToScreenPoint(transform.position + (1 * Vector3.up));
                healthbar.value = Health / maxhealth;
            }

            // REFILL
            internalAmmo += Time.deltaTime / internalReload;
            if (internalAmmo > internalMax)
                internalAmmo = internalMax;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<BoxCollider2D>().tag == "Lava" && (Health > 0))
        {
            Health -= 45 * Time.deltaTime;
            if (Health < 0)
            {
                DeathCause = "Lava";
                if (LastTargetedBy != null)
                {
                    DeathCause = "Sunk by " + LastTargetedBy.name;
                    playerHandler.AddPlayerDeath(new KillData(LastTargetedBy, "Sunk"), gameObject);
                }
            }
        }
    }

    public void Launch()
    {
        if (onlineController != null)
        {
            
        }
        else
        {
            Vector3 target = player.transform.position;
            Vector2 mousePosition = new Vector3();

            if (Random.value < 0.2f)
            {
                target = playerHandler.Players[Random.Range(0, playerHandler.Players.Count - 1)].transform.position;
                mousePosition = target + (Vector2.Distance(transform.position, target) * Vector3.up * Mathf.Tan(Vector2.Distance(transform.position, target) / 15.39286f));
            }

            Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
            Vector3 diff = target - Gun.transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            Gun.transform.rotation = Quaternion.Euler(0, 0, rot_z - 180);
        }

        GameObject newMissile = Instantiate(missile, transform.position + (Gun.transform.right * -0.5f), missile.transform.rotation);
        newMissile.GetComponent<Missile>().Parent = gameObject;

        // Credit to Octet on discussions.unity.com for some code :)

        newMissile.transform.up = -Gun.transform.right;
        newMissile.GetComponent<Missile>().Impact(Gun.transform.right * -225);

        if (!grounded)
        {
            rg.velocity = new Vector2(rg.velocity.x / 8, rg.velocity.y / 8);
            rg.AddForce((transform.position + Gun.transform.right).normalized * 10, ForceMode2D.Impulse);
        }

        if (internalAmmo >= 1)
            internalAmmo--;
    }

    private void OnDestroy()
    {
        if (onlineController != null)
            onlineController.GetComponent<PlayerNetwork>().targetEnemy = false;

        if (healthobject != null)
            Destroy(healthobject);

        if (healthbar != null)
            Destroy(healthbar);

        playerHandler.Enemies.Remove(gameObject);
        playerHandler.Players.Remove(gameObject);
    }
}