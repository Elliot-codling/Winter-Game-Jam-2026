using UnityEngine;

public struct HealthPackage
{
    public HealthPackage(int _healthValue, int _shieldValue, int _passedValue)
    {
        m_healthValue = _healthValue;
        m_shieldValue = _shieldValue;
        m_passedValue = _passedValue;
    }

    public int m_healthValue;
    public int m_shieldValue;
    public int m_passedValue;
}
