using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Ability", menuName = "Pokemon/ScriptableObjects/Ability", order = 12)]
public class AbilitySO : ScriptableObject
{
    public new string name;
    public string description;
}
