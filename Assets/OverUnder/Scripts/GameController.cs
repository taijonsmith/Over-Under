using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int dealersFirstCard = -1;
    int dealerwincount = 0;
    int playerwincount = 0;
    string choice = "";

    public CardStack player;
    public CardStack dealer;
    public CardStack deck;

    public Button sameButton;
    public Button hitButton;
    public Button stayButton;
    public Button playAgainButton;
    public Button quitButton;

    public Text winnerText;
    public Text playerScoreText;
    public Text dealerScoreText;
    public Text playerWinsText;
    public Text dealerWinsText;

    public void Same()
    {
        choice = "same";
        sameButton.interactable = false;
        hitButton.interactable = false;
        stayButton.interactable = false;

        StartCoroutine(DealersTurn());
    }


    public void Over()
    {
        choice = "over";
        sameButton.interactable = false;
        hitButton.interactable = false;
        stayButton.interactable = false;

        StartCoroutine(DealersTurn());
    }

    public void Under()
    {
        choice = "under";
        sameButton.interactable = false;
        hitButton.interactable = false;
        stayButton.interactable = false;

        StartCoroutine(DealersTurn());
    }

    public void PlayAgain()
    {
        playAgainButton.interactable = false;

        player.GetComponent<CardStackView>().Clear();
        dealer.GetComponent<CardStackView>().Clear();
        deck.GetComponent<CardStackView>().Clear();
        deck.CreateDeck();
        sameButton.interactable = true;
        hitButton.interactable = true;
        stayButton.interactable = true;
        dealersFirstCard = -1;
        winnerText.text = "";
        StartGame();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    IEnumerator DealersTurn()
    {
        sameButton.interactable = false;
        hitButton.interactable = false;
        stayButton.interactable = false;

        CardStackView view = dealer.GetComponent<CardStackView>();
        view.Toggle(dealersFirstCard, true);
        view.ShowCards();
        dealerScoreText.text = "House Score: " + dealer.HandValue().ToString();
        yield return new WaitForSeconds(1f);

        if (player.HandValue() > dealer.HandValue())
        {
            if (choice == "over") //if your hand is larger and you choose "over", you win
            {
                winnerText.text = "CORRECT!!";
                playerwincount++;
            }
            else
            {
                winnerText.text = "WRONG!!";
                dealerwincount++;
            }
        }
        else if (player.HandValue() < dealer.HandValue()) //if your hand is smaller and you choose "under", you win
        {
            if (choice == "under")
            {
                winnerText.text = "CORRECT!!";
                playerwincount++;
            }
            else
            {
                winnerText.text = "WRONG!!";
                dealerwincount++;
            }
        }
        else if (player.HandValue() == dealer.HandValue()) //if your hand is the same as the dealer and you choose "same", you win
        {
            if (choice == "same")
            {
                winnerText.text = "CORRECT!!";
                playerwincount++;
            }
            else
            {
                winnerText.text = "WRONG!!";
                dealerwincount++;
            }
        }

        else
        {
            winnerText.text = "DEALER WINS!";
        }
        yield return new WaitForSeconds(1f);
        playAgainButton.interactable = true;
        playerWinsText.text = "Your Wins: " + playerwincount.ToString();
        dealerWinsText.text = "House Wins: " + dealerwincount.ToString();
    }

    void Start()
    {
        StartGame(); 
    }

    void StartGame()
    {
        playerScoreText.text = "Your Score: 0";
        dealerScoreText.text = "House Score: ";
        for (int i=0; i<2; i++)
        {
            player.Push(deck.Pop());
            HitDealer();
        }
        playerScoreText.text = "Your Score: " + player.HandValue().ToString();
    }

    void HitDealer()
    {
        int card = deck.Pop();
        if(dealersFirstCard < 0)
        {
            dealersFirstCard = card;
        }

        dealer.Push(card);

        if(dealer.CardCount >= 2)
        {
            CardStackView view = dealer.GetComponent<CardStackView>();
            view.Toggle(card, true);
        }
    }



}
