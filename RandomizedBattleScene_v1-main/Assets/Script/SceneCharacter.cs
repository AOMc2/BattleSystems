using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCharacter : MonoBehaviour
{
    public bool isBarCharacter;
    public Character characterStats;
    public Database database;
    public Animator animator;
    public SceneCharacter sceneCharacter, barCharacter;
    public SpriteRenderer myRenderer;
    public int repeatRate = 0;
    public float progress = 0;

    private BattleMenu battleMenu;
    private static int speedBarLength = 16;
    private Shader shaderGUIText, shaderSpriteDefault;
    private TMPro.TextMeshProUGUI HPIndicatorHolder;

    private void Start()
    {
        if (isBarCharacter == false)
        {
            GameObject cloner1 = Instantiate(database.characterSprites[characterStats.ID], transform.position, Quaternion.identity);
            cloner1.transform.SetParent(transform);
            animator = cloner1.GetComponent<Animator>();
            myRenderer = cloner1.GetComponent<SpriteRenderer>();
            myRenderer.sortingLayerName = "character";

            GameObject cloner2 = Instantiate(gameObject);
            barCharacter = cloner2.GetComponent<SceneCharacter>();
            barCharacter.isBarCharacter = true;
            barCharacter.name = "barIcon";
            barCharacter.transform.SetParent(transform);

            HPIndicatorHolder = Instantiate(characterStats.textPrefab).GetComponent<TMPro.TextMeshProUGUI>();
            HPIndicatorHolder.transform.position = transform.position;
            HPIndicatorHolder.transform.position += new Vector3(1, -2.1f, 0);
            HPIndicatorHolder.color = Color.black;
            HPIndicatorHolder.transform.SetParent(GameObject.Find("Canvas").transform);
            if (characterStats.isAlly == true)
            {
                HPIndicatorHolder.text = "HP" + characterStats.currentHP + "\nMP" + characterStats.currentMP;
            }
            else
            {
                HPIndicatorHolder.text = "HP" + characterStats.currentHP;
            }
            HPIndicatorHolder.fontSize = 0.2f;
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
            myRenderer.sortingLayerName = "character";
            battleMenu = GameObject.Find("BattleMenu").GetComponent<BattleMenu>();
            sceneCharacter = transform.parent.GetComponent<SceneCharacter>();
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
                                database.logMessage.AddMessage("<Enemy Turn!>");
                                performAttackPattern(characterStats.ID, getAttackID(characterStats.ID, repeatRate), 0, true);
                                StartCoroutine("attack");
                            }
                        }
                    }
                    else
                    {
                            if (characterStats.speed + characterStats.extraSpeed > 0)
                            {
                                progress += (characterStats.speed + characterStats.extraSpeed) * Time.deltaTime;
                            }
                            else
                            {
                                progress += 1 * Time.deltaTime;
                            }
                            transform.position = new Vector2(-8 + progress, 4);
                    }
                }
            }
            else
            {
                if (characterStats.currentHP <= 0 && characterStats.isDead == false)
                {
                    HPIndicatorHolder.text = "Dead";
                    if (characterStats.isAlly == true)
                    {
                        database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(characterStats.gameObject) + " experienced a bloody death!");
                        database.allyDetails.Remove(characterStats.gameObject);
                        database.allyDetails.Add(characterStats.gameObject);
                        characterStats.isDead = true;
                        myRenderer.material.shader = shaderGUIText;
                        myRenderer.color = Color.grey;
                        barCharacter.myRenderer.material.shader = shaderGUIText;
                        barCharacter.myRenderer.material.shader = shaderGUIText;
                        barCharacter.progress = 0;
                        barCharacter.transform.position = new Vector2(-8, 4);

                        int deathNumber = RepositionCharacter(true);
                        if (deathNumber == database.allyDetails.Count)
                        {
                            for(int i = 0; i < database.allyDetails.Count; i++)
                            {
                                Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                                Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                Destroy(tempCharacter.sceneCharacter.gameObject);
                                Destroy(tempCharacter.gameObject);
                            }
                            database.allyDetails.Clear();

                            for (int i = 0; i < database.enemyDetails.Count; i++)
                            {
                                Character tempCharacter = database.enemyDetails[i].GetComponent<Character>();
                                Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                Destroy(tempCharacter.sceneCharacter.gameObject);
                                Destroy(tempCharacter.gameObject);
                            }
                            database.enemyDetails.Clear();

                            database.currentWave = 0;
                            database.waitingEnemies.Clear();
                            database.logMessage.DeleteLog();
                            database.logMessage.AddMessage("[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "] <Battle Ended!>");
                            database.logMessage.AddMessage("All allies died!");
                            database.logMessage.AddMessage("<Game Over!>");
                            database.logMessage.Print(LogMessage.closeStatus.backToBigMap);
                        }
                    }
                    else
                    {
                        database.logMessage.AddMessage("Enemy " + database.enemyDetails.IndexOf(characterStats.gameObject) + " experienced a bloody death!");
                        int reward = (int)(characterStats.maxHP * 0.8f + (characterStats.defense + characterStats.dodgeRate) * 0.4f + (characterStats.speed + characterStats.attackDamage) * 1.5f);
                        database.coinGainInOneRound += reward;
                        database.logMessage.AddMessage("You gained " + reward + " gold!");

                        database.enemyDetails.Remove(characterStats.gameObject);
                        characterStats.isDead = true;

                        int deathNumber = RepositionCharacter(false);
                        if (deathNumber == database.enemyDetails.Count)
                        {
                            database.isHandling = true;
                            for (int i = 0; i < database.enemyDetails.Count; i++) // Destroy All Enemies
                            {
                                Destroy(database.enemyDetails[i].GetComponent<Character>().sceneCharacter.HPIndicatorHolder.gameObject);
                                Destroy(database.enemyDetails[i].GetComponent<Character>().sceneCharacter.gameObject);
                                Destroy(database.enemyDetails[i]);
                            }
                            database.enemyDetails.Clear();

                            for (int i = 0; i < database.allyDetails.Count; i++) // Set Allies Progress Back To Zero
                            {
                                database.allyDetails[i].GetComponent<Character>().sceneCharacter.barCharacter.progress = 0;
                            }

                            if (database.currentWave + 1 == database.totalWave)
                            {
                                for (int i = 0; i < database.allyDetails.Count; i++) // Clear All Effects On Allies
                                {
                                    Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                                    tempCharacter.effects.Clear();
                                    tempCharacter.statsEffects.Clear();
                                    if (tempCharacter.isDead == true) // Destroy Ally If Dead
                                    {
                                        database.allyDetails.Remove(tempCharacter.gameObject);
                                        Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                        Destroy(tempCharacter.sceneCharacter.gameObject);
                                        Destroy(tempCharacter.gameObject);
                                    }
                                    else
                                    {
                                        Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                        Destroy(tempCharacter.sceneCharacter.barCharacter.gameObject);
                                        tempCharacter.sceneCharacter.enabled = false;
                                    }
                                }
                                database.currentWave = 0;
                                database.waitingEnemies.Clear();
                                database.logMessage.AddMessage("[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "] <Battle Ended!>");
                                database.logMessage.AddMessage("Current Gold: " + database.coin + " G + " + database.coinGainInOneRound + " G => " + (database.coin + database.coinGainInOneRound) + " G");
                                database.coin = database.coin + database.coinGainInOneRound;

                                if (Random.Range(1, 6) == 1)
                                {
                                    database.logMessage.Print(LogMessage.closeStatus.backToShop);
                                }
                                else
                                {
                                    database.logMessage.Print(LogMessage.closeStatus.backToBigMap);
                                }
                            }
                            else
                            {
                                database.moveMap.StartLerping();
                            }
                        }
                        Destroy(characterStats.gameObject);
                        Destroy(HPIndicatorHolder.gameObject);
                        Destroy(gameObject);
                    }

                }
                else
                {
                    if (characterStats.isAlly == true)
                    {
                        HPIndicatorHolder.text = "HP" + characterStats.currentHP + "\nMP" + characterStats.currentMP;
                    }
                    else
                    {
                        HPIndicatorHolder.text = "HP" + characterStats.currentHP;
                    }
                }
            }
        }
    }

    public int RepositionCharacter(bool isAlly)
    {
        int deathNumber = 0;
        if (isAlly == true)
        {
            for (int i = 0; i < database.allyDetails.Count; i++)
            {
                Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                if (tempCharacter.isDead == true)
                {
                    deathNumber++;
                }
                tempCharacter.sceneCharacter.transform.position = new Vector2(-2 + i * -2.5f, -2);
                TMPro.TextMeshProUGUI tempHPIndicator = tempCharacter.sceneCharacter.GetComponent<SceneCharacter>().HPIndicatorHolder;
                tempHPIndicator.transform.position = tempCharacter.sceneCharacter.transform.position;
                tempHPIndicator.transform.position += new Vector3(1, -2.1f, 0);
            }
        }
        else
        {
            for (int i = 0; i < database.enemyDetails.Count; i++)
            {
                Character tempCharacter = database.enemyDetails[i].GetComponent<Character>();
                if (tempCharacter.isDead == true)
                {
                    deathNumber++;
                }
                tempCharacter.sceneCharacter.transform.position = new Vector2(2 + i * 2.5f, -2);
                TMPro.TextMeshProUGUI tempHPIndicator = tempCharacter.sceneCharacter.GetComponent<SceneCharacter>().HPIndicatorHolder;
                tempHPIndicator.transform.position = tempCharacter.sceneCharacter.transform.position;
                tempHPIndicator.transform.position += new Vector3(1, -2.1f, 0);
            }
        }
        return deathNumber;
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

    IEnumerator attack()
    {
        sceneCharacter.animator.SetBool("isAttack", true);
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.3f);
        repeatRate++;
        sceneCharacter.animator.SetBool("isAttack", false);
        animator.SetBool("isAttack", false);

        database.isHandling = false;

        for (int i = 0; i < characterStats.statsEffects.Count; i++)
        {
            characterStats.statsEffects[i].FinsihOneRound();
        }
        progress = 0;
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

    private int getOverThread(int current, int addValue, int max = 9999)
    {
        if (current + addValue > max)
        {
            return max;
        }
        if (current + addValue <= 0)
        {
            return 1;
        }
        return current + addValue;
    }

    // For Enemy Only
    private int getAttackID(int ID, int repeatRate)
    {
        int maxSkillPattern = 0;
        switch (ID)
        {
            case 6:
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

    // For Enemy Only, AttackID Sets To -1 = Basic Attack
    private void performAttackPattern(int ID, int attackID, int index, bool isAlly)
    {
        Character target = getTarget(index, isAlly);
        bool isDodged = false;

        if (target != characterStats)
        {
            if (Random.Range(getOverThread(target.dodgeRate, target.extraDodgeRate, 100), 100) == getOverThread(target.dodgeRate, target.extraDodgeRate, 100))
            {
                if (target.isAlly == true)
                {
                    database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(target.gameObject) + " dodged an attack!");
                }
                else
                {
                    database.logMessage.AddMessage("Enemy " + database.enemyDetails.IndexOf(target.gameObject) + " dodged an attack!");
                }
                isDodged = true;
            }
        }

        if (isDodged == false)
        {
            if (attackID == -1)
            {
                characterStats.currentMP += 10;
                if (characterStats.currentMP > characterStats.maxMP)
                {
                    characterStats.currentMP = characterStats.maxMP;
                }
                target.DealDamage(characterStats.attackDamage + characterStats.extraAttackDamage);
            }
            else
            {
                switch (ID)
                {
                    case 6:
                        if (attackID == 0)
                        {
                            database.isHandling = true;
                        }
                        break;
                }
            }
            target.AddEffect(2, characterStats.element);
            target.sceneCharacter.isHit();
        }
    }

    IEnumerator WaitOption()
    {
        database.logMessage.AddMessage("<Your Turn!>");
        database.isSelectedOption = false;
        database.selector = database.allyDetails.IndexOf(characterStats.gameObject);
        battleMenu.Show();
        sceneCharacter.animator.speed = 0;
        sceneCharacter.animator.Play("c" + characterStats.ID.ToString() + "_attack", 0, 0);
        yield return new WaitUntil(() => database.isSelectedOption == true);
        sceneCharacter.animator.Play("c" + characterStats.ID.ToString() + "_idle", 0, 0);
        sceneCharacter.animator.speed = 1;
        database.isSelectedOption = false;
        Character target = getTarget(database.selectedIndex, database.isAllySelected);
        int finalDamage = 0;
        switch (database.selectedState)
        {
            case 0:
                database.logMessage.AddMessage("<Option: Attack!>");
                performAttackPattern(0, -1, database.selectedIndex, false);
                break;
            case 1:
                database.logMessage.AddMessage("<Option: Skill!>");
                Debug.Log("Caster: " + database.selector + ", Skill: " + database.allyDetails[database.selector].GetComponent<Character>().skills[database.selectedItem].ID + ", Is Ally Side: " + database.isAllySelected + ", Target Index: " + database.selectedIndex);
                switch (database.allyDetails[database.selector].GetComponent<Character>().skills[database.selectedItem].ID)
                {
                    case 0:
                        target.AddStatsEffect(2, 0, 100, 0, 0);
                        break;
                    case 1:
                        target.AddStatsEffect(1, 0, 0, -3, 0);
                        target.AddEffect(2, Character.Element.wind);
                        break;
                    case 2:
                        finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        finalDamage += (int)(finalDamage * 0.15f);
                        target.currentHP -= finalDamage;
                        target.AddEffect(2, Character.Element.fire);
                        break;
                    case 3:
                        finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        finalDamage += (int)(finalDamage * 0.05f);
                        for (int i = 0; i < database.enemyDetails.Count; i++)
                        {
                            database.enemyDetails[i].GetComponent<Character>().currentHP -= finalDamage;
                            database.enemyDetails[i].GetComponent<Character>().AddEffect(2, Character.Element.fire);
                        }
                        break;
                    case 4:
                        target.currentHP = target.maxHP;
                        break;
                    case 5:
                        finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        finalDamage = (int)(finalDamage * 0.7f);
                        for (int i = 0; i < database.enemyDetails.Count; i++)
                        {
                            database.enemyDetails[i].GetComponent<Character>().currentHP -= finalDamage;
                            database.enemyDetails[i].GetComponent<Character>().AddEffect(2, Character.Element.water);
                        }
                        break;
                    case 6:
                        target.shieldPoint += 50;
                        break;
                    case 7:
                        for (int i = 0; i < database.allyDetails.Count; i++)
                        {
                            database.allyDetails[i].GetComponent<Character>().shieldPoint += 30;
                        }
                        break;
                    case 8:
                        target.currentHP -= getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        target.AddStatsEffect(2, 0, 0, 6, 0);
                        target.AddEffect(2, Character.Element.electricity);
                        break;
                    case 9:
                        characterStats.currentHP -= (int)(characterStats.currentHP * 0.9f);
                        finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        finalDamage += (int)(finalDamage * 1.5f);
                        target.AddEffect(2, Character.Element.electricity);
                        break;
                }
                break;
            case 2:
                database.logMessage.AddMessage("<Option: Item!>");
                Debug.Log("Caster: " + database.selector + ", Item: " + database.inventory[database.selectedItem].itemName + ", Is Ally Side: " + database.isAllySelected + ", Target Index: " + database.selectedIndex);
                switch (database.inventory[database.selectedItem].ID)
                {
                    case 0:
                        database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(target.gameObject) + " used HP potion!");
                        target.currentHP = getOverThread(target.currentHP, 50, target.maxHP);

                        break;
                    case 1:
                        database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(target.gameObject) + " used MP potion!");
                        target.currentMP = getOverThread(target.currentMP, 30, target.maxHP);
                        break;
                    case 2:
                        database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(target.gameObject) + " used speed potion!");
                        target.AddStatsEffect(3, 0, 0, 3, 0);
                        break;
                    case 3:
                        database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(target.gameObject) + " used strength potion!");
                        target.AddStatsEffect(3, 0, 0, 0, 25);
                        break;
                    case 4:
                        database.logMessage.AddMessage("Ally " + database.allyDetails.IndexOf(target.gameObject) + " used revive potion!");
                        target.isDead = false;
                        target.currentHP = getOverThread(target.currentHP, 50, target.maxHP);
                        target.sceneCharacter.isHit();
                        break;
                }
                database.inventory[database.selectedItem].itemAmount--;
                if (database.inventory[database.selectedItem].itemAmount <= 0)
                {
                    database.inventory.RemoveAt(database.selectedItem);
                }
                break;
        }
        StartCoroutine("attack");
    }
}
