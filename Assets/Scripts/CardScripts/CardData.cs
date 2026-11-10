using System;
using UnityEngine;
using Frankencard.CardPartConstants;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Card Data/Full Card Data"), Serializable]
public class CardData : ScriptableObject {
    [SerializeField] public CardIdentifier CardID = CardIdentifier.NONE;
    [SerializeField] public string CardName = "";
    [SerializeField, HideInInspector] public string GUID = "";
    
    [SerializeField] public CardPartData CardTopData = null;
    [SerializeField] public CardPartData CardBottomData = null;


    public CardPartData GetCardPartDataFromPosition(CardPartPosition _position) {
        switch (_position) {
            case CardPartPosition.TOP: return Instantiate(CardTopData);
            case CardPartPosition.BOTTOM: return Instantiate(CardBottomData);
        }

        return Instantiate(CardTopData);
    }
}
