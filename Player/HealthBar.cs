using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    private float _maxHealth;
    private Player _player;

    private void Start()
    {
        _player = GameManager.Instance.Player;
        _maxHealth = _player.BaseHealth;
    }

    public void UpdateHealthBar(float currentHealth)
    {
        healthBar.fillAmount = currentHealth / _maxHealth;
    }
}
