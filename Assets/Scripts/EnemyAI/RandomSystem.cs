using Frankencard.CardPartConstants;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CardManager;
using Random = UnityEngine.Random;

public class RandomSystem : MonoBehaviour
{
    private List<CardData> franksList;
    private Inventory enemyInventory;

    public UnityEvent<CardManager.PlayedCards> FranksCardsSubmitted = new();
    [SerializeField] private HandManager m_handManager;
    private GameManager GManager;

    void Start()
    {
        GManager = FindAnyObjectByType<GameManager>();
        m_handManager.CardsSubmitted.AddListener(FranksCards);
    }

    private void FranksCards(PlayedCards _playedCards)
    {
        CardManager.PlayedCards CardsToSubmit = new();

        // Get his hand, then tell card mgr that we have it
        CardsToSubmit.m_selectedCards = new();

        int randomNumberOfCardsToDraw = Random.Range(1, 3);
        var listOfGUIDs = GManager.EnemyInventory.GetHeldCardGUIDs();
        for (int index = 0; index < randomNumberOfCardsToDraw; index++)
        {
            CardsToSubmit.m_selectedCards.Add(GManager.EnemyInventory.GetCardData(listOfGUIDs[index]));
            GManager.EnemyInventory.RemoveCardFromDeck(listOfGUIDs[index]);
        }

        // Remove all card parts
        GManager.EnemyInventory.RemoveAllCardParts();

        // Invoke submit event
        Debug.Log("Franks cards submitted");
        FranksCardsSubmitted?.Invoke(CardsToSubmit);
    }

}
