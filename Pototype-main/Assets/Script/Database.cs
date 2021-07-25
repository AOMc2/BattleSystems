using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Database : MonoBehaviour
{
    public static bool isHandling = false, isAttack = false, isSkill = false, isItem = false;
    public GameObject c, s;
    public GameObject[] characterSprites;
    public List<GameObject> allyDetails;
    public List<GameObject> enemyDetails;
    public GameObject battleOptions;

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

    private void Start()
    {
        battleOptions = GameObject.Find("BattleOptions");

        AddCharacterToAllyList(100, 100, 10, 5, 2, 15, Character.Element.fire, 0);
        AddCharacterToAllyList(100, 100, 10, 5, 3, 15, Character.Element.water, 0);
        AddCharacterToAllyList(100, 100, 10, 5, 8, 15, Character.Element.earth, 0);

        AddCharacterToEnemyList(100, 100, 10, 5, 5, 15, Character.Element.fire, 0);
        AddCharacterToEnemyList(100, 100, 10, 5, 6, 15, Character.Element.water, 0);
        AddCharacterToEnemyList(100, 100, 10, 5, 7, 15, Character.Element.earth, 0);

        CreateAlly();
        CreateEnemy();

    }
}
