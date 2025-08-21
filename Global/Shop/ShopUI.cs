using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] CardUI[] cardsToSpawn;
    [SerializeField] GameObject shopBoard;
    private EnemySpawner _spawner;
    private bool isActive;
    Coroutine _currentCoroutine;

    async void Start()
    {
        await Task.Delay(1000);
        _spawner = GameManager.Instance.EnemySpawner;
        _spawner.OnWaveCompleted += CardsSpawn;
    }

    private void Update()
    {
        if(isActive && !shopBoard.activeInHierarchy)
        {
            isActive = false;
            foreach (var card in cardsToSpawn)
            {
                card.gameObject.SetActive(false);
            }
        }
    }
    public void CardsSpawn(int costil)
    {
        isActive = true;
        _currentCoroutine = StartCoroutine(ProcessCardsSpawn());
    }

    private IEnumerator ProcessCardsSpawn()
    {
        List<G.Rarity> rarityList = new List<G.Rarity>();

        var items = Shop.Instance.GetRandomCardsItems(out rarityList);

        for (int i = 0; i < cardsToSpawn.Length; i++)
        {
            cardsToSpawn[i].SetCardAttributes(items[i].Icon, items[i].Name, items[i].Description, items[i].Price, rarityList[i]);
            cardsToSpawn[i].gameObject.SetActive(true);
            yield return cardsToSpawn[i].Show();
        }
    }
    public void ShopReroll()
    {
        if (isActive)
        {
            StopCoroutine(_currentCoroutine);
            for (int i = 0;i < cardsToSpawn.Length; i++)
            {
                cardsToSpawn[i].Hide();
            }
            _currentCoroutine = StartCoroutine(ProcessCardsSpawn());
        }
    }
}
