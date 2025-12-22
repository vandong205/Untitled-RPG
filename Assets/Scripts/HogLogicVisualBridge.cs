using UnityEngine;

public class HogLogicVisualBridge : MonoBehaviour
{
    public Hog m_main;
    public void CheckView()
    {
        m_main.ChechView();
    }
    public void setPreviousAnimationState()
    {
        m_main.setAnimationState();
    }
}
