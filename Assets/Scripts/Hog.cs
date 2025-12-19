using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Hog : MonoBehaviour,IEnemy
{
    public float m_health { get; set; } = 70;
    public Animator m_animator;
    public Slider m_slider;

    private float m_wspeed = 2;
    private float m_rspeed = 5;
    private float m_attackcooldown = 2;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_attackcooldown = 0;
        m_animator.SetFloat("AttackCooldown", 0);
        m_slider.maxValue = m_health;
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
        m_animator.SetTrigger("Death");
        StopAllCoroutines();
    }
}
