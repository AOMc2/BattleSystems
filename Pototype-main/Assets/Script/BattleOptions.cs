using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOptions : MonoBehaviour
{
    Database database;
    public GameObject targetImage;

    private void Awake()
    {
        database = GameObject.Find("Database").GetComponent<Database>();
    }

    public void Attack()
    {
        GameObject targetController = Instantiate(targetImage, getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex), Quaternion.identity);
        targetController.transform.SetParent(GameObject.Find("Canvas").transform.GetChild(1).GetChild(1));
        StartCoroutine("WaitSelect");    
    }

    IEnumerator WaitSelect()
    {
        for (int i = 0; i < 4; i++)
        {
            if (transform.parent.GetChild(i).name != "Attack")
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        RectTransform temp = GetComponent<RectTransform>();
        temp.sizeDelta = Vector2.zero;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (Database.isSelectAlly == false) // Enemy
                {
                    if (Database.selectedTargetIndex - 1 >= 0)
                    {
                        Database.selectedTargetIndex--;
                        transform.GetChild(0).position = getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex);
                    }
                    else
                    {
                        Database.isSelectAlly = true;
                        Database.selectedTargetIndex = 0;
                        transform.GetChild(0).position = getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex);
                    }
                }
                else //Ally
                {
                    if(Database.selectedTargetIndex + 1 < database.allyDetails.Count)
                    {
                        Database.selectedTargetIndex++;
                        transform.GetChild(0).position = getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (Database.isSelectAlly == false) //Enemy
                {
                    if (Database.selectedTargetIndex + 1 < database.enemyDetails.Count)
                    {
                        Database.selectedTargetIndex++;
                        transform.GetChild(0).position = getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex);
                    }
                }
                else //Ally
                {
                    if(Database.selectedTargetIndex - 1 < 0)
                    {
                        Database.selectedTargetIndex = 0;
                        Database.isSelectAlly = false;
                        transform.GetChild(0).position = getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex);
                    }
                    else
                    {
                        Database.selectedTargetIndex--;
                        transform.GetChild(0).position = getTargetImagePosition(Database.isSelectAlly, Database.selectedTargetIndex);
                    }
                }
            }
            yield return null;
        }
        Database.isAttack = true;
        temp.sizeDelta = new Vector2(10, 1);
        Destroy(transform.GetChild(0).gameObject);
        Debug.Log(transform.GetChild(0).name);
        gameObject.SetActive(false);
    }

    private Vector2 getTargetImagePosition(bool isAlly, int index)
    {
        if (isAlly == true)
        {
            return new Vector2(0 + index * -2, 2);
        }
        else
        {
            return new Vector2(4 + index * 2, 2);
        }
    }

    public void Skill()
    {
        StartCoroutine("WaitSelect2");
    }

    IEnumerator WaitSelect2()
    {
        transform.parent.localScale = Vector2.zero;
        for (int i = 0; i < 4; i++)
        {
            if (transform.parent.GetChild(i).name != "Skill")
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (Database.selectedTargetIndex - 1 >= 0)
                {
                    Database.selectedTargetIndex--;
                    Debug.Log(Database.selectedTargetIndex);
                }
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (Database.selectedTargetIndex + 1 < database.enemyDetails.Count)
                {
                    Database.selectedTargetIndex++;
                    Debug.Log(Database.selectedTargetIndex);
                }
            }
            yield return null;
        }
        Database.isSkill = true;
        transform.parent.localScale = new Vector2(1, 1);
        gameObject.SetActive(false);
    }

    public void Item()
    {
        StartCoroutine("WaitSelect3");
    }

    IEnumerator WaitSelect3()
    {
        transform.parent.localScale = Vector2.zero;
        for (int i = 0; i < 4; i++)
        {
            if (transform.parent.GetChild(i).name != "Item")
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (Database.selectedTargetIndex - 1 >= 0)
                {
                    Database.selectedTargetIndex--;
                    Debug.Log(Database.selectedTargetIndex);
                }
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (Database.selectedTargetIndex + 1 < database.enemyDetails.Count)
                {
                    Database.selectedTargetIndex++;
                    Debug.Log(Database.selectedTargetIndex);
                }
            }
            yield return null;
        }
        Database.isItem = true;
        transform.parent.localScale = new Vector2(1, 1);
        gameObject.SetActive(false);
    }

}
