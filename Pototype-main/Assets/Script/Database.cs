using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public bool isHandling = false, isSelectedOption = false, isAllySelected = false;
    public int selectedState = 0, selectedIndex = 0, selector = 0, selectedItem = 0;
    public GameObject c, s;
    public GameObject[] characterSprites;
    public List<GameObject> allyDetails;
    public List<GameObject> enemyDetails;
    public List<Item> inventory;

    public void AddCharacterToAllyList(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Character.Element element, int ID)
    {
        GameObject cloner = Instantiate(c);
        cloner.GetComponent<Character>().SetCharacter(maxHP, maxMP, defense, dodgeRate, speed, attackDamage, element, ID);
        cloner.GetComponent<Character>().isAlly = true;
        cloner.name = ID.ToString();
        cloner.transform.SetParent(transform);
        allyDetails.Add(cloner);
    }

    public void AddCharacterToEnemyList(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Character.Element element, int ID)
    {
        GameObject cloner = Instantiate(c);
        cloner.GetComponent<Character>().SetCharacter(maxHP, maxMP, defense, dodgeRate, speed, attackDamage, element, ID);
        cloner.GetComponent<Character>().isAlly = false;
        cloner.name = ID.ToString();
        cloner.transform.SetParent(transform);
        enemyDetails.Add(cloner);
    }

    public void CreateAlly()
    {
        for (int i = 0; i < allyDetails.Count; i++)
        {
            GameObject cloner = Instantiate(s, new Vector2(-2 + i * -2, -2), Quaternion.identity);
            SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
            temp.characterStats = allyDetails[i].GetComponent<Character>();
            temp.database = this;
            temp.isBarCharacter = false;
            allyDetails[i].GetComponent<Character>().sceneCharacter = temp;
            cloner.tag = "Ally";
        }
    }

    public void CreateEnemy()
    {
        for (int i = 0; i < enemyDetails.Count; i++)
        {
            GameObject cloner = Instantiate(s, new Vector2(2 + i * 2, -2), Quaternion.identity);
            SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
            temp.characterStats = enemyDetails[i].GetComponent<Character>();
            temp.database = this;
            temp.isBarCharacter = false;
            enemyDetails[i].GetComponent<Character>().sceneCharacter = temp;
            cloner.tag = "Enemy";
        }
    }

    public void AddItemToInventory(string name, int amount)
    {
        bool isExist = false;
        int existingIndex = 0;
        for(int i = 0; i < inventory.Count; i++)
        {
            if (name == inventory[i].itemName)
            {
                isExist = true;
                existingIndex = i;
            }
        }
        if (isExist == true)
        {
            inventory[existingIndex].itemAmount += amount;
        }
        else
        {
            inventory.Add(new Item(name, amount));
        }
    }

    private void Start()
    {
        AddCharacterToAllyList(100, 100, 10, 5, 8, 15, Character.Element.fire, 0);
        AddCharacterToAllyList(100, 100, 10, 5, 3, 15, Character.Element.water, 0);
        AddCharacterToAllyList(100, 100, 10, 5, 4, 15, Character.Element.earth, 0);
        allyDetails[0].GetComponent<Character>().AddSkill(new Skill(Character.Element.wind, 0, 10, "Dodge"));
        allyDetails[0].GetComponent<Character>().AddSkill(new Skill(Character.Element.wind, 1, 10, "AgainstTheCurrent"));
        allyDetails[0].GetComponent<Character>().AddSkill(new Skill(Character.Element.fire, 2, 10, "Fireball"));
        allyDetails[0].GetComponent<Character>().AddSkill(new Skill(Character.Element.fire, 3, 10, "Explosion"));


        AddCharacterToEnemyList(20, 100, 10, 5, 5, 15, Character.Element.fire, 0);
        AddCharacterToEnemyList(100, 100, 10, 5, 6, 15, Character.Element.water, 0);
        AddCharacterToEnemyList(100, 100, 10, 5, 7, 15, Character.Element.earth, 0);

        //CreateMap(0);
        CreateAlly();
        CreateEnemy();

        AddItemToInventory("HP Potion", 5);
        AddItemToInventory("MP Potion", 15);
        AddItemToInventory("Strength Potion", 10);
        AddItemToInventory("Revive Potion", 8);

    }
}
