using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [Header("HpBar")]
    public RectTransform m_HpMask;
    public RectTransform m_HpUI;
    public RectTransform m_EnergyUI;
    public RectTransform m_AbilityUI;
    public void SetHPBar(float percent)
    {
        float max_anchorposy = -186;
        Vector2 offset = new Vector2(0, max_anchorposy * (1-percent));
        m_HpMask.anchoredPosition = offset;
        m_HpUI.anchoredPosition=-offset;
    }
    public void SetEnergyBar(float percent) {
        float max = 152;
        m_EnergyUI.sizeDelta = new Vector2(max * percent, m_EnergyUI.sizeDelta.y);
    }
    public void SetAbilityBar(float percent)
    {
        float max = 97;
        m_AbilityUI.sizeDelta = new Vector2(max * percent, m_AbilityUI.sizeDelta.y);
    }

}
