using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Snail : MonoBehaviour,IEnemy
{
    public Animator m_animator;
    public Rigidbody2D m_rb;
    public Slider m_slider;
    public BoxCollider2D m_hitBox;

    private float m_moveDistance = 4f;
    private float m_moveSpeed = 8f;
    private float m_moveDelay = 1.5f;

    public float m_attackDmg { get; set; } = 30;
    public float m_health { get; set; } = 50.0f;
    private int m_direction = -1;

    private void Awake()
    {
        // Cache component
        if (m_rb == null)
            m_rb = GetComponent<Rigidbody2D>();
        if (m_slider != null)
        {
            m_slider.maxValue = m_health;
            m_slider.value = m_health;
        }
    }

    private void Start()
    {
        m_health = 50;
        StartCoroutine(MoveRoutine());
    }
    private bool IsGrounded()
    {
        return Mathf.Abs(m_rb.linearVelocity.y) < 0.01f;
    }
    IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitUntil(IsGrounded);

            yield return StartCoroutine(MoveStep());
            yield return new WaitForSeconds(m_moveDelay);
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            m_direction *= -1;
        }
    }
    
    IEnumerator MoveStep()
    {
        m_animator.SetTrigger("Move");

        Vector2 startPos = m_rb.position;
        Vector2 targetPos = startPos + Vector2.right* m_direction * m_moveDistance;

        while (Vector2.Distance(m_rb.position, targetPos) > 0.01f)
        {
            Vector2 newPos = Vector2.MoveTowards(
                m_rb.position,
                targetPos,
                m_moveSpeed * Time.fixedDeltaTime
            );

            m_rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
    }
    public void TakeDamage(float dmg)
    {
        m_health = Mathf.Max(0, m_health - dmg);
        //m_animator.SetTrigger("TakeDmg");
        Debug.Log(gameObject.name + " da nhan " + dmg.ToString()+" dmg");
        if (m_slider != null)
            m_slider.value = m_health;

        if (m_health <= 0)
            Die();
    }
    void Die()
    {
        m_animator.SetTrigger("Death");
        Debug.Log(gameObject.name + " da chet");
        StopAllCoroutines();
        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
        Destroy(gameObject, 1);
    }
}
