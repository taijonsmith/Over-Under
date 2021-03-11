using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour 
{
    //public GameObject ShuffleCards; 
    public ShuffleCards ShuffleCards; 
    // On Button Shuffle click
    public void OnButtomShuffle()
    {
        ShuffleCards.Shuffle();
    }
}
