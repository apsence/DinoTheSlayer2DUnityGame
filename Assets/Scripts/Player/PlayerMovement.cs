using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float speedBoost;
    public float speedMulriplier;
    private Animator _animator;
    public Camera camera;
    public float sprintMulriplier = 2f;
    public float sprintDuration = 0.4f;
    private bool _isSprinting = false;
    private Scale _scale;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Movement();
    }

    void FixedUpdate()
    {

    }

    private void Movement()
    {
        float xMovement = Input.GetAxisRaw("Horizontal");
        float yMovement = Input.GetAxisRaw("Vertical");

        bool isMoving = Mathf.Abs(xMovement) > 0 || Mathf.Abs(yMovement) > 0;
        _animator.SetBool("isMoving", isMoving);

        Vector2 movement = new Vector2(xMovement, yMovement).normalized * speedBoost * speedMulriplier;
        Vector2 newPosition = _rigidbody.position + movement * Time.fixedDeltaTime;

        _rigidbody.linearVelocity = movement; // физическое перемещение с учётом столкновений


        // Спринт
        if (Input.GetKeyDown(KeyCode.Space) && !_isSprinting)
        {
            StartCoroutine(Sprint(sprintDuration));
        }


        // Разворот персонажа
        if (Input.GetKey(KeyCode.A))
        {
            _spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _spriteRenderer.flipX = false;
        }
    }

    IEnumerator Sprint(float sprintDuration)
    {
        _isSprinting = true;
        speedBoost *= sprintMulriplier;

        yield return new WaitForSeconds(sprintDuration);

        speedBoost /= sprintMulriplier;
        _isSprinting = false;
    }

}
