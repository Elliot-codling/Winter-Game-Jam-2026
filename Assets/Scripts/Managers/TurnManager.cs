using System;
using Frankencard.CardPartConstants;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour {
    [HideInInspector] public UnityEvent TurnStart;
    [HideInInspector] public UnityEvent TurnEnd;

    private GameState CurrentGameState = GameState.NONE;

    private GameManager GManager;
    
    [SerializeField] private HandManager PlayerHandManager;

    private int MaxCardsToDrawTo = 5;

    [SerializeField] private CardManager GCardManager;

    [SerializeField] private Light ButtonLampLight;
    
    public enum GameState {
        NONE,
        PICKING,
        EXECUTION
    }
    
    private void Awake() {
        GManager = FindAnyObjectByType<GameManager>();
        
        PlayerHandManager.CardsSubmitted.AddListener(x => EndTurn());
        
        // When cards have finished executing, start next turn
        GCardManager.FinishedRoundExecution.AddListener(NextTurn);
    }

    private void Start() {
        StartGame();
    }

    private void StartGame() {
        // Enable light
        ButtonLampLight.intensity = 1.0f;
        
        // Draw up to the maximum number of cards
        for (int i = 0; i < MaxCardsToDrawTo; i++) {
            PlayerDrawCard();
            
            EnemyDrawCard();
        }

        // Set game state to picking, player and enemy are picking cards
        CurrentGameState = GameState.PICKING;
        
        GManager.AcceptPlayerInteraction = true;
    }
    
    private void NextTurn() {
        
        // Enable light
        ButtonLampLight.intensity = 1.0f;
        
        int playerCardsToDraw = MaxCardsToDrawTo - GManager.PlayerInventory.GetNumberOfHeldCards();
        int enemyCardsToDraw = MaxCardsToDrawTo - GManager.EnemyInventory.GetNumberOfHeldCards();
        
        // Draw extra cards here
        
        // Draw cards for player back up to maximum
        for (int i = 0; i < playerCardsToDraw; i++) {
            PlayerDrawCard();
        }

        // Draw cards for enemy back up to maximum
        for (int i = 0; i < enemyCardsToDraw; i++) {
            EnemyDrawCard();
        }
        
        GManager.AcceptPlayerInteraction = true;
    }

    private void EndTurn() {
        // Disable light
        ButtonLampLight.intensity = 0.0f;

        GManager.AcceptPlayerInteraction = false;
    }

    private void PlayerDrawCard() {
        // Randomise what card is drawn
        CardIdentifier cardID = (CardIdentifier)Random.Range(2, (int)CardIdentifier.ID_MAX);

        // Get card data from generated ID
        CardData dataToAdd = GManager.CardDataConstants.GetDataFromID(cardID);
            
        // Add data to the player deck
        GManager.PlayerInventory.AddCardToDeck(dataToAdd.CardTopData, dataToAdd.CardBottomData);
    }

    private void EnemyDrawCard() {
        // Randomise what card is drawn
        CardIdentifier cardID = (CardIdentifier)Random.Range(2, (int)CardIdentifier.ID_MAX);

        // Get card data from generated ID
        CardData dataToAdd = GManager.CardDataConstants.GetDataFromID(cardID);
            
        // Add data to the player deck
        GManager.EnemyInventory.AddCardToDeck(dataToAdd.CardTopData, dataToAdd.CardBottomData);
    }
}

