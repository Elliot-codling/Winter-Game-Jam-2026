using System;
using System.Collections.Generic;
using System.Linq;
using Frankencard.CardPartConstants;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

// map GUIDs to card parts and cards
[DeclareBoxGroup("MainBox", Title = "Data Box")]
[DeclareHorizontalGroup("MainBox/Data")]

public class Inventory : MonoBehaviour {
    [SerializeField, ShowInInspector, ReadOnly, Group("MainBox/Data")] private SerializedDictionary<string, CardPartData> HeldCardParts = new();
    [SerializeField, ShowInInspector, ReadOnly, Group("MainBox/Data")] private SerializedDictionary<string, CardData> HeldCardDeck = new();

    [HideInInspector] public UnityEvent<string> CardAdded;
    [HideInInspector] public UnityEvent<string> CardRemoved;
    
    [HideInInspector] public UnityEvent<string> CardPartAdded;
    [HideInInspector] public UnityEvent<string> CardPartRemoved;
    
    [Button]
    private void ScrapRightmostCard() {
        ScrapCard(HeldCardDeck.Values.ToArray()[HeldCardDeck.Count - 1].GUID);
    }
    public void AddCardPartInventory(CardIdentifier _identifier, CardPartPosition _position) {
        // create card part using identifier and position
        CardData data = GameObject.FindAnyObjectByType<GameManager>().CardDataConstants.GetDataFromID(_identifier);
        CardPartData cardPartData = data.GetCardPartDataFromPosition(_position);
        cardPartData.GUID = System.Guid.NewGuid().ToString(); // create new GUID for card part
        
        HeldCardParts.Add(cardPartData.GUID, cardPartData);
        
        CardPartAdded.Invoke(cardPartData.GUID);
    }

    public void AddCardToDeck(CardPartData _dataTop, CardPartData _dataBottom) {
        // form card class from the two card parts
        CardData newData = Instantiate(ScriptableObject.CreateInstance<CardData>());

        // set data
        newData.CardTopData = _dataTop;
        newData.CardBottomData = _dataBottom;
        newData.CardID = _dataTop.PartID == _dataBottom.PartID ? _dataTop.PartID : CardIdentifier.MERGED;
        newData.name = newData.CardID.ToString();
        newData.GUID = System.Guid.NewGuid().ToString(); // create new GUID for the card

        HeldCardDeck.Add(newData.GUID, newData);
        
        // Invoke card added event for external listeners
        CardAdded.Invoke(newData.GUID);
    }

    public void RemoveCardFromDeck(string _guid) {
        // Remove card from inventory
        HeldCardDeck.Remove(_guid);
        
        // Invoke card removed event for external listeners
        CardRemoved.Invoke(_guid);
    }

    public void RemoveCardPartFromInventory(string _guid) {
        // Remove card part from inventory
        HeldCardParts.Remove(_guid);
        
        // Invoke card part removed event for external listeners
        CardPartRemoved.Invoke(_guid);
    }

    public void ScrapCard(string _guid) {
        if (!HeldCardDeck.TryGetValue(_guid, out CardData data)) {
            Debug.Log("Could not find card with GUID: " + _guid);
            return;
        }
        
        // add top part to inventory
        AddCardPartInventory(data.CardTopData.PartID, data.CardTopData.PartPosition);
        // add bottom part to inventory
        AddCardPartInventory(data.CardBottomData.PartID, data.CardBottomData.PartPosition);
        
        RemoveCardFromDeck(_guid);
    }

    // This data is editable and not readonly, I do not have the time to fix this
    public CardData GetCardData(string _guid) {
        HeldCardDeck.TryGetValue(_guid, out CardData data);

        return data;

    }
    
    // This data is editable and not readonly, I do not have the time to fix this
    public CardPartData GetCardPartData(string _guid) {
        HeldCardParts.TryGetValue(_guid, out CardPartData data);

        return data;

    }

    public void TryMergeCards(string _guid0, string _guid1) {
        Debug.Log("Merging Cards");
    
        HeldCardParts.TryGetValue(_guid0, out CardPartData data0);
        HeldCardParts.TryGetValue(_guid1, out CardPartData data1);

        if (data0.PartPosition == data1.PartPosition) {
            Debug.Log("Could not combine, both same positions");

            return;
        }

        CardData newData = Instantiate(ScriptableObject.CreateInstance<CardData>());
        if (data0.PartPosition == CardPartPosition.TOP) {
            AddCardToDeck(data0, data1);
        }
        else {
            AddCardToDeck(data1, data0);
        }
        
        // this is terrible, but temporary for visual
        RemoveCardPartFromInventory(data0.GUID);
        RemoveCardPartFromInventory(data1.GUID);
    }

    public void RemoveAllCardParts() {
        // Remove backwards for safety
        for (int i = HeldCardParts.Count - 1; i >= 0; i--) {
            RemoveCardPartFromInventory(HeldCardParts.Keys.ElementAt(i));
        }
    }
    
    public int GetNumberOfHeldCards() {
        return HeldCardDeck.Count;
    }

    public List<string> GetHeldCardGUIDs() {
        return HeldCardDeck.Keys.ToList();
    }
}
