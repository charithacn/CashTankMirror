using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.Advertisements;
//using Unity.Services.Leaderboards;


public class PlayerHandler : MonoBehaviour
{ // ADS
    public string myGameIdAndroid = "5515476";
    public string myGameIdIOS = "5515477";
    public string adUnitIdAndroid = "Interstitial_Android";
    public string adUnitIdIOS = "Interstitial_iOS";
    public string myAdUnitId;
    // public bool adStarted;
    bool testMode = true;

    // Normal stuff

    public TMP_Text PlayersLeftText;
    public int PLAYERSLEFT;
    public string GAMESTATE;
    public int Maxplayers;

    public GameObject enemy;

    public List<PlayerKillData> PLAYERKILLDATAS = new List<PlayerKillData>();

    // REWARDS

    public int trophiesEarned;
    public int coinsEarned;

    public GameObject player;
    public int TOTALTROPHIES;
    public int TOTALCOINS;

    public int skinNumber;

    // Texts

    public TMP_Text trophiesWinText;
    public TMP_Text coinsWinText;
    public TMP_Text trophiesLoseText;
    public TMP_Text coinsLoseText;
    public TMP_Text fullcoinstext;
    public TMP_Text fulltrophiestext;

    public List<GameObject> Players = new List<GameObject>();
    public List<GameObject> Enemies = new List<GameObject>();
    public List<bool> Present = new List<bool>();

    public string ChatGPTUsernamesString;
    public List<string> usernames = new List<string>();

    // KILL INDICATOR

    public KillIndicator killIndicator;

    // MAPS
    public List<GameObject> maps = new List<GameObject>();
    public int mapNumber;
    public List<SpawnPoints> spawns = new List<SpawnPoints>();

    float timeSinceLastAd;

    // Multiplayer
    public bool mapOnline;

    public Vector2 Movement;

