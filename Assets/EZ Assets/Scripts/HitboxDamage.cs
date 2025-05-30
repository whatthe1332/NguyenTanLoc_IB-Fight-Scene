using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damage = 10;
    public string hitAnimationName = "Stomach Hit";
    public GameObject hitEffectPrefab;

    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        HealthSystem health = other.GetComponent<HealthSystem>()
                            ?? other.GetComponentInParent<HealthSystem>()
                            ?? other.GetComponentInChildren<HealthSystem>();

        if (health != null && health.currentHealth > 0)
        {
            health.TakeDamage(damage, hitAnimationName);
            hasHit = true;

            if (hitEffectPrefab != null)
            {
                Vector3 effectPos = other.ClosestPoint(transform.position);
                Quaternion effectRot = Quaternion.Euler(90, 0, 0);
                Instantiate(hitEffectPrefab, effectPos, effectRot);
            }
        }
    }
}
