using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private G.DamageType damageType = G.DamageType.Pure;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<IDamageable>();
        var playerEffects = collision.GetComponent<CharacterEffects>();
        playerEffects.ApplyPoisonEffect(1, 3);
        playerEffects.ApplySpeedBoostEffect(-0.3f, 1.5f);
        player.TakeDamage(damage, damageType);
    }
}