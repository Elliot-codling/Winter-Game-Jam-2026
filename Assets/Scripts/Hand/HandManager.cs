using System;
using System.Collections.Generic;
using System.Linq;
using Frankencard.CardPartConstants;
using Frankencard.Utils;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

using DG.Tweening;

public class HandManager : MonoBehaviour {
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private GameObject CardPartPrefab;

    [Title("Card Hand Vars")]
    [SerializeField,FormerlySerializedAs("Curve")] private AnimationCurve XPositionCurve;
    [SerializeField] private AnimationCurve YPositionCurve;
    [SerializeField] private AnimationCurve RotationCurve;
    private Dictionary<string, GameObject> HandCards = new(); // visuals
    [SerializeField] private int MaxCards = 10;
    
    [Title("Card Parts Hand Vars")]
    [SerializeField] private AnimationCurve XPositionCurve_CardParts;
    private Dictionary<string, GameObject> HandCardParts = new(); // visuals
    [SerializeField] private int MaxCardParts = 14;
    [SerializeField] private Transform OnTablePartsTrans;
    
    private GameManager GManager;
    private GameObject CurrentSelectedCardPart;
    
    [Title("Interactions")]
    [SerializeField] private InputActionReference ClickAction; 
    [SerializeField] private InputActionReference RightClickAction; 
    
    [Title("Selected Cards")]
    [SerializeField, ShowInInspector, ReadOnly] private SerializableDictionary<string, GameObject> CurrentSelectedCards = new();

    [HideInInspector] public UnityEvent<CardManager.PlayedCards> CardsSubmitted = new UnityEvent<CardManager.PlayedCards>();

    [SerializeField] private Material FullBackOfCardMaterial;
    [SerializeField] private Material TopBackOfCardMaterial;
    [SerializeField] private Material BottomBackOfCardMaterial;

    [SerializeField, ShowInInspector, ReadOnly] private SerializableDictionary<string, Vector3> CardFanDefaultPositions = new();

    private readonly int MaxSubmitCost = 10;
    private int CurrentSubmitCost = 0;

    [SerializeField] private CardDescriptionUIDataContainer UIContainer;

    [SerializeField] private Transform InitialCardSpawnLocation;

    private void Awake() {
        GManager = FindAnyObjectByType<GameManager>();
        
        UpdateCardPosition();
        
        // Call add card
        GManager.PlayerInventory.CardAdded.AddListener((x) => {
            CardData data = GManager.PlayerInventory.GetCardData(x);
            AddVisualCard(data.CardTopData, data.CardBottomData, x);
        });
        
        // Call remove card
        GManager.PlayerInventory.CardRemoved.AddListener(RemoveVisualCard);
        
        // Call add card part
        GManager.PlayerInventory.CardPartAdded.AddListener((x) => {
            CardPartData data = GManager.PlayerInventory.GetCardPartData(x);
            AddVisualCardPart(data, x);
        });
        
        // Call remove card part
        GManager.PlayerInventory.CardPartRemoved.AddListener(RemoveVisualCardPart);

        // Set initial cost text
        UIContainer.ChangeHandCostText(CurrentSubmitCost.ToString());

        // Interaction input actions
        ClickAction.action.Enable();
        RightClickAction.action.Enable();
    }

    private void Update() {
        // If we can't interact with hand, return so none of the click logic runs
        if (!GManager.AcceptPlayerInteraction) {
            return;
        }
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        // Scrap card on right click logic
        if (RightClickAction.action.WasPressedThisFrame()) {
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.CompareTag("Card")) {
                    Debug.Log("Hit card with GUID: " + hit.transform.name);
                    if (CurrentSelectedCards.ContainsKey(hit.transform.name)) {
                        CurrentSelectedCards.Remove(hit.transform.name);

                        CardData data = GManager.PlayerInventory.GetCardData(hit.transform.name);
                        
                        // Remove the cost
                        CurrentSubmitCost -= data.CardTopData.PartCost + data.CardBottomData.PartCost;
                        
                        UIContainer.ChangeHandCostText(CurrentSubmitCost.ToString());
                        
                        // Disable outline
                        hit.transform.gameObject.GetComponent<Outline>().enabled = false;
                    }
                    
                    GManager.PlayerInventory.ScrapCard(hit.transform.name);
                }
            }

