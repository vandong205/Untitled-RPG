using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Character : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputActionAsset inputActions;
    public Transform Sword;

    private InputAction m_moveAction;
    private InputAction m_attackAction;

    private float m_attackDmg = 10.0f;
    private Animator m_animator;
    private Vector2 m_moveAmt;
    private bool m_facingRight = true;
    private float _runSpeed = 5f;
    private bool m_canMove = true;
    private bool m_isAttacking = false;

    private float m_health = 100;
    enum PlayerState
    {
        Idle,
        Run
    }

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_moveAction = inputActions
            .FindActionMap("Player")
            .FindAction("Move");
        m_attackAction = inputActions
            .FindActionMap("Player")
            .FindAction("Attack");
        Sword.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        UpdateAnimation();
        Attack();
        FlipCharacter();
    }

    private void FixedUpdate()
    {
        Running();
        
    }
    private void Running()
    {
        if (!m_canMove)
        {
            Debug.Log("Khong the di chuyen");
            return;
        }
        rb.linearVelocity = new Vector2(
          m_moveAmt.x * _runSpeed,
          rb.linearVelocity.y
      );
    }
    private void Attack()
    {
        if (m_attackAction.triggered && !m_isAttacking)
        {
            Sword.gameObject.SetActive(true);
            m_isAttacking = true;
            m_canMove = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            m_animator.SetTrigger("Attack");
            StartCoroutine(MoveSafetyRoutine(0.3f));
        }
    }
    private void Death()
    {
        if (m_health <= 0)
        {
            m_animator.SetTrigger("Death");
            m_canMove = false;
        }
    }
    public void EnableMoving()
    {
        Debug.Log("EnableMoving CALLED");
        m_canMove = true;
        m_isAttacking = false;
    }
    public float getAttackDamage() { return this.m_attackDmg; }
    private void UpdateAnimation()
    {
        bool isRunning = m_canMove&&Mathf.Abs(m_moveAmt.x) > 0.01f;
        m_animator.SetBool("isRunning", isRunning);
    }

    private void FlipCharacter()
    {
        if (m_moveAmt.x > 0 && !m_facingRight)
        {
            Flip();
        }
        else if (m_moveAmt.x < 0 && m_facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        m_facingRight = !m_facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private IEnumerator MoveSafetyRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Sword.gameObject.SetActive(false);
        EnableMoving();
    }
}
