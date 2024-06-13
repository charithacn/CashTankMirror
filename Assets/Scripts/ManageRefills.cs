using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageRefills : MonoBehaviour
{
    public GameObject refillObject;
    public List<GameObject> current = new List<GameObject>();
    public PlayerHandler playerHandler;

    public float AMMO;
    public float Refilltime;
    public int Max;

    void Update()
    {
        if (playerHandler.GAMESTATE == "GAMEPLAY")
        {
            AMMO += Time.deltaTime / Refilltime;
            if (AMMO > Max)
                AMMO = Max;

            for (int i = 0; i < Max; i++)
            {
                if (i <= AMMO - 1)
                    current[i].GetComponent<Slider>().value = 1;
                else if (i > AMMO)
                    current[i].GetComponent<Slider>().value = 0;
                else if (i == Mathf.FloorToInt(AMMO))
                    current[i].GetComponent<Slider>().value = AMMO % 1;
            }
        }
    }

    public void ResetObjectCount()
    {
        foreach (GameObject g in current)
            Destroy(g);
        current.Clear();

        for (int i = 0; i < Max; i++)
        {
            current.Add(Instantiate(refillObject, transform)); // 0 / 3 = 0; 1 / 3 = 1/3
            current[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(i * 250 - Max * 125, 25, 0);
        }

        AMMO = Max;
    }
}