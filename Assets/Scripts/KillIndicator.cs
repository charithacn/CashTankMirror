using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillIndicator : MonoBehaviour
{
    public TMP_Text cause;
    public TMP_Text username;

    bool going;
    Vector2 start;
    Vector2 end;
    float progress;
    float length;

    public void PopUpWithText(string KillCause, string Username)
    {
        cause.text = KillCause;
        username.text = Username;

        FromTop(0.75f);
        StartCoroutine("DropDownThenBack");
    }

    void Update()
    {
        if (going)
        {
            progress += Time.deltaTime / length;
            if (progress > 1)
            {
                End();
            }
            gameObject.transform.position = Vector2.Lerp(start, end, progress);
        }
    }

    IEnumerator DropDownThenBack()
    {
        yield return new WaitForSeconds(2);
        ToTop(0.75f);
    }

    void FromTop(float time)
    {
        if (progress < 1 && progress != 0)
            SkipAnimation();

        start = new Vector2(Screen.width / 2, Screen.height + 320);
        end = new Vector2(Screen.width / 2, Screen.height - 48);
        length = time;
        going = true;
        progress = 0;
    }

    void ToTop(float time)
    {
        if (progress < 1 && progress != 0)
            SkipAnimation();

        start = new Vector2(Screen.width / 2, Screen.height - 48);
        end = new Vector2(Screen.width / 2, Screen.height + 320);
        length = time;
        going = true;
        progress = 0;
    }

    public void SkipAnimation()
    {
        StopAllCoroutines();
        End();

        gameObject.transform.position = end;
    }

    void End()
    {
        progress = 1;
        going = false;
    }
}