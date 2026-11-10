using Frankencard.CardPartConstants;
using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_frank;
    public struct PlayedCards
    {
        public List<CardData> m_selectedCards;
        public GameObject m_owner;
        public GameObject m_opponent;
    }
    [ShowInInspector] private PlayedCards m_playerSide;
    [ShowInInspector] private PlayedCards m_frankSide;

    [SerializeField] private HandManager m_handManager;
    [SerializeField] private RandomSystem m_enemyAI;

    [SerializeField] private float m_waitTimePerAbility = 1.0f;
    

    public UnityEvent FinishedRoundExecution  = new UnityEvent();

    void Start()
    {
        SetOwners();

        m_playerSide.m_selectedCards = new List<CardData>();
        m_frankSide.m_selectedCards = new List<CardData>();

        m_handManager.CardsSubmitted.AddListener(AddPlayerCards);
        m_enemyAI.FranksCardsSubmitted.AddListener(AddFrankCards);
    }
    public void AddPlayerCards(PlayedCards _playedCards)
    {
        m_playerSide = _playedCards;
    }
    public void AddFrankCards(PlayedCards _playedCards)
    {
        m_frankSide = _playedCards;
        StartCoroutine(StartRound());
    }

    [Button]
    public IEnumerator StartRound()
    {
        yield return new WaitForSeconds(0.1f);
        SetOwners();
        StartCoroutine(PlayerGainShield());
    }

    [Button]
    public void DealWithCard(PlayedCards _playedCards, CardData _currentCard, bool _shieldRound)
    {
        // ### Top card Types: ###
        //
        // Attack_General
        // Gain_Shield
        // DrawExtra
        //
        // ### Bottom card type: ###
        //
        // Half_cost (Halves the cost of a card)
        // Attack_HP (does hp attack dmg only)
        // Double_Round (lasts for two rounds)
        // Double_value (increases the value of the top card, if the opponent has shield )

        var topCardType = _currentCard.CardTopData.PartType;
        switch (topCardType)
        {
            case CardType.ATTACK_GENERAL:
                AttackCard(_playedCards, _currentCard, _shieldRound);
                break;
            case CardType.GAIN_SHIELD:
                DefenceCard(_playedCards, _currentCard, _shieldRound);
                break;
            case CardType.DRAW_EXTRA:
                break;
        }
    }


    private void AttackCard(PlayedCards _playedCards, CardData _currentCard, bool _shieldRound)
    {
        var bottomCardType = _currentCard.CardBottomData.PartType;
        var topCardValue = _currentCard.CardTopData.AbilityValue;
        HealthPackage info = new HealthPackage();
        info.m_passedValue = topCardValue;

        HealthPackage info2 = new HealthPackage();
        info2.m_passedValue = _currentCard.CardBottomData.AbilityValue;

        switch (bottomCardType)
        {
            // ### SPECIAL CARDS ###
            case CardType.ATTACK_HP when _shieldRound == false:
                // "Silverbullet"
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealHealthDmg, _playedCards.m_opponent.name, info);
                break;
            case CardType.DOUBLE_VALUE when _shieldRound == true:
                // "stake"
                // Bot card gives shield
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
            case CardType.DOUBLE_VALUE when _shieldRound == false:
                // "stake"
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info);
                break;
            case CardType.HALF_COST when _shieldRound == false:
                // "frankenstein"
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info);
                break;
            case CardType.DOUBLE_ROUND when _shieldRound == false:
                // "werewolf"
                info.m_passedValue = topCardValue * 2;
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info);
                break;
            case CardType.GAIN_SHIELD when _shieldRound == true:
                // when shield round, only do bot
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info2);
                break;
            case CardType.GAIN_SHIELD when _shieldRound == false:
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info);
                break;
            case CardType.ATTACK_GENERAL when _shieldRound == false:
                info.m_passedValue += info2.m_passedValue;
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info);
                break;
        }
    }
    private void DefenceCard(PlayedCards _playedCards, CardData _currentCard, bool _shieldRound)
    {
        var bottomCardType = _currentCard.CardBottomData.PartType;
        var topCardValue = _currentCard.CardTopData.AbilityValue;
        HealthPackage info = new HealthPackage();
        info.m_passedValue = topCardValue;

        HealthPackage info2 = new HealthPackage();
        info2.m_passedValue = _currentCard.CardBottomData.AbilityValue;

        switch (bottomCardType)
        {
            // ### SPECIAL CARDS ###
            case CardType.ATTACK_HP when _shieldRound == false:
                // "Silverbullet"
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
            case CardType.DOUBLE_VALUE when _shieldRound == true:
                // "stake"
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
            case CardType.DOUBLE_VALUE when _shieldRound == false:
                // "stake"
                // Bot card attacks
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info);
                break;
            case CardType.HALF_COST when _shieldRound == true:
                // "frankenstein"
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
            case CardType.DOUBLE_ROUND when _shieldRound == true:
                // "werewolf"
                info.m_passedValue = topCardValue * 2;
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
            case CardType.GAIN_SHIELD when _shieldRound == true:
                info.m_passedValue += info2.m_passedValue;
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
            case CardType.ATTACK_GENERAL when _shieldRound == false:
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.DealAttackDmg, _playedCards.m_opponent.name, info2);
                break;
            case CardType.ATTACK_GENERAL when _shieldRound == true:
                HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.GainShield, _playedCards.m_owner.name, info);
                break;
        }
    }

    private void SetOwners()
    {
        m_playerSide.m_owner = m_player;
        m_playerSide.m_opponent = m_frank;

        m_frankSide.m_owner = m_frank;
        m_frankSide.m_opponent = m_player;
    }

    private IEnumerator PlayerGainShield()
    {
        HealthPackage previousHealthPackage = new HealthPackage();
        HealthPackage currentHealthPackage = new HealthPackage();
        float changeInValue = 0;
        CardInformationSystem.UpdateCardInformation?.Invoke("Let the battle begin!");
        yield return new WaitForSeconds(m_waitTimePerAbility);

        previousHealthPackage = HealthManager.m_instance.GetMemberInformation(m_playerSide.m_owner.name);
        // Find shield in player, and then play them
        foreach (var cardData in m_playerSide.m_selectedCards)
        {
            DealWithCard(m_playerSide, cardData, true);
        }
        currentHealthPackage = HealthManager.m_instance.GetMemberInformation(m_playerSide.m_owner.name);
        changeInValue = ChangeInValue(previousHealthPackage, currentHealthPackage);
        CardInformationSystem.UpdateCardInformation?.Invoke("Player has gained " + changeInValue + " shield");

        yield return new WaitForSeconds(m_waitTimePerAbility);
        previousHealthPackage = HealthManager.m_instance.GetMemberInformation(m_frankSide.m_owner.name);
        // Find shield in enemy, and then play them
        foreach (var cardData in m_frankSide.m_selectedCards)
        {
            DealWithCard(m_frankSide, cardData, true);
        }

        currentHealthPackage = HealthManager.m_instance.GetMemberInformation(m_frankSide.m_owner.name);
        changeInValue = ChangeInValue(previousHealthPackage, currentHealthPackage);
        CardInformationSystem.UpdateCardInformation?.Invoke("Frank has gained " + changeInValue + " shield");

        yield return new WaitForSeconds(m_waitTimePerAbility);
        previousHealthPackage = HealthManager.m_instance.GetMemberInformation(m_playerSide.m_opponent.name);
        // Player attack
        foreach (var cardData in m_playerSide.m_selectedCards)
        {
            DealWithCard(m_playerSide, cardData, false);
        }

        currentHealthPackage = HealthManager.m_instance.GetMemberInformation(m_playerSide.m_opponent.name);
        changeInValue = ChangeInValue(previousHealthPackage, currentHealthPackage);
        CardInformationSystem.UpdateCardInformation?.Invoke("Player has dealt " + changeInValue + " damage");

        yield return new WaitForSeconds(m_waitTimePerAbility);
        previousHealthPackage = HealthManager.m_instance.GetMemberInformation(m_frankSide.m_opponent.name);
        //Frank attack
        foreach (var cardData in m_frankSide.m_selectedCards)
        {
            DealWithCard(m_frankSide, cardData, false);
        }

        currentHealthPackage = HealthManager.m_instance.GetMemberInformation(m_frankSide.m_opponent.name);
        changeInValue = ChangeInValue(previousHealthPackage, currentHealthPackage);
        CardInformationSystem.UpdateCardInformation?.Invoke("Frank has dealt " + changeInValue + " damage");

        yield return new WaitForSeconds(m_waitTimePerAbility);
        CardInformationSystem.UpdateCardInformation?.Invoke("Round over!");
        yield return new WaitForSeconds(2);
        CardInformationSystem.UpdateCardInformation?.Invoke("");
        HealthManager.m_informHealthManager?.Invoke(HealthManager.MessageType.ResetShieldForAllMembers, "", new HealthPackage());
        FinishedRoundExecution?.Invoke();
    }

    private float ChangeInValue(HealthPackage _previous, HealthPackage _current)
    {
        float overallChange = 0;
        overallChange += Mathf.Abs(_current.m_healthValue - _previous.m_healthValue);
        overallChange += Mathf.Abs(_current.m_shieldValue - _previous.m_shieldValue);

        return overallChange;
    }


}
