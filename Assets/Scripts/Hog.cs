using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hog : MonoBehaviour, IEnemy
{
    enum HogState
    {
        Walk,
        Attack,
        Cooldown
    }

    // ====== PUBLIC ======
    public float m_health { get; set; } = 300;
    public float m_attackDmg { get; set; } = 30;

    public Collider2D m_view;
    public LayerMask playerMask;
    public Rigidbody2D m_rb;
    public Animator m_animator;
    public Slider m_slider;

    // ====== CONFIG ======
    [SerializeField] private float m_walkSpeed = 2f;
    [SerializeField] private float m_runSpeed = 7f;
    [SerializeField] private float m_moveDistance = 3f;
    [SerializeField] private float m_attackCooldownTime = 3f;

    // ====== STATE ======
    private HogState m_state = HogState.Walk;
    private float m_direction ;
    private Vector2 m_targetPos;

    private Coroutine m_walkCoroutine;
    private Coroutine m_attackCoroutine;

    // ====== UNITY ======
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_direction=transform.localScale.x<0 ? 1 : -1;  
        if (m_slider != null)
        {
            m_slider.maxValue = m_health;
            m_slider.value = m_health;
        }
    }

    private void Start()
    {
        StartWalk();
    }

    // ====== WALK ======
    void StartWalk()
    {
        StopAllCoroutines();
        m_state = HogState.Walk;
        m_walkCoroutine = StartCoroutine(WalkRoutine());
    }

    IEnumerator WalkRoutine()
    {
        while (m_state == HogState.Walk)
        {
            yield return MoveStep();
            Flip();
            yield return null;
        }
    }
    IEnumerator MoveStep()
    {
        Vector2 startPos = m_rb.position;
        Vector2 targetPos = startPos + Vector2.right * m_direction * m_moveDistance;

        while (Vector2.Distance(m_rb.position, targetPos) > 0.05f)
        {
            if (m_state != HogState.Walk)
                yield break;

            Vector2 newPos = Vector2.MoveTowards(
                m_rb.position,
                targetPos,
                m_walkSpeed * Time.fixedDeltaTime
            );

            m_rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        m_direction *= -1;
    }

    // ====== VIEW CHECK ======
    public void ChechView()
    {
        if (m_state != HogState.Walk)
            return;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerMask);
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[3];
        m_view.Overlap(filter, results);

        foreach (Collider2D col in results)
        {
            Character player = col?.GetComponentInParent<Character>();
            if (player == null) continue;

            Vector2? pos = player.getPositon();
            if (!pos.HasValue) continue;

            StartAttack(pos.Value);
            break;
        }
    }

    // ====== ATTACK ======
    void StartAttack(Vector2 targetPos)
    {
        StopAllCoroutines();

        m_state = HogState.Attack;
        m_targetPos = targetPos;

        m_animator.SetTrigger("Attack");
        m_attackCoroutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        // ---- chạy tới player ----
        while (Vector2.Distance(m_rb.position, m_targetPos) > 0.05f)
        {
            Vector2 newPos = Vector2.MoveTowards(
                m_rb.position,
                m_targetPos,
                m_runSpeed * Time.fixedDeltaTime
            );

            m_rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }


        m_rb.linearVelocity = Vector2.zero;
        m_animator.SetTrigger("FinishAttack");
        // ---- cooldown ----
        m_state = HogState.Cooldown;
        yield return new WaitForSeconds(m_attackCooldownTime);
        m_animator.SetTrigger("Walk");

        // ---- quay lại walk ----
        StartWalk();
    }
    // ====== DAMAGE ======
    public void TakeDamage(float dmg)
    {
        m_health = Mathf.Max(0, m_health - dmg);
        m_animator.SetTrigger("TakeDmg");

        if (m_slider != null)
            m_slider.value = m_health;

        if (m_health <= 0)
            Die();
    }

    void Die()
    {
        StopAllCoroutines();

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        Destroy(gameObject, 1f);
    }

    public float getAttackDamage()
    {
        return m_attackDmg;
    }
    public void setAnimationState()
    {
        switch (m_state)
        {
            case HogState.Cooldown:
                m_animator.SetInteger("IsInState", 0);
                Debug.Log("Da chuyen State ve 0");
                break;
            case HogState.Walk:
                m_animator.SetInteger("IsInState", 1);
                Debug.Log("Da chuyen State ve 1");
                break;
            case HogState.Attack:
                m_animator.SetInteger("IsInState", 2);
                Debug.Log("Da chuyen State ve 2");
                break;
        }
    }
}
