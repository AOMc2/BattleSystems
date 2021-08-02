using UnityEngine;
using System.Collections;


public class SortingOrderScript : MonoBehaviour
{
    public const string LAYER_NAME = "Mapbackground";
    public int sortingOrder = 0;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        sprite.sortingOrder = (int)transform.position.z * -1;
        sprite.sortingOrder = sortingOrder;
        sprite.sortingLayerName = LAYER_NAME;
    }
}