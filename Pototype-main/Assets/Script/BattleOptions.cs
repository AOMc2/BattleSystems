using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOptions : MonoBehaviour
{

    public void Attack()
    {
        Database.isAttack = true;
        for(int i = 0; i < 3; i++)
        {
            if(transform.parent.GetChild(i).name != "Attack")
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }

    public void Skill()
    {
        Database.isSkill = true;
        for (int i = 0; i < 3; i++)
        {
            if (transform.parent.GetChild(i).name != "Skill")
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }

    public void Item()
    {
        Database.isItem = true;
        for (int i = 0; i < 3; i++)
        {
            if (transform.parent.GetChild(i).name != "Item")
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }

}
