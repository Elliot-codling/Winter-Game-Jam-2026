using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

public class HealthClient : MonoBehaviour
{
    // ### Editor only ###
    [Title("Client Settings")] 
    [SerializeField] private int m_startingHealth = 10;
    [SerializeField] private int m_startingShield = 0;

    [SerializeField] private UnityEvent<string> m_clientDeath;

    void Start()
    {
        // Listen to any incoming messages
        HealthManager.m_informHealthClient.AddListener(SortMessage);
        // Let the health manager know we are here

        HealthManager.m_informHealthManager?.Invoke(
            HealthManager.MessageType.GameObjectActive,
            gameObject.name,
            new HealthPackage(m_startingHealth, m_startingShield, 0)
            );
    }

    void SortMessage(HealthManager.MessageType _messageType, string _memberName, HealthPackage _variables)
    {
        // Return if the member does not match this game object
        if (_memberName != gameObject.name)
        {
            return;
        }
        switch (_messageType)
        {
            case HealthManager.MessageType.Death:
                m_clientDeath?.Invoke(_memberName);
                break;
        }
    }

    
}
