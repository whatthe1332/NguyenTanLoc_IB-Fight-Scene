using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public int maxHealth = 100;
    public int currentHealth;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (slider == null)
        {
            // Tự tìm trong con (kể cả bị ẩn)
            slider = GetComponentInChildren<Slider>(true);
        }

        currentHealth = maxHealth;
        isDead = false;

        if (slider != null)
        {
            slider.gameObject.SetActive(true);
            UpdateHealthBar();
        }
    }

    public void UpdateHealthBar()
    {
        if (slider != null)
        {
            slider.value = (float)currentHealth / maxHealth;
        }
    }

    public void TakeDamage(int damage, string animationName)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth > 0)
        {
            if (animator != null)
            {
                animator.SetTrigger(animationName);
            }
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        var agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Knocked Out");
        }

        if (slider != null)
        {
            slider.gameObject.SetActive(false);
        }

        var enemyAI = GetComponent<EnemyAIController>();
        if (enemyAI != null)
        {
            enemyAI.isDead = true;
        }

        var tagToCheck = tag;
        StartCoroutine(DelayedDeath(tagToCheck));
    }

    IEnumerator DelayedDeath(string teamTag)
    {
        yield return new WaitForSeconds(2f);

        if (teamTag == "Player")
        {
            GameManager.Instance.OnPlayerDead();
        }
        else if (teamTag == "Enemy")
        {
            GameManager.Instance.OnEnemyDead();
        }

        Destroy(gameObject);
    }
}
