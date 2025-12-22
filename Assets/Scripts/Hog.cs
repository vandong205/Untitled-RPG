using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hog : MonoBehaviour, IEnemy
{
    enum HogState
    {
        Walk,
        Attack,
        Cooldown
    }
    public float m_health { get; set; } = 300;
    public float m_attackDmg { get; set; } = 1;

    public Collider2D m_view;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Rigidbody2D m_rb;
    public Animator m_animator;
    public Slider m_slider;
    public BoxCollider2D _groundChecker;
    public LayerMask _groundLayer;
    public Transform _wallCheckpoint;

    private float wallCheckDistance = 1;

    // ====== CONFIG ======
    private float m_walkSpeed = 2f;
    private float m_runSpeed = 7f;
    private float m_moveDistance = 3f;
    private float m_attackCooldownTime = 3f;
    private float m_direction = -1;

    // ====== STATE ======
    private HogState m_state = HogState.Walk;
    private Vector2 m_targetPos;

    // ====== RENDER ======
    private SpriteRenderer m_sprite;

    // ====== UNITY ======
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponentInChildren<Animator>();
        m_sprite = GetComponentInChildren<SpriteRenderer>();

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
        StartCoroutine(WalkRoutine());
    }

    IEnumerator WalkRoutine()
    {
        while (m_state == HogState.Walk)
        {
            yield return new WaitUntil(IsOnGround);
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
            if (IsBlocked()) yield break;
            Vector2 newPos = Vector2.MoveTowards(
                m_rb.position,
                targetPos,
                m_walkSpeed * Time.fixedDeltaTime
            );
            m_rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
            
        }
    }

    // ====== FLIP (SPRITE ONLY) ======
    void Flip()
    {
        if (m_sprite == null) return;
        m_view.gameObject.transform.localPosition = new Vector3(
            m_view.gameObject.transform.localPosition.x *-1,
            m_view.gameObject.transform.localPosition.y,
            m_view.gameObject.transform.localPosition.z
        );
        Vector2 off = m_view.offset;
        off.x = -off.x;
        m_view.offset = off;
        m_sprite.flipX = !m_sprite.flipX;

        m_direction *= -1;
    }
    bool IsBlocked()
    {
        Vector2 dir = Vector2.right * m_direction;
        return Physics2D.Raycast(
            _wallCheckpoint.position,
            dir,
            wallCheckDistance,
            _groundLayer
        );
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
            if (col == null) continue;

            Character player = col.GetComponentInParent<Character>();
            if (player == null) continue;

            Vector2? pos = player.getPositon();
            if (!pos.HasValue) continue;

            // ===== LINE OF SIGHT CHECK =====
            Vector2 origin = m_rb.position;
            Vector2 target = pos.Value;
            Vector2 dir = (target - origin).normalized;
            float dist = Vector2.Distance(origin, target);

            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                dir,
                dist,
                obstacleMask
            );

            // Nếu ray chạm vật cản → không attack
            if (hit.collider != null)
            {
                // Debug để nhìn ray
                Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
                return;
            }

            // Không có vật cản → attack
            Debug.DrawLine(origin, target, Color.green, 0.1f);
            StartAttack(target);
            break;
        }
    }


    private bool IsOnGround()
    {
        if (_groundChecker != null)
            return _groundChecker.IsTouchingLayers(_groundLayer);

        return false;
    }

    // ====== ATTACK ======
    void StartAttack(Vector2 targetPos)
    {
        StopAllCoroutines();

        m_state = HogState.Attack;
        m_targetPos = targetPos;

        //// xoay mặt về phía player trước khi chạy
        //float dirToPlayer = Mathf.Sign(m_targetPos.x - m_rb.position.x);
        //if (dirToPlayer != m_direction)
        //    Flip();

        m_animator.SetTrigger("Attack");
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return AttackHit();
        yield return AfterAttack();
    }
    IEnumerator AttackHit()
    {
        while (Vector2.Distance(m_rb.position, m_targetPos) > 0.05f)
        {
            if (IsBlocked()) break;
            Vector2 newPos = Vector2.MoveTowards(
                m_rb.position,
                m_targetPos,
                m_runSpeed * Time.fixedDeltaTime
            );

            m_rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator AfterAttack()
    {
        m_rb.linearVelocity = Vector2.zero;

        m_animator.SetTrigger("FinishAttack");

        m_state = HogState.Cooldown;
        yield return new WaitForSeconds(m_attackCooldownTime);

        m_animator.SetTrigger("Walk");
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

    // ====== INTERFACE ======
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
                break;
            case HogState.Walk:
                m_animator.SetInteger("IsInState", 1);
                break;
            case HogState.Attack:
                m_animator.SetInteger("IsInState", 2);
                break;
        }
    }
}
