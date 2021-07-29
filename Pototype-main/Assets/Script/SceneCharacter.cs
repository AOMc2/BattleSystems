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
    private BattleMenu battleMenu;
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
            battleMenu = GameObject.Find("BattleMenu").GetComponent<BattleMenu>();
            parent = transform.parent.GetComponent<SceneCharacter>();
        }
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpriteDefault = Shader.Find("Sprites/Default");
    }

    private void FixedUpdate()
    {
        if (database.isHandling == false)
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
                        if (database.isHandling == false)
                        {
                            database.isHandling = true;
                            if (characterStats.isAlly == true)
                            {
                                StartCoroutine("WaitOption");
                            }
                            else
                            {
                                performAttackPattern(characterStats.ID, getAttackID(characterStats.ID, repeatRate), 0, true);
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
                    if (characterStats.specialEffects.Count > 0)
                    {
                        for(int i = 0; i< characterStats.specialEffects.Count; i++)
                        {
                            Destroy(characterStats.specialEffects[i]);
                        }
                        characterStats.specialEffects.Clear();
                    }
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
                            if(database.allyDetails[i].GetComponent<Character>().isDead == true)
                            {
                                deathNumber++;
                            }
                        }
                        if (deathNumber == database.allyDetails.Count)
                        {
                            Debug.Log("All Allies Died.");
                        }
                    }
                    else
                    {
                        database.enemyDetails.Remove(characterStats.gameObject);
                        database.enemyDetails.Add(characterStats.gameObject);
                        characterStats.isDead = true;
                        myRenderer.material.shader = shaderGUIText;
                        myRenderer.color = Color.grey;

                        int deathNumber = 0;
                        for (int i = 0; i < database.enemyDetails.Count; i++)
                        {
                            if (database.enemyDetails[i].GetComponent<Character>().isDead == true)
                            {
                                deathNumber++;
                            }
                        }
                        if (deathNumber == database.enemyDetails.Count)
                        {
                            Debug.Log("All Enemies Died.");
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
        if (characterStats.isDead == true)
        {
            myRenderer.material.shader = shaderGUIText;
            myRenderer.color = Color.grey;
        }
        else
        {
            myRenderer.material.shader = shaderSpriteDefault;
            myRenderer.color = Color.white;
        }
    }

    IEnumerator playAnimation()
    {
        parent.animator.SetBool("isAttack", true);
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).speed);
        if (characterStats.specialEffects.Count != 0)
        {
            for (int i = 0; i < characterStats.specialEffects.Count; i++)
            {
                ElementEffect temp = characterStats.specialEffects[i].GetComponent<ElementEffect>();
                temp.roundRemainings--;
                if (temp.roundRemainings <= 0)
                {
                    characterStats.specialEffects.Remove(characterStats.specialEffects[i]);
                    Destroy(temp.gameObject);
                    for (int j = i; j < characterStats.specialEffects.Count; j++)
                    {
                        characterStats.specialEffects[j].transform.position += new Vector3(0, -1, 0);
                    }
                }
            }
        }
        repeatRate++;
        parent.animator.SetBool("isAttack", false);
        animator.SetBool("isAttack", false);
        database.isHandling = false;
        progress = 0;
    }

    // For Enemy Only
    private int getAttackID(int ID, int repeatRate)
    {
        int maxSkillPattern = 0;
        switch (ID)
        {
            case 0:
                maxSkillPattern = 1;
                break;
            default:
                maxSkillPattern = -1;
                break;
        }
        if (maxSkillPattern == -1)
        {
            return -1;
        }
        return repeatRate % maxSkillPattern;
    }

    private Character getTarget(int index, bool isAlly)
    {
        if (isAlly)
        {
            return database.allyDetails[index].GetComponent<Character>();
        }
        else
        {
            return database.enemyDetails[index].GetComponent<Character>();
        }
    }

    // For Enemy Only, AttackID Sets To -1 = Basic Attack
    private void performAttackPattern(int ID, int attackID, int index, bool isAlly)
    {
        Character target = getTarget(index, isAlly);
        if (attackID == -1)
        {
            target.currentHP -= characterStats.attackDamage;
        }
        else
        {
            switch (ID)
            {
                case 0:
                    if (attackID == 0)
                    {
                        target.currentHP -= characterStats.attackDamage;
                    }
                    break;
            }
        }
        target.AddElementEffect(2, target, characterStats.element);
        target.sceneCharacter.isHit();
    }

    IEnumerator WaitOption()
    {
        database.isSelectedOption = false;
        database.selector = database.allyDetails.IndexOf(characterStats.gameObject);
        battleMenu.Show();
        yield return new WaitUntil(() => database.isSelectedOption == true);
        database.isSelectedOption = false;
        switch (database.selectedState)
        {
            case 0:
                performAttackPattern(0, -1, database.selectedIndex, false);
                break;
            case 1:
                Debug.Log("Caster: " + database.selector + ", Skill: " + database.allyDetails[database.selector].GetComponent<Character>().skills[database.selectedItem].skillName + ", Is Ally Side: " + database.isAllySelected + ", Target Index: " + database.selectedIndex);
                break;
            case 2:
                Debug.Log("Caster: " + database.selector + ", Item: " + database.inventory[database.selectedItem].itemName + ", Is Ally Side: " + database.isAllySelected + ", Target Index: " + database.selectedIndex);
                break;
        }
        StartCoroutine("playAnimation");
    }
}
