using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Effect : MonoBehaviour
{
    public float duration;
    public float timeElapsed;

    public virtual void Activate(GameObject target)
    {
        timeElapsed = 0f;
    }
    public virtual bool UpdateEffect(float deltaTime)
    {
        timeElapsed += deltaTime;
        return timeElapsed >= duration;
    }
}

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

public class SpeedBoostEffect : Effect
{
    public float speedMultiplier;
    private IEffectable _effectTarget;
    
    public override void Activate(GameObject target)
    {
        if (target.TryGetComponent<IEffectable>(out _effectTarget))
        {
            _effectTarget.SpeedMultiplierChange(speedMultiplier, G.OperationType.Encreas);
        }
        timeElapsed = 0f;
    }

    public override bool UpdateEffect(float deltaTime)
    {
        timeElapsed += deltaTime;
        
        if (timeElapsed >= duration)
        {
            Debug.Log(timeElapsed + "vv" + duration);
            _effectTarget.SpeedMultiplierChange(speedMultiplier, G.OperationType.Decrease);
            return true;
        }
        
        return false;
    }
}