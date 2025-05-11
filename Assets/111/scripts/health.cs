using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    //public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        //if (healthSlider != null)
        //{
        //    healthSlider.value = currentHealth;
        //}
    }

    void Die()
    {
        Debug.Log("Player died");
    }
}