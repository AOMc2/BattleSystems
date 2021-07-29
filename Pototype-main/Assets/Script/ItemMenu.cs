using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    public SpriteRenderer sr;
    public BattleMenu battleMenu;
    public ItemMenu itemMenu;
    public Sprite[] itemSprites;
    public bool isDescription;
    public GameObject description1, description2, itemAmount;
    private TMPro.TextMeshProUGUI text;
    public int descriptionID, scroller = 0;

    private void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = (int)transform.position.z * 1;
        if (isDescription == false)
        {
            description1 = Instantiate(gameObject, transform.position, Quaternion.identity);
            description1.GetComponent<ItemMenu>().isDescription = true;
            description1.GetComponent<ItemMenu>().descriptionID = 0;
            description1.name = "Page1";

            description2 = Instantiate(gameObject, transform.position, Quaternion.identity);
            description2.GetComponent<ItemMenu>().isDescription = true;
            description2.GetComponent<ItemMenu>().descriptionID = 1;
            description2.name = "Page2";
        }
        else
        {
            transform.SetParent(GameObject.Find("ItemMenu(Clone)").transform);
            transform.position += new Vector3(0, 0, 1);
            itemMenu = transform.parent.GetComponent<ItemMenu>();

            if (descriptionID  + itemMenu.scroller < battleMenu.database.inventory.Count)
            {
                sr.sprite = itemSprites[battleMenu.database.inventory[descriptionID + itemMenu.scroller].ID];
            }
            else
            {
                Destroy(gameObject);
            }

            if (descriptionID == 0)
            {
                transform.position = (Vector2)transform.position + new Vector2(0, 0.75f);
            }
            else
            {
                transform.position = (Vector2)transform.position + new Vector2(0, -0.75f);
                sr.color = new Color32(170, 70, 200, 255);
            }

            text = Instantiate(itemAmount, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>();
            text.transform.SetParent(transform);
            text.transform.position = (Vector2)transform.position + new Vector2(3.25f, 0.5f);

            text.text = "x" + battleMenu.database.inventory[itemMenu.scroller + descriptionID].itemAmount;
        }
    }

    private void Update()
    {
        if (isDescription == true)
        {
            int temp = itemMenu.scroller + descriptionID;
            if (temp >= battleMenu.database.inventory.Count)
            {
                text.text = "";
                sr.sprite = null;
            }
            else
            {
                text.text = "x" + battleMenu.database.inventory[temp].itemAmount;
                sr.sprite = itemSprites[battleMenu.database.inventory[temp].ID];
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                battleMenu.isSelectedOption = false;
                Destroy(gameObject);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (scroller + 2 <= battleMenu.database.inventory.Count)
                {
                    scroller++;
                }
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (scroller - 1 >= 0)
                {
                    scroller--;
                }
            }
            if (Input.GetKeyDown(KeyCode.Z) && battleMenu.database.inventory.Count != 0)
            {
                battleMenu.database.inventory[scroller].itemAmount--;
                battleMenu.currentItem = scroller;
                battleMenu.isSelectedItem = true;
                Destroy(gameObject);
            }
        }
    }
}