    async void Awake()
    {
       // await UnityServices.InitializeAsync();
       // await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Start()
    {
        #if UNITY_IOS
	        Advertisement.Initialize(myGameIdIOS, testMode);
	        myAdUnitId = adUnitIdIOS;
        #else
                //Advertisement.Initialize(myGameIdAndroid, testMode);
                myAdUnitId = adUnitIdAndroid;
        #endif

        PlayersLeftText.text = "";
        GAMESTATE = "MAIN MENU";

        TOTALTROPHIES = PlayerPrefs.GetInt("Trophies");
        TOTALCOINS = PlayerPrefs.GetInt("Coins");
        string[] temp = ChatGPTUsernamesString.Split(' ');
        foreach (string s in temp)
            usernames.Add(s);

        skinNumber = Random.Range(0, 13);
    }

    void Update()
    {
        if (GAMESTATE == "MAIN MENU")
        {
            PlayersLeftText.text = "";
        }
        else if (GAMESTATE == "GAMEPLAY")
        {
            PlayersLeftText.text = PLAYERSLEFT.ToString()/* + "/" + Maxplayers*/;
        }
        else if (GAMESTATE == "EARLY START")
        {
            PlayersLeftText.text = PLAYERSLEFT.ToString()/* + "/" + Maxplayers*/;
        }
        else
        {
            PlayersLeftText.text = "";
        }

        trophiesWinText.text = trophiesEarned.ToString();
        trophiesLoseText.text = trophiesEarned.ToString();
        coinsWinText.text = coinsEarned.ToString();
        coinsLoseText.text = coinsEarned.ToString();

        fullcoinstext.text = TOTALCOINS.ToString();
        fulltrophiestext.text = TOTALTROPHIES.ToString();

        timeSinceLastAd += Time.deltaTime;
    }

    public void ChangeSkin(int change)
    {
        skinNumber += change;
        if (skinNumber < 0)
            skinNumber = 12;
        else if (skinNumber > 12)
            skinNumber = 0;
    }

    public void AddEnemies(int count)
    {
        List<Vector3> unusedPositions = spawns[mapNumber].spawns;
        Maxplayers = 0;

        for (int i = 0; i < count; i++)
        {
            if (unusedPositions.Count > 0)
            {
                int index = Random.Range(0, unusedPositions.Count - 1);
                Vector3 newPos = unusedPositions[index];
                unusedPositions.RemoveAt(index);

                if (i == 0)
                {
                    player.transform.position = newPos;
                }
                else
                {
                    GameObject newEnemy = Instantiate(enemy, newPos, enemy.transform.rotation);
                    newEnemy.GetComponent<Enemy>().EnemyNumber = i;
                    Enemies.Add(newEnemy);
                    Present.Add(true);
                    PLAYERSLEFT++;
                    AddPlayerKillDataUser(newEnemy);
                }

                Maxplayers++;
            }
        }
    }

    public void SetRandomMap()
    {
        foreach (GameObject m in maps)
            m.SetActive(false);

        if (!mapOnline)
            mapNumber = Random.Range(0, maps.Count);
        maps[mapNumber].GetComponentInChildren<ResetTerrain>().Refresh();
        maps[mapNumber].SetActive(true);
    }

    public void EarlyStart()
    {
        Players.Clear();
        Enemies.Clear();
        Present.Clear();

        SetRandomMap();
        PLAYERSLEFT = 0;
        ResetPlayerKillData();
        ResetRewards();
        AddEnemies(16);

        SetGameState("EARLY START");

        //if (Advertisement.isInitialized && /*!adStarted && */timeSinceLastAd > 300) // 5 Minutes
        //{
        //    Advertisement.Load(myAdUnitId);
        //    Advertisement.Show(myAdUnitId);
        //    // adStarted = true;
        //    timeSinceLastAd = 0;
        //}
    }

    public void AddPlayerDeath(KillData data, GameObject parent)
    {
        foreach (PlayerKillData p in PLAYERKILLDATAS)
        {
            if (p.main == parent)
            {
                p.data.Add(data);
                /*if (parent == player)
                {
                    if (p.data[p.data.Count - 1].howKilled == "Killed")
                    {
                        trophiesEarned += 5;
                        coinsEarned += 10;
                    }
                    else if (p.data[p.data.Count - 1].howKilled == "Duckhunted")
                    {
                        trophiesEarned += 7;
                        coinsEarned += 20;
                    }
                    else if (p.data[p.data.Count - 1].howKilled == "Midaired")
                    {
                        trophiesEarned += 6;
                        coinsEarned += 15;
                    }
                    else if (p.data[p.data.Count - 1].howKilled == "Aerialed")
                    {
                        trophiesEarned += 10;
                        coinsEarned += 25;
                    }
                    else if (p.data[p.data.Count - 1].howKilled == "Sniped")
                    {
                        trophiesEarned += 6;
                        coinsEarned += 20;
                    }
                    else
                    {
                        trophiesEarned += 999;
                        coinsEarned += 999;
                    }
                }

                killIndicator.PopUpWithText(p.data[p.data.Count - 1].howKilled, "no usernames yet :(");*/

                return;
            }
        }
    }

    public void CalculatePlayerEarnings()
    {
        trophiesEarned = Mathf.FloorToInt(Maxplayers / 2) - PLAYERSLEFT;
        coinsEarned = trophiesEarned * 2;
    }

    public void ResetRewards()
    {
        trophiesEarned = 0;
        coinsEarned = 0;
    }

    public void ResetPlayerKillData()
    {
        PLAYERKILLDATAS.Clear();
    }

    public void AddPlayerKillDataUser(GameObject user)
    {
        PLAYERKILLDATAS.Add(new PlayerKillData(user, new List<KillData>()));
    }

    public void SetGameState(string state)
    {
        GAMESTATE = state;
    }

    public void AddToPlayers(GameObject g)
    {
        Players.Add(g);
    }

    public void AddToEnemies(GameObject g)
    {
        Players.Add(g);
        // Enemies.Add(g);
    }

    public void AddRewards()
    {
        TOTALCOINS += coinsEarned;
        TOTALTROPHIES += trophiesEarned;
        PlayerPrefs.SetInt("Coins", TOTALCOINS);
        PlayerPrefs.SetInt("Trophies", TOTALTROPHIES);
    }
}

[System.Serializable]
public class PlayerKillData
{
    public GameObject main;
    public List<KillData> data = new List<KillData>();

    public PlayerKillData (GameObject g, List<KillData> d)
    {
        main = g;
        data = d;
    }

    public PlayerKillData ()
    {
        main = null;
        data = null;
    }
}

[System.Serializable]
public class KillData
{
    public GameObject killedPlayer;
    public string howKilled;

    public KillData (GameObject k, string h)
    {
        killedPlayer = k;
        howKilled = h;
    }
}