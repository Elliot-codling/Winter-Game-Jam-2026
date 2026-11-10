using System;
using TMPro;
using UnityEngine;

public class UIDeathMessageLogic : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_frank;
    [SerializeField] private TextMeshProUGUI m_textField;

    public void UpdateMessage(string _memberName)
    {
        if (_memberName == m_player.name)
        {
            m_textField.text = "You have died!";
        }

        if (_memberName == m_frank.name)
        {
            m_textField.text = "Frank died!";
        }
    }
}
