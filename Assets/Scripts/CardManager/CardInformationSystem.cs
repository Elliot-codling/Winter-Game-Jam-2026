using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.PropertyVariants.TrackedObjects;

public class CardInformationSystem : MonoBehaviour
{
    [SerializeField] private GameObject m_playerTracked;
    [SerializeField] private GameObject m_frankTracked;

    [SerializeField] private TextMeshProUGUI m_textField;
    public static UnityEvent<string> UpdateCardInformation = new UnityEvent<string>();

    private void Awake()
    {
        UpdateCardInformation.AddListener(UpdateText);
    }

    private void UpdateText(string _newText)
    {
        m_textField.text = _newText;
    }
}
