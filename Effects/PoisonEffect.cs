using System.Collections;
using UnityEngine;

public class PoisonEffect : Effect
{
    public float damagePerSecond;
    private ParticleSystem _poisonParticles;

    public override void Activate(GameObject target)
    {
        base.Activate(target);
        _poisonParticles = target.GetComponent<IEffectable>().PoisonParticleSystem;
        StartCoroutine(PoisonCoroutine(target, _poisonParticles));
    }

    public override bool UpdateEffect(float deltaTime)
    {
        return timeElapsed >= duration;
    }

    private IEnumerator PoisonCoroutine(GameObject target, ParticleSystem poisonParticles = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target.TryGetComponent<IDamageable>(out var damageTarget))
            {
                damageTarget.TakeDamage(damagePerSecond, G.DamageType.Poison);
                if (poisonParticles != null)
                {
                    _poisonParticles.Play();
                }
            }
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
            timeElapsed = elapsed;
        }
    }
}
