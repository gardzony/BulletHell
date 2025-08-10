using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float maxDistance = 0.1f;

    private Vector3 _targetPosition;

    private void Start()
    {
        if (player == null || anchorPoint == null)
        {
            Debug.LogError("Player или AnchorPoint не назначены!");
            return;
        }

        transform.position = anchorPoint.position;
    }

    private void Update()
    {
        if (player == null || anchorPoint == null) return;

        _targetPosition = anchorPoint.position;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, followSpeed * Time.deltaTime);

        float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
        if (distanceToTarget > maxDistance)
        {
            Vector3 direction = (_targetPosition - transform.position).normalized;
            transform.position = _targetPosition - direction * maxDistance;
        }
    }
}