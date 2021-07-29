using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHP, currentHP, maxMP, currentMP, defense, dodgeRate, speed, attackDamage, ID;
    public enum Element {none, fire, water, wind, earth, electricity, ice, rock, wildFire};
    public Element element;
    public bool isAlly, isDead = false;
    public SceneCharacter sceneCharacter;
    public List<Skill> skills;
    public Sprite[] elementImages;
    public List<GameObject> specialEffects;
    public GameObject elementEffectImage;

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

    public void AddSkill(Skill skill)
    {
        skills.Add(skill);
    }

    public void AddElementEffect(int round, Character characterStats, Element element)
    {
        Debug.Log(element);
        Element replaceElement = Element.none;
        bool isMixable = false, isRepeated = false;
        int wantedIndex = 0, repeatedIndex = 0;
        if (element != Element.none)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ElementEffect currentElement = specialEffects[i].GetComponent<ElementEffect>();
                Element tempElement = isElementMixable(element, currentElement.element);
                if (tempElement != Element.none && isMixable == false)
                {
                    wantedIndex = i;
                    isMixable = true;
                    replaceElement = tempElement;
                }
                if(element == currentElement.element && isRepeated == false)
                {
                    repeatedIndex = i;
                    isRepeated = true;
                }
            }
            GameObject newElement;
            if (isMixable == false)
            {
                if (isRepeated == false)
                {
                    newElement = Instantiate(elementEffectImage, sceneCharacter.transform.position, Quaternion.Euler(Vector3.zero)); ;
                    newElement.GetComponent<ElementEffect>().Set(round, characterStats, element);
                    newElement.transform.position += new Vector3(1, -1 + specialEffects.Count, 0);
                    newElement.transform.SetParent(transform);
                    specialEffects.Add(newElement);
                }
                else
                {
                    specialEffects[repeatedIndex].GetComponent<ElementEffect>().roundRemainings += round;
                }
            }
            else
            {
                for(int i = wantedIndex; i < transform.childCount; i++)
                {
                    transform.GetChild(i).position -= new Vector3(0, 1, 0);
                }


                ElementEffect deleteElement = specialEffects[wantedIndex].GetComponent<ElementEffect>();
                specialEffects.Remove(deleteElement.gameObject);
                Destroy(deleteElement.gameObject);
                newElement = Instantiate(elementEffectImage, sceneCharacter.transform.position, Quaternion.Euler(Vector3.zero)); ;
                newElement.GetComponent<ElementEffect>().Set(round, characterStats, replaceElement);
                newElement.transform.position += new Vector3(1, -1 + specialEffects.Count, 0);
                newElement.transform.SetParent(transform);
                specialEffects.Add(newElement);
            }
        }
    }

    private Element isElementMixable(Element newElement, Element currentElement)
    {
        if (newElement == Element.fire && currentElement == Element.wind || newElement == Element.wind && currentElement == Element.fire)
            return Element.wildFire;
        if (newElement == Element.water && currentElement == Element.wind || newElement == Element.wind && currentElement == Element.water)
            return Element.ice;
        if (newElement == Element.earth && currentElement == Element.fire || newElement == Element.fire && currentElement == Element.earth)
            return Element.rock;
        return Element.none;
    }
}
