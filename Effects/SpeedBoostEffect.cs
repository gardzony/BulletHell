using UnityEngine;

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
