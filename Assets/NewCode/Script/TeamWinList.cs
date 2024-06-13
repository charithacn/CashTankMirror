using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WinSet", order = 1)]
public class TeamWinList : ScriptableObject
{
    public List<string> winList;
}
