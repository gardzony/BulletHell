using UnityEngine;

public class ForceStaff : MonoBehaviour
{
    [SerializeField] private float pushDistance = 3f;
    [SerializeField] private float pushDuration = 0.2f;
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private TrailRenderer trail;

    private Player _player;
    private Rigidbody2D _rb;
    private float _cooldownTimer;
    private bool _isPushing = false;

    void Start()
    {
        _player = GetComponent<Player>();
        _rb = GetComponent<Rigidbody2D>();
        trail.emitting = false;
        _cooldownTimer = 0f;
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _cooldownTimer <= 0f && !_isPushing)
        {
            StartPush();
            _player.SetPlayerImmortality();
        }
    }

    private void StartPush()
    {
        _isPushing = true;
        _cooldownTimer = cooldown;
        
        Vector2 pushDirection = _rb.linearVelocity.normalized;
        if (pushDirection == Vector2.zero)
        {
            pushDirection = _player.GetLastMoveDirection();
        }
    
        Vector2 targetPosition = (Vector2)transform.position + pushDirection * pushDistance;
        
        if (trail != null)
        {
            trail.emitting = true;
        }
        StartCoroutine(PushCoroutine(targetPosition));
    }

    private System.Collections.IEnumerator PushCoroutine(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        while (elapsedTime < pushDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / pushDuration;
            _rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }
        
        _rb.MovePosition(targetPosition);
        
        _isPushing = false;
        if (trail != null)
        {
            trail.emitting = false;
        }
    }
}