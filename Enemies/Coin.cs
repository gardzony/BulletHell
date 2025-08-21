using UnityEngine;

public class Coin : MonoBehaviour
{
    public int CoinValue = 1;
    public GameObject Target;
    [SerializeField] float speed = 10;

    private void Update()
    {
        if (Target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                Target.transform.position,
                speed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Shop.Instance.IncreasePlayerMoney(CoinValue);
            ObjectPool.Instance.ReturnObjectToPool(gameObject);
        }
    }
}
