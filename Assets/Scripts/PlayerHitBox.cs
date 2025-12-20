using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IEnemy enemy = collision.GetComponentInParent<IEnemy>();
        Character player = GetComponentInParent<Character>();
        if(enemy != null)
        {
            player.TakeDamage(enemy.m_attackDmg);
        }
    }
}
