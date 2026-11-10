using System;
using TMPro;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class HealthUI : MonoBehaviour
{
    // ### Editor only ###
    [Title("Tracked Object")]
    [SerializeField] private GameObject m_trackedObject;
    [ReadOnly] [SerializeField] private int m_trackedObjectMaxHealth;
    [ReadOnly][SerializeField] private int m_trackedObjectMaxShield;

    [SerializeField] TextMeshProUGUI m_healthText;
    [SerializeField] TextMeshProUGUI m_shieldText;

    void Awake()
    {
        HealthManager.m_informHealthClient.AddListener(SortMessage);
    }
    private void SortMessage(HealthManager.MessageType _messageType, string _memberName, HealthPackage _variables)
    {
        // Return if the member does not match this game object
        if (_memberName != m_trackedObject.name)
        {
            return;
        }
        switch (_messageType)
        {
            case HealthManager.MessageType.InitUI:
                m_trackedObjectMaxHealth = _variables.m_healthValue;
                m_trackedObjectMaxShield = _variables.m_shieldValue;
                UpdateText(_variables);
                break;
            case HealthManager.MessageType.UpdateUI:
                UpdateText(_variables);
                break;
        };
    }

    private void UpdateText(HealthPackage _variables)
    {
        m_healthText.text = _variables.m_healthValue.ToString();
        m_shieldText.text = _variables.m_shieldValue.ToString();
    }
}
