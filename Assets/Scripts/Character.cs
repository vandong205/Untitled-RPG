using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputActionAsset inputActions;

    private InputAction m_moveAction;
    private Animator m_animator;
    private Vector2 m_moveAmt;
    private bool m_facingRight = true;
    private float _runSpeed = 5f;

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
        FlipCharacter();
    }

    private void FixedUpdate()
    {
        Running();
    }
    private void Running()
    {
        rb.linearVelocity = new Vector2(
          m_moveAmt.x * _runSpeed,
          rb.linearVelocity.y
      );
    }
    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(m_moveAmt.x) > 0.01f;
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
}
