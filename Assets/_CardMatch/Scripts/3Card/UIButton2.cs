using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIButton2 : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;
    //[SerializeField] private string targetMessage;


    public Color highlightColor = Color.cyan;

    public void OnMouseEnter()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = highlightColor;
        }
    }
    public void OnMouseExit()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.white;
        }
    }

    public void OnMouseDown()
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }
    public void OnMouseUp()
    {
        transform.localScale = Vector3.one;
        if (targetObject != null)
        {
            Messenger.Broadcast(GameEvent.RESTART);
            //targetObject.SendMessage(targetMessage);
        }
    }
}
