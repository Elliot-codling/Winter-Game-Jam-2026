using System.Collections.Generic;
using TriInspector;
using UnityEngine;

public class WeighingSystem : MonoBehaviour
{
    public enum CardType
    {
        Attack,
        Defence,
        DrawExtra,
        DamageHp,
        DamageShield,
        HalfCost,
        TwoTurns
    }

    public struct CardData
    {
        public CardType m_cardType;
        public int m_value;
    }


    [ShowInInspector] private List<CardData> m_franksInventory = new();

    // DEBUG
    [ReadOnly] public float m_chanceOfAttack = 0.5f;
    [ReadOnly] public float m_chanceOfDefence = 0.5f;

    [Button]
    public void AddCard(CardData _newCard)
    {
        m_franksInventory.Add(_newCard);
    }

    [Button]
    public void PlayerAttacked(float _percentageLoss)
    {
        // The player has been damaged from the last round, increase chance of attack
        IncreaseChanceOfAttack(_percentageLoss / 2);
    }

    [Button]
    public void FrankAttacked(float _percentageLoss)
    {
        // Frank has been damaged from the last round, increase chance of defence
        IncreaseChanceOfDefence(_percentageLoss / 2);
    }

    [Button]
    public void PlayNextRound()
    {

    }



    // ### Percentage Change ###
    private void IncreaseChanceOfAttack(float _percentage)
    {
        m_chanceOfAttack += _percentage;
        m_chanceOfDefence -= _percentage;
        ClampPercentages();
    }
    private void IncreaseChanceOfDefence(float _percentage)
    {
        m_chanceOfAttack -= _percentage;
        m_chanceOfDefence += _percentage;
        ClampPercentages();
    }

    private void ClampPercentages()
    {
        m_chanceOfDefence = Mathf.Clamp(m_chanceOfDefence, 0, 1);
        m_chanceOfAttack = Mathf.Clamp(m_chanceOfAttack, 0, 1);
    }
}
