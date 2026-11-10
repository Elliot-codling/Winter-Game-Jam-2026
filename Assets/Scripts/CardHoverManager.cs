using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardHoverManager : MonoBehaviour {
    [SerializeField] private CardDescriptionUIDataContainer UIData;
    private GameManager GManager;

    private bool IsHoveringCard = false;
    private bool IsHoveringCardPart = false;

    private string CurrentHoveredCardGUID = "";
    private string CurrentHoveredCardPartGUID = "";
    

    private void Awake() {
        GManager = FindAnyObjectByType<GameManager>();
        
        // Disable UI On Awake
        UIData.SetFullCardUIVisibility(false);
        UIData.SetCardPartAbilityTextVisiblity(false);
    }

    private void Update() {
        if (!GManager.AcceptPlayerInteraction) {
            if (UIData.FullCardUIOwner.activeInHierarchy) {
                UIData.SetFullCardUIVisibility(false);
            }
            if (UIData.CardPartAbilityOwner.activeInHierarchy) {
                UIData.SetCardPartAbilityTextVisiblity(false);
            }
            
            return;
        }
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.CompareTag("Card")) {
                // Avoid setting this multiple times
                if (IsHoveringCard && CurrentHoveredCardGUID == hit.transform.name) {
                    return;
                }
                
                IsHoveringCard = true;
                CurrentHoveredCardGUID = hit.transform.name;
                
                // Get card data
                CardData data = GManager.PlayerInventory.GetCardData(hit.transform.name);
                if (data == null) {
                    Debug.Log("Dats is somehow null");
                    return;
                }
                
                UIData.SetFullCardUIVisibility(true);
                UIData.ChangeTopAbilityTitleText(data.CardTopData.PartName, data.CardTopData.PartCost.ToString());
                UIData.ChangeTopAbilityDescText(data.CardTopData.AbilityDesc);
                
                UIData.ChangeBottomAbilityTitleText(data.CardBottomData.PartName, data.CardBottomData.PartCost.ToString());
                UIData.ChangeBottomAbilityDescText(data.CardBottomData.AbilityDesc);

                int cardCost = data.CardTopData.PartCost + data.CardBottomData.PartCost;
                UIData.ChangeTotalCostText(cardCost.ToString());
            }
            else {
                // If was hovering card, disable UI elements
                if (IsHoveringCard) {
                    UIData.SetFullCardUIVisibility(false);
                    CurrentHoveredCardGUID = "";
                    IsHoveringCard = false;
                }
            }
            
            if (hit.transform.CompareTag("CardPart")) {
                // Avoid setting this multiple times
                if (IsHoveringCardPart && CurrentHoveredCardPartGUID == hit.transform.name) {
                    return;
                }
                
                IsHoveringCardPart = true;
                CurrentHoveredCardPartGUID = hit.transform.name;
                
                // Get card part data
                CardPartData data = GManager.PlayerInventory.GetCardPartData(hit.transform.name);
                
                UIData.SetCardPartAbilityTextVisiblity(true);
                UIData.ChangeCardPartAbilityTitleText(data.PartName, data.PartCost.ToString(), data.PartPosition.ToString());
                UIData.ChangeCardPartAbilityDescText(data.AbilityDesc);
            }
            else {
                // If was hovering card part, disable UI elements
                if (IsHoveringCardPart) {
                    UIData.SetCardPartAbilityTextVisiblity(false);
                    CurrentHoveredCardPartGUID = "";
                    IsHoveringCardPart = false;
                }
            }
        }
        else {
            // If was hovering card, disable UI elements
            if (IsHoveringCard) {
                UIData.SetFullCardUIVisibility(false);
                IsHoveringCard = false;
                CurrentHoveredCardGUID = "";
            }
            
            // If was hovering card part, disable UI elements
            if (IsHoveringCardPart) {
                UIData.SetCardPartAbilityTextVisiblity(false);
                IsHoveringCardPart = false;
                CurrentHoveredCardPartGUID = "";
            }
        }
    }
}
