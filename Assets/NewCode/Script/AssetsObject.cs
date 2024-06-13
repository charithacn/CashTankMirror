using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/AssetsObject", order = 1)]
public class AssetsObject : ScriptableObject
{
    public Sprite[] body;
    public Sprite[] bodyBack;
    public Sprite[] realTyre;
    public Sprite[] frontTyre;
    public Sprite[] gun;

    public Sprite SelectionBody;
    public Sprite selectionBodyBack;
    public Sprite selectionRealTyre;
    public Sprite selectionFrontTyre;
    public Sprite selectionGun;

    public Sprite[] VSbody;
    public Sprite[] VSbodyBack;
    public Sprite[] VSrealTyre;
    public Sprite[] VSfrontTyre;
    public Sprite[] VSgun;
}
