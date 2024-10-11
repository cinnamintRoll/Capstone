using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;
    public Slider[] lifeSliders;  // Two sliders representing the lives
    public TMP_Text killsRemainingText;

    public int killsToRestoreLife = 5;  // Number of kills required to restore one life
    private int currentKillCount;
    [SerializeField] private GameMenu gameMenu;
    private float maxHealthPerLife = 100f;  // Each life corresponds to 100% health
    private float currentHealth;

    [SerializeField] private Animator DamageAnimator;
    void Start()
    {
        currentLives = maxLives;
        currentHealth = maxHealthPerLife;
        UpdateSliders();
        UpdateKillCountText();
    }

    void UpdateSliders()
    {
        for (int i = 0; i < lifeSliders.Length; i++)
        {
            if (i < currentLives - 1)
            {
                lifeSliders[i].value = 1;  // Full life
            }
            else if (i == currentLives - 1)
            {
                lifeSliders[i].value = currentHealth / maxHealthPerLife;  // Current life fills based on health
            }
            else
            {
                lifeSliders[i].value = 0;  // No life
            }
        }
    }

    void UpdateKillCountText()
    {
        killsRemainingText.text = $"{killsToRestoreLife - currentKillCount}";
    }

    // Damage instantly removes a full life
    public void TakeDamage(float damageAmount)
    {
        if (currentLives > 0)
        {
            currentLives--;
            currentHealth = 0;  // Instant full bar removal on damage
            currentKillCount = 0;  // Reset kill progress on damage
        }

        if (currentLives <= 0)
        {
            // Handle death logic
            Debug.Log("Player is dead");
            PlayerDeath();
        }

        DamageAnimator.SetTrigger("TakeDamage");

        UpdateKillCountText();
        UpdateSliders();
    }

    // Killing enemies slowly fills up the current life
    public void KillEnemy()
    {
        if (currentLives < maxLives)
        {
            currentKillCount++;
            UpdateKillCountText();

            // Gradually fill up the current health bar based on kills if we're working on restoring a life
            if (currentKillCount < killsToRestoreLife)
            {
                currentHealth = (float)currentKillCount / killsToRestoreLife * maxHealthPerLife;
            }
            else
            {
                RestoreLife();  // Restore a full life once enough kills are made
            }

            UpdateSliders();
        }
    }

    // Restores one full life when enough enemies are killed
    void RestoreLife()
    {
        currentLives++;
        currentKillCount = 0;
        currentHealth = 0;  // Set the newly restored life to full health
        UpdateSliders();
        UpdateKillCountText();
    }

    // Debug methods for the Inspector buttons
    public void DebugTakeDamage()
    {
        TakeDamage(100f);  // Arbitrary damage for full bar removal
    }

    public void DebugKillEnemy()
    {
        KillEnemy();  // Simulate killing an enemy
    }

    public void PlayerDeath()
    {
        gameMenu.TriggerDeathMenu();
    }
}
