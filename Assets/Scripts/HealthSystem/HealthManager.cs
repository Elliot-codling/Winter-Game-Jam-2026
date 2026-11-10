using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public enum MessageType
    {
        // Initialisation
        InitUI,
        GameObjectActive,
        // Attack - Applies to shield first (if applicable), then applies to health (if applicable)
        DealAttackDmg,
        // Shield attack - Only applies dmg to shield (if applicable)
        DealShieldDmg,
        // Health attack - Only applies dmg to health (if applicable)
        DealHealthDmg,
        // Gain shield - Adds shield
        GainShield,
        // Updates the UI
        UpdateUI,
        // Reset round - Resets the shield to 0 for all members
        ResetShieldForAllMembers,
        // Reset shield - Only applies to one member
        ResetShieldForMember,
        // Death
        Death
    }

    // Singleton
    public static HealthManager m_instance;

    // Store the GameObjects which will participate in health
    private Dictionary<string, HealthPackage> m_healthMembers = new();

    // ### Unity events ###

    // Applicable GameObject
    public static UnityEvent<MessageType, string, HealthPackage> m_informHealthManager = new();
    
    // Applicable GameObject, damage points to apply
    public static UnityEvent<MessageType, string, HealthPackage> m_informHealthClient = new();

    [Button]
    public void EndGame()
    {
        m_informHealthClient?.Invoke(MessageType.Death, "Player", new HealthPackage());
    }
    void Awake()
    {
        if (m_instance)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
        DontDestroyOnLoad(gameObject);

        // Health will be preserved when switching scenes
        m_informHealthManager.AddListener(SortMessage);
    }

    public HealthPackage GetMemberInformation(string _memberName)
    {
        return m_healthMembers[_memberName];
    }

    private void SortMessage(MessageType _messageType, string _memberName, HealthPackage _variables)
    {
        switch (_messageType)
        {
            case MessageType.GameObjectActive:
                // Adds the member to the dictionary
                TryAddMember(_memberName, _variables);
                InitUI(_memberName, _variables);
                break;
            case MessageType.DealAttackDmg:
                ApplyAttackDamage(_memberName, _variables);
                UpdateUI(_memberName);
                break;
            case MessageType.DealHealthDmg:
                ApplyHealthDmg(_memberName, _variables);
                UpdateUI(_memberName);
                break;
            case MessageType.DealShieldDmg:
                ApplyShieldDmg(_memberName, _variables);
                UpdateUI(_memberName);
                break;
            case MessageType.GainShield:
                GainShield(_memberName, _variables);
                UpdateUI(_memberName);
                break;
            case MessageType.ResetShieldForAllMembers:
                ResetShieldForAll();
                return;
            case MessageType.ResetShieldForMember:
                ResetShieldForMember(_memberName);
                UpdateUI(_memberName);
                return;

        }

        if (HasMemberDied(_memberName))
        {
            m_informHealthClient?.Invoke(MessageType.Death, _memberName, _variables);
        }
    }

    private void TryAddMember(string _memberName, HealthPackage _variables)
    {
        m_healthMembers.TryAdd(_memberName, _variables);
    }
    private bool DoesMemberExist(string _memberName)
    {
        if (m_healthMembers.ContainsKey(_memberName))
        {
            return true;
        }
        Debug.LogWarning("Cannot find member: " + _memberName);
        return false;
    }

    private bool HasMemberDied(string _memberName)
    {
        var healthInfo = m_healthMembers[_memberName];
        return healthInfo.m_healthValue <= 0;
    }

    private void InitUI(string _memberName, HealthPackage _variables)
    {
        m_informHealthClient?.Invoke(MessageType.InitUI, _memberName, _variables);

        m_healthMembers[_memberName] = _variables;
    }

    private void UpdateUI(string _memberName)
    {
        m_informHealthClient?.Invoke(MessageType.UpdateUI, _memberName, m_healthMembers[_memberName]);
    }

    private void ApplyAttackDamage(string _memberName, HealthPackage _variables)
    {
        if (!DoesMemberExist(_memberName))
        {
            return;
        }

        var healthInfo = m_healthMembers[_memberName];

        if (healthInfo.m_shieldValue <= _variables.m_passedValue)
        {
            // Reduce the attack damage
            _variables.m_passedValue -= healthInfo.m_shieldValue;
            // Apply the rest to health
            healthInfo.m_healthValue -= _variables.m_passedValue;
            // Dont go below 0
            healthInfo.m_healthValue = Math.Clamp(healthInfo.m_healthValue, 0, int.MaxValue);
            // Set shield to 0
            healthInfo.m_shieldValue = 0;
        }
        else if (healthInfo.m_shieldValue > _variables.m_passedValue)
        {
            // Reduce the shield however its not enough to full get rid of shield
            healthInfo.m_shieldValue -= _variables.m_passedValue;
        }
        // Update information
        m_healthMembers[_memberName] = healthInfo;
    }

    private void ApplyHealthDmg(string _memberName, HealthPackage _variables)
    {
        if (!DoesMemberExist(_memberName))
        {
            return;
        }
        var healthInfo = m_healthMembers[_memberName];

        // Ignores shield, applies only to health
        healthInfo.m_healthValue -= _variables.m_passedValue;
        // Dont go below 0
        healthInfo.m_healthValue = Math.Clamp(healthInfo.m_healthValue, 0, int.MaxValue);
        // Update information
        m_healthMembers[_memberName] = healthInfo;
    }

    private void ApplyShieldDmg(string _memberName, HealthPackage _variables)
    {
        if (!DoesMemberExist(_memberName))
        {
            return;
        }
        var healthInfo = m_healthMembers[_memberName];

        // Ignores health, only applies to shield
        healthInfo.m_shieldValue -= _variables.m_passedValue;
        // Dont go below 0
        healthInfo.m_shieldValue = Math.Clamp(healthInfo.m_shieldValue, 0, int.MaxValue);
        // Update information
        m_healthMembers[_memberName] = healthInfo;
    }

    private void GainShield(string _memberName, HealthPackage _variables)
    {
        if (!DoesMemberExist(_memberName))
        {
            return;
        }
        var healthInfo = m_healthMembers[_memberName];
        healthInfo.m_shieldValue += _variables.m_passedValue;

        // Update information
        m_healthMembers[_memberName] = healthInfo;
    }

    private void ResetShieldForAll()
    {
        // Source - https://stackoverflow.com/a/1276792
        // Posted by Camal, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-11-03, License - CC BY-SA 4.0

        List<string> memberList = new List<string>(m_healthMembers.Keys);

        foreach (var member in memberList)
        {
            var healthInfo = m_healthMembers[member];
            healthInfo.m_shieldValue = 0;
            m_healthMembers[member] = healthInfo;

            UpdateUI(member);
        }
    }

    private void ResetShieldForMember(string _memberName)
    {
        // Source - https://stackoverflow.com/a/1276792
        // Posted by Camal, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-11-03, License - CC BY-SA 4.0

        var healthInfo = m_healthMembers[_memberName];
        healthInfo.m_shieldValue = 0;
        m_healthMembers[_memberName] = healthInfo;
    }


}
