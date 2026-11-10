using TMPro;
using UnityEngine;
using TriInspector;

public class CardDescriptionUIDataContainer : MonoBehaviour {
    [Title("Full Card UI Data Container")]
    [SerializeField] public GameObject FullCardUIOwner;
    [Space(5)]
    [SerializeField] public TextMeshProUGUI TopAbilityTitleText;
    [SerializeField] public TextMeshProUGUI TopAbilityDescText;
    [Space(5)]
    [SerializeField] public TextMeshProUGUI BottomAbilityTitleText;
    [SerializeField] public TextMeshProUGUI BottomAbilityDescText;
    [Space(5)]
    [SerializeField] public TextMeshProUGUI TotalCostText;
    [Space (10)]
    [Title("Card Part UI Data Container")]
    [SerializeField] public GameObject CardPartAbilityOwner;
    [SerializeField] public TextMeshProUGUI CardPartAbilityTitle;
    [SerializeField] public TextMeshProUGUI CardPartAbilityDescText;
    [Title("Current Hand Cost")]
    [SerializeField] public TextMeshProUGUI CurrentHandCostText;

    
    /// <summary>
    ///  FULL CARD UI
    /// </summary
    public void SetFullCardUIVisibility(bool _state) {
        FullCardUIOwner.SetActive(_state);
    }
    
    /// <summary>
    ///  TOP ABILITIES
    /// </summary
    public void ChangeTopAbilityTitleText(string _name, string _cost) {
        TopAbilityTitleText.text = _name + " Top Ability | Cost: " + _cost;
    }
    public void ChangeTopAbilityDescText(string _desc) {
        TopAbilityDescText.text = _desc;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    ///  BOTTOM ABILITIES
    /// </summary>
    public void ChangeBottomAbilityTitleText(string _name, string _cost) {
        BottomAbilityTitleText.text = _name + " Bottom Ability | Cost: " + _cost;
    }
    public void ChangeBottomAbilityDescText(string _desc) {
        BottomAbilityDescText.text = _desc;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///  TOTAL CARD COST
    /// </summary>
    public void ChangeTotalCostText(string _cost) {
        TotalCostText.text = "Cost: " + _cost;
    }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    
    /// <summary>
    ///  CARD PART ABILITY
    /// </summary>
    public void SetCardPartAbilityTextVisiblity(bool _state) {
        CardPartAbilityOwner.SetActive(_state);
    }
    public void ChangeCardPartAbilityTitleText(string _name, string _cost, string _position) {
        CardPartAbilityTitle.text = _name + " " + _position + " Ability | Cost: " + _cost;
    }
    public void ChangeCardPartAbilityDescText(string _desc) {
        CardPartAbilityDescText.text = _desc;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    ///  CURRENT HAND COST
    /// </summary>
    public void ChangeHandCostText(string _cost) {
        CurrentHandCostText.text = "Hand Cost: " + _cost + "/10";
    }
}
