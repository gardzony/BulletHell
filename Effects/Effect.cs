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