using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHP, currentHP, maxMP, currentMP, defense, dodgeRate, speed, attackDamage, ID;
    public enum Element { fire, water, wind, earth, electricity };
    public Element element;
    public bool isAlly, isDead = false;
    public SceneCharacter sceneCharacter;

    public void SetCharacter(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Element element, int ID)
    {
        this.maxHP = maxHP;
        currentHP = maxHP;
        this.maxMP = maxMP;
        currentMP = maxMP;
        this.defense = defense;
        this.dodgeRate = dodgeRate;
        this.speed = speed;
        this.attackDamage = attackDamage;
        this.element = element;
        this.ID = ID;
    }
}
