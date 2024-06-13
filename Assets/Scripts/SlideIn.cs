using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideIn : MonoBehaviour
{
    public GameObject main;
    bool going;
    Vector2 start;
    Vector2 end;
    float progress;
    float length;
    byte extraInfo;

    public PlayerHandler playerHandler;

    void Awake()
    {
        playerHandler = GameObject.Find("PlayerHandler").GetComponent<PlayerHandler>();
    }

    void Update()
    {
        if (going)
        {
            if (!main.activeSelf)
                main.SetActive(true);

            progress += Time.deltaTime / length;
            if (progress > 1)
            {
                End();
            }
            main.transform.position = Vector2.Lerp(start, end, progress);
        }
    }

    public void SkipAnimation()
    {
        End();

        main.transform.position = end;
    }

    void End()
    {
        progress = 1;
        going = false;
        if (extraInfo == 1)
        {
            GameObject.Find("Player").GetComponent<Movement>().StartPlayer();
            main.SetActive(false);
        }
        else if (extraInfo == 2)
        {
            GameObject.Find("Player").GetComponent<Movement>().EndPlayer();
        }
        else if (extraInfo == 3)
        {
            StartCoroutine("WaitThenFromTop");
        }
    }

    IEnumerator WaitThenFromTop()
    {
        yield return new WaitForSeconds(2);
        GameObject.Find("Player").GetComponent<Movement>().menu.GetComponent<SlideIn>().FromTop(1, 2);
        yield return new WaitForSeconds(1);
        main.SetActive(false);
    }

    public void FromTop(float time, int info)
    {
        if (progress < 1 && progress != 0)
            SkipAnimation();

        start = new Vector2(Screen.width / 2, Screen.height * 1.5f);
        end = new Vector2(Screen.width / 2, Screen.height / 2);
        length = time;
        going = true;
        progress = 0;
        main.SetActive(true);
        if (info > 0)
            extraInfo = (byte)info;
    }

    public void ToTop(float time, bool startGame)
    {
        if (progress < 1 && progress != 0)
            SkipAnimation();

        start = new Vector2(Screen.width / 2, Screen.height / 2);
        end = new Vector2(Screen.width / 2, Screen.height * 1.5f);
        length = time;
        going = true;
        progress = 0;
        main.SetActive(true);
        if (startGame)
        {
            extraInfo = 1;
            GameObject.Find("Player").GetComponent<Movement>().EarlyStart(0);
        }
    }

    public void ToTopTrue(float time)
    {
        ToTop(time, true);
    }
}