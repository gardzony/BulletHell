using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected string targetTag;
    [SerializeField] protected float attackRange = 5f;

    public G.Rarity WeaponRarity;
    public Sprite WeaponImage;

    protected virtual List<Collider2D> FindEnemiesInAttackRange()
    {
        var hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy")).ToList();

        return hitEnemies;
    }

    protected virtual void Attack(GameObject target) { }

    public virtual void FlipWeaponRotation() { }
}
