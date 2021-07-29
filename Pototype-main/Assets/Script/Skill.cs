using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public Character.Element element;
    public string skillName;
    public int MPCost, ID;

    public Skill(Character.Element element, int ID, int MPCost, string skillName = "null")
    {
        this.element = element;
        this.ID = ID;
        this.MPCost = MPCost;
        this.skillName = skillName;
    }


}
