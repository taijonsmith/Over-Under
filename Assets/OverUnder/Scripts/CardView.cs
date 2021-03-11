using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView
{
    public GameObject Card { get; private set; }
    public bool IsFaceUp { get; set; }

    public CardView(GameObject card)
    {
        Card = card;
        IsFaceUp = false;
    }
	
}
