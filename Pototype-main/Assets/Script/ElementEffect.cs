using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementEffect : MonoBehaviour
{
    public int roundRemainings;
    public GameObject textHolder, textPrefab;
    public Character.Element element;
    public Character characterStats;
    public Sprite[] elementImages;
    public SpriteRenderer sr;
    public TMPro.TextMeshProUGUI text;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        textHolder = Instantiate(textPrefab, transform.position, Quaternion.identity);
        textHolder.transform.SetParent(gameObject.transform);
        textHolder.transform.position += new Vector3(0.5f, -0.5f, 0);
        text = textHolder.GetComponent<TMPro.TextMeshProUGUI>();
        text.text = roundRemainings.ToString();
    }

    public void Set(int round, Character characterStats, Character.Element element)
    {
        roundRemainings = round;
        this.characterStats = characterStats;
        this.element = element;
        sr.sprite = elementImages[GetSpriteIndex(element)];
        if(element == Character.Element.none)
        {
            characterStats.specialEffects.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private int GetSpriteIndex(Character.Element element)
    {
        switch (element)
        {
            case Character.Element.earth:
                return 0;
            case Character.Element.electricity:
                return 1;
            case Character.Element.fire:
                return 2;
            case Character.Element.ice:
                return 3;
            case Character.Element.rock:
                return 4;
            case Character.Element.water:
                return 5;
            case Character.Element.wildFire:
                return 6;
            case Character.Element.wind:
                return 7;
            default:
                return 0;
        }
    }

    private void LateUpdate()
    {
        text.text = roundRemainings.ToString();
    }
}
