using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePlayer : MonoBehaviour
{
    float timer;
    public Movement PLAYER;

    public GameObject Front;
    public GameObject Back;
    public GameObject Gun;
    public GameObject Socket;
    public GameObject Wheel1;
    public GameObject Wheel2;
    public GameObject Window;

    Image _Front;
    Image _Back;
    Image _Gun;
    Image _Socket;
    Image _Wheel1;
    Image _Wheel2;
    Image _Window;

    public PlayerHandler p;

    void Start()
    {
        _Front = Front.GetComponent<Image>();
        _Back = Back.GetComponent<Image>();
        _Gun = Gun.GetComponent<Image>();
        _Socket = Socket.GetComponent<Image>();
        _Wheel1 = Wheel1.GetComponent<Image>();
        _Wheel2 = Wheel2.GetComponent<Image>();
        _Window = Window.GetComponent<Image>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        Vector3 diff = /*Camera.main.ScreenToWorldPoint(*/Input.mousePosition/*)*/ - Gun.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        transform.localScale = new Vector3(-(Mathf.Sin(timer * 3) / 20) - 1, ((-Mathf.Pow(0.25f, Mathf.Sin(timer * 9.12f + 0.75f)) + 2) / 19) + 1);
        Gun.transform.rotation = Quaternion.Euler(0, 0, rot_z/* - 180*/);

        _Front.sprite = PLAYER.fronts[p.skinNumber];
        _Back.sprite = PLAYER.backs[p.skinNumber];
        _Gun.sprite = PLAYER.guns[p.skinNumber];
        _Socket.sprite = PLAYER.sockets[p.skinNumber];
        Sprite wheelSprite = PLAYER.wheels[p.skinNumber];
        _Wheel1.sprite = wheelSprite;
        _Wheel2.sprite = wheelSprite;
        _Window.sprite = PLAYER.windows[p.skinNumber];
    }
}