using UnityEngine;

public class Sword : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        IEnemy enemy = collision.GetComponent<IEnemy>();
        if (enemy != null)
        {
            Debug.Log("Da danh trung!");
            enemy.TakeDamage(20);
        }
    }
}
