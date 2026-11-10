using UnityEngine;
using Frankencard.CardPartConstants;
using TriInspector;

[CreateAssetMenu(fileName = "New Card Part Data", menuName = "Card Data/Card Part Data", order = 1)]
public class CardPartData : ScriptableObject {
    [Title("Identifiers")]
    [SerializeField] public CardIdentifier PartID = CardIdentifier.NONE; // if is none, this is invalid
    [SerializeField] public string PartName = "";
    [SerializeField, HideInInspector] public string GUID = ""; // if is none, this is invalid

    [PropertySpace(5)] [Title("Position")]
    // whether is the top or bottom of the card
    [SerializeField] public CardPartPosition PartPosition = CardPartPosition.NONE; // if is none, this is invalid
    
    [PropertySpace(5)] [Title("Material"), Required, PreviewObject]
    [SerializeField] public Material PartMaterial = null;

    [PropertySpace(5)] [Title("Cost")] 
    [SerializeField] public int PartCost = 0;

    [PropertySpace(5)] [Title("Ability")]
    [SerializeField] public CardType PartType = CardType.NONE;          // Modifiers are none
    [SerializeField] public int AbilityValue;
    [SerializeField] public string AbilityDesc = "";
}
