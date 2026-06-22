using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform graphics;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerBinds playerBinds;
    private PlayerAnimator _playerAnimator;
    public float speedBoost;
    public float sprintMultiplier = 2f;
    public float sprintDuration = 0.4f;
    [SerializeField] private float secondsBeforeDashing = 0.2f;
    private bool _isDashing = false;

    void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if(GamePause.IsPaused) return;
        if(_playerAnimator.CurrentState == PlayerState.Attack)
        {
            _rb.linearVelocity = new Vector3(0, 0, 0);
            return;
        }
        float xMovement = Input.GetAxisRaw("Horizontal");
        float yMovement = Input.GetAxisRaw("Vertical");

        bool isMoving = IsMoving();
        
        if (isMoving)
        {
            _playerAnimator.ChangeState(PlayerState.Move);
        }
        else
        {
            _playerAnimator.ChangeState(PlayerState.Idle);
        }

        Vector2 movement = new Vector2(xMovement, yMovement).normalized * speedBoost;
        _rb.linearVelocity = movement;

        // Спринт
        if (Input.GetKeyDown(playerBinds.dashBind) && !_isDashing)
        {
            StartCoroutine(Dash(sprintDuration, secondsBeforeDashing));
        }

        // Разворот персонажа
        if (xMovement > 0)
        {
            graphics.localScale = new Vector3(1, 1, 1);
        }
        else if (xMovement < 0)
        {
            graphics.localScale = new Vector3(-1, 1, 1);
        }
    }

    IEnumerator Dash(float sprintDuration, float secondsBeforeDashing)
    {
        _isDashing = true;
        _playerAnimator.ChangeState(PlayerState.Dash);

        // Ждём перед бустом, но следим за вводом
        float elapsed = 0f;
        while (elapsed < secondsBeforeDashing)
        {
            if (!IsMoving())
            {
                _isDashing = false;
                _playerAnimator.ChangeState(PlayerState.Idle);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        speedBoost *= sprintMultiplier;

        elapsed = 0f;
        while (elapsed < sprintDuration)
        {
            if (!IsMoving())
            {
                speedBoost /= sprintMultiplier;
                _isDashing = false;
                _playerAnimator.ChangeState(PlayerState.Idle);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        speedBoost /= sprintMultiplier;
        _isDashing = false;
        _playerAnimator.ChangeState(PlayerState.Idle);
    }

    private bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 
            || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0;
    }

}