            if (CurrentSelectedCardPart != null) {
                // Disable outline
                CurrentSelectedCardPart.GetComponent<Outline>().enabled = false;
                CurrentSelectedCardPart = null;
            }
        }

        // Left click action
        if (ClickAction.action.WasPressedThisFrame()) {
            if (Physics.Raycast(ray, out hit)) {
                // Card selection logic for playing cards
                if (hit.transform.CompareTag("Card")) {
                    // Get default position
                    CardFanDefaultPositions.TryGetValue(hit.transform.name, out Vector3 destination);
                        
                    // Check whether key exists and remove if it does
                    if (CurrentSelectedCards.ContainsKey(hit.transform.name)) {
                        CardData data = GManager.PlayerInventory.GetCardData(hit.transform.name);
                        
                        // Remove cost
                        CurrentSubmitCost -= data.CardTopData.PartCost + data.CardBottomData.PartCost;
                        UIContainer.ChangeHandCostText(CurrentSubmitCost.ToString());
                        
                        CurrentSelectedCards.Remove(hit.transform.name);
                        
                        //hit.transform.DOLocalMove(destination, 0.2f);    

                        // Disable outline
                        hit.transform.gameObject.GetComponent<Outline>().enabled = false;
                        return;
                    }
                    CardData data0 = GManager.PlayerInventory.GetCardData(hit.transform.name);

                    int costOfCard = data0.CardTopData.PartCost + data0.CardBottomData.PartCost;

                    if (CurrentSubmitCost + costOfCard > MaxSubmitCost) {
                        Debug.Log("Cost too much");
                        return;
                    }
                    
                    CurrentSubmitCost += costOfCard;
                    UIContainer.ChangeHandCostText(CurrentSubmitCost.ToString());
                    
                    //hit.transform.DOLocalMove(destination + (-hit.transform.right * 0.1f), 0.2f);    
                    
                    CurrentSelectedCards.Add(hit.transform.name, hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<Outline>().enabled = true;
                }
                
                if (hit.transform.CompareTag("CardPart")) {
                    if (CurrentSelectedCardPart != null) {
                        // Perform merge here
                        GManager.PlayerInventory.TryMergeCards(CurrentSelectedCardPart.name, hit.transform.name);

                        // Disable outline
                        CurrentSelectedCardPart.GetComponent<Outline>().enabled = false;
                        CurrentSelectedCardPart = null;
                    }

                    Debug.Log("Hit card part with GUID: " + hit.transform.name);
                    // enable outline
                    hit.transform.gameObject.GetComponent<Outline>().enabled = true;

                    CurrentSelectedCardPart = hit.transform.gameObject;
                }
                else {
                    if (CurrentSelectedCardPart != null) {
                        // disable outline
                        CurrentSelectedCardPart.GetComponent<Outline>().enabled = false;
                        CurrentSelectedCardPart = null;
                    }
                }

                if (hit.transform.CompareTag("Button")) {
                    CardManager.PlayedCards CardsToSubmit = new();
                    // intialise list
                    CardsToSubmit.m_selectedCards = new();
                    
                    foreach (var value in CurrentSelectedCards.Keys.ToArray()) {
                        CardsToSubmit.m_selectedCards.Add(GManager.PlayerInventory.GetCardData(value));
                        GManager.PlayerInventory.RemoveCardFromDeck(value);
                    }
                    
                    // Clear selected cards
                    CurrentSelectedCards.Clear();
                    
                    // Remove all card parts
                    GManager.PlayerInventory.RemoveAllCardParts();
                    
                    // Reset cost
                    CurrentSubmitCost = 0;
                    // Update UI
                    UIContainer.ChangeHandCostText(CurrentSubmitCost.ToString());
                    
                    // Invoke submit event
                    CardsSubmitted?.Invoke(CardsToSubmit);
                    
                    Debug.Log("Submitted cards // Amount: " + CardsToSubmit.m_selectedCards.Count + " // Reset cost to: " + CurrentSubmitCost);
                }
            }
        }
    }
    
    private void AddVisualCard(CardPartData _topData, CardPartData _bottomData, string _guid) {
        if (HandCards.Count >= MaxCards) {
            return;
        }

        GameObject newGO = Instantiate(CardPrefab, this.gameObject.transform);
        newGO.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = _topData.PartMaterial; // set top visual texture
        newGO.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material = _bottomData.PartMaterial; // set bottom visual texture
        newGO.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material = FullBackOfCardMaterial; // set bottom visual texture
        newGO.name = _guid; // set name to GUID for easy access for now
        newGO.GetComponent<Outline>().enabled = false;

        // Add card to dictionary
        HandCards.Add(_guid, newGO);

        // Set position and rotation to initial position (I want to go home)
        newGO.transform.position = InitialCardSpawnLocation.position;
        newGO.transform.rotation = InitialCardSpawnLocation.rotation;
        
        // Update positions
        UpdateCardPosition();
    }
    
    private void RemoveVisualCard(string _guid) {
        if (HandCards.Count <= 0) {
            return;
        }

        HandCards.TryGetValue(_guid, out GameObject go);
        
        // Destroy GameObject
        Destroy(go);
        
        // Remove card from dictionary
        HandCards.Remove(_guid);
        
        // Update positions
        UpdateCardPosition();
    }
    
    private void UpdateCardPosition() {
        if (HandCards.Count == 0) {
            return;
        }
        
        CardFanDefaultPositions.Clear();

        float ratioOffset = 1 / ((float)HandCards.Count + 1);

        for (int i = 1; i <= HandCards.Count; i++) {
            float handRatio = ratioOffset * i;
            if (handRatio <= 0) {
                handRatio = 0.5f;
            }
            
            // Calculate X Position Spread
            Vector3 destination = this.transform.position;
            destination.x += XPositionCurve.Evaluate(handRatio);
            // Calculate Y Position
            destination.y += YPositionCurve.Evaluate(handRatio);
            destination.z -= 0.00001f * i;
            // Set Position
            //HandCards.Values.ToArray()[i - 1].transform.position = destination;
            
            // START THE MOVE
            HandCards.Values.ToArray()[i - 1].transform.DOMove(destination, 0.5f).OnComplete(() => Debug.Log("Completed"));
            
            // Rotation
            float z = RotationCurve.Evaluate(handRatio) - 90;
            //HandCards.Values.ToArray()[i - 1].transform.localRotation = Quaternion.Euler(0, 0, z);
            HandCards.Values.ToArray()[i - 1].transform.DORotate(new Vector3(0, 0, z), 0.4f).OnComplete(() => Debug.Log("Completed Rotation"));
            
            //CardFanDefaultPositions.Add(HandCards.Keys.ToArray()[i - 1], destination - gameObject.transform.position);
        }
    }
    
    ///
    /// CARD PARTS 
    ///
    
    private void UpdateCardPartsPosition() {
        if (HandCardParts.Count == 0) {
            return;
        }

        float ratioOffset = 1 / ((float)HandCardParts.Count + 1);

        for (int i = 1; i <= HandCardParts.Count; i++) {
            float handRatio = ratioOffset * i;
            if (handRatio <= 0) {
                handRatio = 0.5f;
            }
            
            // Calculate X Position Spread
            Vector3 destination = OnTablePartsTrans.position;
            destination.x += XPositionCurve.Evaluate(handRatio);
            destination.y -= 0.00001f * i;
            HandCardParts.Values.ToArray()[i - 1].transform.position = destination;
            
            // Rotation
            float z = RotationCurve.Evaluate(handRatio) - 90;
            HandCardParts.Values.ToArray()[i - 1].transform.localRotation = Quaternion.Euler(90, 0, -90);
        }
    }
    
    private void AddVisualCardPart(CardPartData _data, string _guid) {
        if (HandCards.Count >= MaxCards) {
            return;
        }

        GameObject newGO = Instantiate(CardPartPrefab, this.gameObject.transform);
        newGO.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = _data.PartMaterial; // set front visual texture

        switch (_data.PartPosition) {
            case CardPartPosition.TOP:
                newGO.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material = TopBackOfCardMaterial; // set top visual texture
                break;
            case CardPartPosition.BOTTOM:
                newGO.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material = BottomBackOfCardMaterial; // set bottom visual texture
                break;
            default:
                break;
        }
        
        newGO.name = _guid; // set name to GUID for easy access for now
        newGO.GetComponent<Outline>().enabled = false; // disable outline immediately
        
        // Add card to dictionary
        HandCardParts.Add(_guid, newGO);
        
        // Update positions
        UpdateCardPartsPosition();
    }
    
    private void RemoveVisualCardPart(string _guid) {
        if (HandCardParts.Count <= 0) {
            return;
        }
        
        HandCardParts.TryGetValue(_guid, out GameObject go);
        
        // Destroy GameObject
        Destroy(go);
        
        // Remove card from dictionary
        HandCardParts.Remove(_guid);
        
        // Update positions
        UpdateCardPartsPosition();
    }
}
