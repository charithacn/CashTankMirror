using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    // public Vector2 change;
    public PlayerHandler p;

    /*public Button left;
    public Button right;
    public Button up;
    public Button down;*/

    // Vector2 direciton;

    public void MoveX(float x)
    {
        if (x != p.Movement.x)
            p.Movement.x = x;
        else
            p.Movement.x = 0;
    }

    public void MoveY(float y)
    {
        if (y != p.Movement.y)
            p.Movement.y = y;
        else
            p.Movement.y = 0;
    }
}