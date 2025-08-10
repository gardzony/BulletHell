using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Bonus", menuName = "Game/Bonuses/Speed Bonus")]

public class SpeedBonus : Bonus
{
    [SerializeField] private float speedBuff;

    public override void Activate()
    {
        BuffManager.Instance.SpeedBuff += speedBuff;
    }

    public override void Deactivate()
    {
        BuffManager.Instance.SpeedBuff -= speedBuff;
    }
}
