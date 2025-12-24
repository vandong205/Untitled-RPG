using UnityEngine;

public class PlayerLogicVisualBridge : MonoBehaviour
{
    public Character character;
    public void CheckWeaponHit()
    {
        character.CheckHit();
    }
    public void EnableMoving()
    {
        character.EnableMoving();
    }
}
