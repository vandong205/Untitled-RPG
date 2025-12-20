using UnityEngine;

public class Sword : MonoBehaviour
{
    public Collider2D swordCollider;  
    public LayerMask enemyMask;
    public int damage = 20;

    public void Hit()
    {
        Debug.Log("Dang kiem tra Hit");

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyMask);
        filter.useTriggers = true; 

        Collider2D[] results = new Collider2D[10];
        int count = swordCollider.Overlap(filter, results);

        Debug.Log("Da chem trung " + count);

        for (int i = 0; i < count; i++)
        {
            IEnemy enemy = results[i].GetComponentInParent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
