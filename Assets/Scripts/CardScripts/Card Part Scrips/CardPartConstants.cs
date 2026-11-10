using System;
//using System.Collections.Generic;
//using TriInspector;
//using UnityEngine;

namespace Frankencard.CardPartConstants {
    [Serializable]
    public enum CardIdentifier {
        NONE = 0,
        MERGED,
        WEREWOLF,
        SILVER_BULLET,
        PITCHFORK,
        FIRE,
        DRACULA,
        BOLT,
        BOLT2,
        STAKE,

        // Make sure this is always last
        ID_MAX,

        // The naughty corner
        FRANKENSTEIN

    }

    public enum CardType
    {
        NONE = 0,           // Modifier only

        ATTACK_GENERAL,
        GAIN_SHIELD,
        DRAW_EXTRA,

        ATTACK_HP,
        DOUBLE_VALUE,
        HALF_COST,
        DOUBLE_ROUND
    }
    
    [Serializable]
    public enum CardPartPosition {
        NONE = 0,
        TOP = 1,
        BOTTOM = 2
    }

    // TODO: Implement mapping enum -> string name
    //[Serializable, CreateAssetMenu(fileName = "Card Name")]
    //public class CardConstants : ScriptableObject {
    //    [SerializeField, DictionaryDrawerSettings(AlwaysExpanded = true, KeyColumnSize = 0.25f)] 
    //    public Dictionary<CardIdentifier, string> NameDictionary;
    //}
}
