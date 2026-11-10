using System;
using Frankencard.CardPartConstants;
using Frankencard.Utils;
using TriInspector;
using UnityEditor;
using UnityEngine;

// This is a mess, I am aware, leave me alone :(

namespace Frankencard.CardConstants {
    // TODO: Make this a singleton
    [DeclareBoxGroup("AddBox", Title = "Add To Dictionary")]
    [DeclareHorizontalGroup("AddBox/Fields")]
    
    [DeclareBoxGroup("RemoveBox", Title = "Remove From Dictionary")]
    [DeclareHorizontalGroup("RemoveBox/Fields")]
    
    [CreateAssetMenu(fileName = "New Card Constants", menuName = "Card Data/Card Constants"), Serializable]
    public class CardConstants : ScriptableObject {
        // Field for adding to dictionary
        [Group("AddBox/Fields")]public CardIdentifier AddCardIDField;
        [Group("AddBox/Fields")]public CardData AddCardDataField;
        [Button, PropertyOrder(2), PropertySpace(SpaceAfter = 20)]
        public void Add() {
            if (AddCardDataField == null) {
                Debug.LogError("Card Data Field is missing");
                return;
            }
            
            CardDataContainer.Add(AddCardIDField, AddCardDataField);

            AddCardIDField = CardIdentifier.NONE;
            AddCardDataField = null;
        }
        
        [Group("RemoveBox/Fields")] public CardIdentifier RemoveCardIDField = CardIdentifier.NONE;
        [Button, PropertyOrder(3)]
        public void Remove() {
            CardDataContainer.Remove(RemoveCardIDField);

            RemoveCardIDField = CardIdentifier.NONE;
        }
        
        [PropertySpace(25)]
        [SerializeField, ShowInInspector, ReadOnly] private SerializableDictionary<CardIdentifier, CardData> CardDataContainer = new();
        
        
        public CardData GetDataFromID(CardIdentifier _identifier) {
            CardDataContainer.TryGetValue(_identifier, out CardData data);
            
            return data;
        }
    }
}