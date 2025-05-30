using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    public Transform player;
    public float attackRange = 0.6f;
    public float attackCooldown = 2f;
    public int damage = 3;
    public string[] attackAnimations;

    private Animator animator;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private HealthSystem playerHealth;
    private string lastUsedAnimation;
    public GameObject hitEffectPrefab;
    public bool isDead = false;
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        TryFindPlayerAgain();
    }

    void Update()
    {
        if (isDead) return;

        if (player == null || playerHealth == null || playerHealth.currentHealth <= 0)
        {
            TryFindPlayerAgain();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(player.position);
            }
            animator.SetBool("IsMoving", true);
        }
        else
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(transform.position);
            }
            animator.SetBool("IsMoving", false);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                string chosenAnimation = attackAnimations[Random.Range(0, attackAnimations.Length)];
                lastUsedAnimation = chosenAnimation;
                animator.SetTrigger(chosenAnimation);
                lastAttackTime = Time.time;

                if (playerHealth != null && playerHealth.currentHealth > 0)
                {
                    playerHealth.TakeDamage(damage, GetHitTypeFromAnimation(lastUsedAnimation));

                    if (hitEffectPrefab != null)
                    {
                        Vector3 hitPos = player.position + Vector3.up * 1f;
                        Quaternion rot = Quaternion.Euler(90, 0, 0);
                        Instantiate(hitEffectPrefab, hitPos, rot);
                    }
                }
            }
        }
    }

    private string GetHitTypeFromAnimation(string animation)
    {
        switch (animation)
        {
            case "Head Punch":
                return "Head Hit";
            case "Stomach Punch":
                return "Stomach Hit";
            case "Kidney Punch Left":
            case "Kidney Punch Right":
                return "Kidney Hit";
            default:
                return "Stomach Hit";
        }
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer.transform;
        playerHealth = newPlayer.GetComponent<HealthSystem>();
    }

    private void TryFindPlayerAgain()
    {
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            SetPlayer(foundPlayer);
        }
        else
        {
            player = null;
            playerHealth = null;
        }
    }
}
