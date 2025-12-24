using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputActionAsset inputActions;
    public PlayerStatController playerStatController;
    public Sword sword;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D swordCollider;

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction jumpAction;
    private Vector2 moveAmt;


    private bool _facingRight = true;
    private float _runSpeed = 5f;
    private bool _canMove = true;
    private bool _isAttacking = false;

    private float m_attackDmg = 10.0f;
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
        if(animator==null)
        animator = GetComponentInChildren<Animator>();

        moveAction = inputActions
            .FindActionMap("Player")
            .FindAction("Move");
        attackAction = inputActions
            .FindActionMap("Player")
            .FindAction("Attack");
        jumpAction = inputActions
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
        moveAmt = moveAction.ReadValue<Vector2>();

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
        if (jumpAction.triggered && onGround)
        {
            animator.SetTrigger("Jump");
            rb.linearVelocityY = 5;
        }
    }

    private void FixedUpdate()
    {
        Running();
        
    }
    private void Running()
    {
        if (!_canMove)
        {
            Debug.Log("Khong the di chuyen");
            return;
        }
        rb.linearVelocity = new Vector2(
          moveAmt.x * _runSpeed,
          rb.linearVelocity.y
      );
    }
    public void CheckHit()
    {
        sword.Hit();
    }
    private void Attack()
    {
        if (attackAction.triggered && !_isAttacking)
        {
            m_energy =Mathf.Max(0,m_energy-m_attackenergy);
            playerStatController.SetAbilityBar(m_ability / m_maxability);
            m_ability = Mathf.Min(m_ability+m_attackenergy, m_maxability);
            playerStatController.SetEnergyBar(m_energy / m_maxenergy);
            _isAttacking = true;
            _canMove = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetTrigger("Attack");
        }
    }
    public void TakeDamage(float dmg)
    {
        m_health = Mathf.Max(0, m_health - dmg);
        playerStatController.SetHPBar(m_health/ m_maxhealth);
        //animator.SetTrigger("TakeDmg");
        Debug.Log(gameObject.name + " da nhan " + dmg.ToString() + " dmg");
        if (m_health <= 0)
            Death();
    }
    private void Death()
    {
        if (m_health <= 0)
        {
            animator.SetTrigger("Death");
            _canMove = false;
        }
    }
    public void EnableMoving()
    {
        Debug.Log("EnableMoving CALLED");
        _canMove = true;
        _isAttacking = false;
    }
    public float getAttackDamage() { return this.m_attackDmg; }
    private void UpdateAnimation()
    {
        bool isRunning = _canMove&&Mathf.Abs(moveAmt.x) > 0.01f;
        animator.SetBool("isRunning", isRunning);
    }

    private void FlipCharacter()
    {
        if (moveAmt.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (moveAmt.x < 0 && _facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if (spriteRenderer == null) return;
        _facingRight = !_facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        swordCollider.gameObject.transform.localPosition = new Vector3(
            swordCollider.gameObject.transform.localPosition.x * -1,
            swordCollider.gameObject.transform.localPosition.y,
            swordCollider.gameObject.transform.localPosition.z
        );
        Vector2 off = swordCollider.offset;
        off.x = -off.x;
        swordCollider.offset = off;
    }
    public Vector2 getPositon()
    {
        return transform.position;
    }
}
