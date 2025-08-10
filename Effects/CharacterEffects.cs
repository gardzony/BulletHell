using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
    private List<Effect> _activeEffects = new List<Effect>();

    // ReSharper disable Unity.PerformanceAnalysis
    private void AddEffect<T>(Action<T> configure) where T : Effect
    {
        if (Time.timeScale == 0) return;
        T effect = gameObject.AddComponent<T>();
        configure(effect);
        effect.Activate(gameObject);
        _activeEffects.Add(effect);
    }

    public void StopAllEffectCoroutines()
    {
        foreach (var effect in _activeEffects)
        {
            effect.StopAllCoroutines();
        }
    }
    
    private void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            if (_activeEffects[i].UpdateEffect(Time.deltaTime))
            {
                Destroy(_activeEffects[i]);
                _activeEffects.RemoveAt(i);
            }
        }
    }

    public void ClearAllEffects()
    {
        StopAllEffectCoroutines();
        _activeEffects.Clear();
    }
    
    public void ApplyPoisonEffect(float damagePerSecond, float duration)
    {
        AddEffect<PoisonEffect>(effect =>
        {
            effect.duration = duration;
            effect.damagePerSecond = damagePerSecond;
        });
    }
    
    public void ApplySpeedBoostEffect(float multiplier, float duration)
    {
        AddEffect<SpeedBoostEffect>(effect =>
        {
            effect.duration = duration;
            effect.speedMultiplier = multiplier;
        });
    }
}