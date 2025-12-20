using UnityEngine;

public class HogAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Va cham player");
        Character player = collision.GetComponentInParent<Character>();
        Hog hog = GetComponentInParent<Hog>() ;
        if(hog != null)
        {
            Debug.Log("hog dang co:" + hog.getAttackDamage().ToString());
            player.TakeDamage(hog.getAttackDamage());
        }
        
    }
}
