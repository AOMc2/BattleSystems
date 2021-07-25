using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCharacter : MonoBehaviour
{
    public bool isBarCharacter;
    public Character characterStats;
    public Database database;
    public Animator animator;
    public SceneCharacter parent;
    public SpriteRenderer myRenderer;

    private int standingPosition = 0;
    private float progress = 0;
    private GameObject sceneCharacter;
    private static int speedBarLength = 16;
    private int repeatRate = 0;
    private Shader shaderGUIText, shaderSpriteDefault;


    private void Start()
    {
        if (isBarCharacter == false)
        {
            sceneCharacter = Instantiate(database.characterSprites[characterStats.ID], transform.position, Quaternion.identity);
            sceneCharacter.transform.SetParent(transform);
            animator = sceneCharacter.GetComponent<Animator>();
            myRenderer = sceneCharacter.GetComponent<SpriteRenderer>();

            GameObject cloner = Instantiate(gameObject);
            cloner.GetComponent<SceneCharacter>().isBarCharacter = true;
            cloner.name = "barIcon";
            cloner.transform.SetParent(transform);
            if (tag == "Enemy")
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector2(0.4f, 0.4f);
            animator = transform.GetChild(0).GetComponent<Animator>();
            myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            parent = transform.parent.GetComponent<SceneCharacter>();
        }
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpriteDefault = Shader.Find("Sprites/Default");
    }

    private void FixedUpdate()
    {
        if (Database.isHandling == false)
        {
            if (isBarCharacter == true)
            {
                if (characterStats.isDead == false)
                {
                    if (progress <= 0)
                    {
                        transform.position = new Vector2(-8, 4);
                        progress += 0.1f;
                    }
                    else if (progress >= speedBarLength)
                    {
                        if (Database.isHandling == false)
                        {
                            Database.isHandling = true;
                            if (characterStats.isAlly == true)
                            {
                                Database.isAttack = false;
                                Database.isSkill = false;
                                Database.isItem = false;
                                database.battleOptions.transform.GetChild(0).gameObject.SetActive(true);
                                database.battleOptions.transform.GetChild(1).gameObject.SetActive(true);
                                database.battleOptions.transform.GetChild(2).gameObject.SetActive(true);
                                StartCoroutine("WaitOption");
                            }
                            else
                            {
                                castSkill(characterStats.ID, getSkillID(characterStats.ID, repeatRate), true, 0);
                                StartCoroutine("playAnimation");
                            }
                        }
                    }
                    else
                    {
                        progress += characterStats.speed * Time.deltaTime;
                        transform.position = new Vector2(-8 + progress, 4);
                    }
                }
                else
                {
                    transform.position = new Vector2(-8, 4);
                    progress = 0;
                    myRenderer.material.shader = shaderGUIText;
                    myRenderer.color = Color.grey;
                }
            }
            else
            {
                if (characterStats.isAlly == true)
                {
                    standingPosition = database.allyDetails.IndexOf(characterStats.gameObject);
                    transform.position = new Vector2(-2 + standingPosition * -2, -2);
                }
                else
                {
                    standingPosition = database.enemyDetails.IndexOf(characterStats.gameObject);
                    transform.position = new Vector2(2 + standingPosition * 2, -2);
                }

                if (characterStats.currentHP <= 0 && characterStats.isDead == false)
                {
                    if (characterStats.isAlly == true)
                    {
                        database.allyDetails.Remove(characterStats.gameObject);
                        database.allyDetails.Add(characterStats.gameObject);
                        characterStats.isDead = true;
                        myRenderer.material.shader = shaderGUIText;
                        myRenderer.color = Color.grey;

                        int deathNumber = 0;
                        for (int i = 0; i < database.allyDetails.Count; i++)
                        {
                            if (database.allyDetails[i].GetComponent<Character>().isDead == true)
                            {
                                deathNumber++;
                            }
                        }
                        if (deathNumber == database.allyDetails.Count)
                        {
                            Debug.Log("All Allies Died.");
                        }
                    }

                }
            }
        }
    }

    public void isHit()
    {
        StartCoroutine("Flash");
    }

    IEnumerator Flash()
    {
        myRenderer.material.shader = shaderGUIText;
        myRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        myRenderer.material.shader = shaderSpriteDefault;
        myRenderer.color = Color.white;
    }

    IEnumerator playAnimation()
    {
        parent.animator.SetBool("isAttack", true);
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).speed);
        repeatRate++;
        parent.animator.SetBool("isAttack", false);
        animator.SetBool("isAttack", false);
        Database.isHandling = false;
        progress = 0;
    }

    private int getSkillID(int ID, int repeatRate)
    {
        int maxSkillPattern = 0;
        switch (ID)
        {
            case 0:
                maxSkillPattern = 1;
                break;
        }
        return repeatRate % maxSkillPattern;
    }

    private Character getCharacterItem(bool isAlly, int index)
    {
        if (isAlly == true)
        {
            return database.allyDetails[index].GetComponent<Character>();
        }
        return database.enemyDetails[index].GetComponent<Character>();
    }

    private void castSkill(int ID, int skillID, bool isAllySide, int index)
    {
        Character target = getCharacterItem(isAllySide, index);
        switch (ID)
        {
            case 0:
                if (skillID == 0)
                {
                    target.currentHP -= characterStats.attackDamage;
                }
                break;
        }
        target.sceneCharacter.isHit();
    }

    IEnumerator WaitOption()
    {
        yield return new WaitUntil(() => Database.isAttack == true || Database.isSkill == true || Database.isItem == true);
        StartCoroutine("playAnimation");
    }
}
