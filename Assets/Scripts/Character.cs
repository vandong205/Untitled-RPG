using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Character : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputActionAsset inputActions;
    public PlayerStatController playerStatController;
    public Sword sword;

    private InputAction m_moveAction;
    private InputAction m_attackAction;
    private InputAction m_jumpAction;

    private float m_attackDmg = 10.0f;
    private Animator m_animator;
    private Vector2 m_moveAmt;
    private bool m_facingRight = true;
    private float _runSpeed = 5f;
    private bool m_canMove = true;
    private bool m_isAttacking = false;

    private float m_health = 500;
    private float m_maxhealth = 500;
    private float m_energy = 100;
    private float m_maxenergy = 100;
    private float m_attackenergy = 10;
    private float m_ability = 0;
    private float m_maxability = 100;
    private bool onGround = true;
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
        m_jumpAction = inputActions
            .FindActionMap("Player")
            .FindAction("Jump");
        playerStatController.SetHPBar(m_health/m_maxhealth);
        playerStatController.SetEnergyBar(m_energy/m_maxenergy);
        playerStatController.SetAbilityBar(m_ability / m_maxability);
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
        CheckGrounded();
        Jump();
        Attack();
        FlipCharacter();
    }
    private void CheckGrounded()
    {
        if (rb.linearVelocityY <= 0)
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }
        Debug.Log("OnGround: " + onGround);
    }
    private void Jump()
    {
        if (m_jumpAction.triggered && onGround)
        {
            m_animator.SetTrigger("Jump");
            rb.linearVelocityY = 5;
        }
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
    public void CheckHit()
    {
        sword.Hit();
    }
    private void Attack()
    {
        if (m_attackAction.triggered && !m_isAttacking)
        {
            m_energy =Mathf.Max(0,m_energy-m_attackenergy);
            playerStatController.SetAbilityBar(m_ability / m_maxability);
            m_ability = Mathf.Min(m_ability+m_attackenergy, m_maxability);
            playerStatController.SetEnergyBar(m_energy / m_maxenergy);
            m_isAttacking = true;
            m_canMove = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            m_animator.SetTrigger("Attack");
        }
    }
    public void TakeDamage(float dmg)
    {
        m_health = Mathf.Max(0, m_health - dmg);
        playerStatController.SetHPBar(m_health/ m_maxhealth);
        //m_animator.SetTrigger("TakeDmg");
        Debug.Log(gameObject.name + " da nhan " + dmg.ToString() + " dmg");
        if (m_health <= 0)
            Death();
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
    public Vector2 getPositon()
    {
        return transform.position;
    }
}
