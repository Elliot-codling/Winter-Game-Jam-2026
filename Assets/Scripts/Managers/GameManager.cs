using Frankencard.CardConstants;
using UnityEngine;

// TODO: This should probably be a singleton

public class GameManager : MonoBehaviour {
    public CardConstants CardDataConstants;

    public Inventory PlayerInventory;
    public Inventory EnemyInventory;
    
    public bool AcceptPlayerInteraction = false;

}
