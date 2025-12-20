using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Hog : MonoBehaviour,IEnemy
{
    public float m_health { get; set; } = 70;
    public Collider2D m_view;
    public LayerMask playerMask;
    public float m_attackDmg { get; set; } = 30;
    public Animator m_animator;
    public Slider m_slider;

    private float m_wspeed = 2;
    private float m_rspeed = 5;
    private float m_attackcooldown = 2;
    private float m_direction = -1;
    private Vector2 m_targetPos;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_attackcooldown = 0;
        m_animator.SetFloat("AttackCooldown", 0);
        m_slider.maxValue = m_health;
        m_slider.value = m_health;
    }
    public void ChechView()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerMask);
        filter.useTriggers = true;
        Collider2D[] results = new Collider2D[3];
        m_view.Overlap(filter, results);
        foreach(Collider2D col in results)
        {
            Character player = col?.GetComponentInParent<Character>();
            if(player==null) continue;
            Debug.Log("Da phat hien player "+player.gameObject.name);
            Vector2? playerPos = player.getPositon();
            if (playerPos.HasValue)
            {
                m_targetPos = playerPos.Value;
                m_animator.SetBool("SeeEnemy", true);
                Attack();
            }
        }

    }
    private void Attack()
    {
        Debug.Log(gameObject.name + " dang tan cong");
        //m_animator
    }
    public void TakeDamage(float dmg)
    {
        m_health = Mathf.Max(0, m_health - dmg);
        m_animator.SetTrigger("TakeDmg");
        Debug.Log(gameObject.name + " da nhan " + dmg.ToString() + " dmg");
        if (m_slider != null)
            m_slider.value = m_health;
        if (m_health <= 0)
            Die();
    }
    void Die()
    {
        Debug.Log(gameObject.name + " da chet");
        StopAllCoroutines();
        foreach(var col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
        Destroy(gameObject,1);
    }
    public float getAttackDamage()
    {
        return m_attackDmg;
    }


}
