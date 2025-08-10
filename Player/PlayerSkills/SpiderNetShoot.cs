using UnityEngine;
using System.Collections.Generic;

public class SpiderNetShoot : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform character;
    [SerializeField] private float detectionAngle = 70f;
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private float slowCoeff = -0.3f;
    [SerializeField] private ParticleSystem attackEffect;

    private List<GameObject> _enemiesInRange = new List<GameObject>();

    private void Start()
    {
        if (attackEffect != null)
        {
            attackEffect.Stop();
            attackEffect.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            _enemiesInRange = StopMouseDrag();
            if (_enemiesInRange.Count > 0) SlowEffectToEnemy(_enemiesInRange);
            PlayAttackEffect();
        }
    }

    private List<GameObject> StopMouseDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 characterPosition = character.position;
        Vector2 directionToMouse = (mousePosition - characterPosition).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(characterPosition, detectionRadius, enemyLayer);
        List<GameObject> enemiesInQuarter = new List<GameObject>();

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Vector2 enemyDirection = ((Vector2)hit.transform.position - characterPosition).normalized;
                float angle = Vector2.Angle(directionToMouse, enemyDirection);

                if (angle <= detectionAngle / 2f)
                {
                    float dotProduct = Vector2.Dot(directionToMouse, enemyDirection);
                    if (dotProduct > 0)
                    {
                        enemiesInQuarter.Add(hit.gameObject);
                        Debug.Log($"Найден враг: {hit.gameObject.name}");
                    }
                }
            }
        }
        return enemiesInQuarter;
    }

    private void SlowEffectToEnemy(List<GameObject> enemies)
    {
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<CharacterEffects>().ApplySpeedBoostEffect(slowCoeff, slowDuration);
        }
    }

    private void PlayAttackEffect()
    {
        if (attackEffect == null)
        {
            Debug.LogWarning("Particle System не назначена в инспекторе!");
            return;
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 characterPosition = character.position;
        Vector2 directionToMouse = (mousePosition - characterPosition).normalized;
        ParticleSystem effect = Instantiate(
            attackEffect,
            characterPosition,
            Quaternion.identity
        );
        effect.transform.position = characterPosition;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        effect.transform.rotation = Quaternion.Euler(-angle, 90f, 0);

        effect.gameObject.SetActive(true);
        effect.Play();
        
        Destroy(effect.gameObject, 1);
    }

    private void OnDrawGizmos()
    {
        if (character == null) return;

        Vector2 characterPosition = character.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - characterPosition).normalized;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(characterPosition, detectionRadius);

        // Рисуем границы четверти круга
        Gizmos.color = Color.yellow;
        Quaternion leftRotation = Quaternion.AngleAxis(detectionAngle/2f, Vector3.forward);
        Quaternion rightRotation = Quaternion.AngleAxis(-detectionAngle/2f, Vector3.forward);
        
        Vector2 leftBound = leftRotation * directionToMouse * detectionRadius;
        Vector2 rightBound = rightRotation * directionToMouse * detectionRadius;

        Gizmos.DrawLine(characterPosition, characterPosition + leftBound);
        Gizmos.DrawLine(characterPosition, characterPosition + rightBound);
    }
}