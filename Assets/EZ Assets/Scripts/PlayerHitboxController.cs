using UnityEngine;

public class PlayerHitboxController : MonoBehaviour
{
    public GameObject stomachHitbox;
    public GameObject headHitbox;
    public GameObject kidneyRHitbox;
    public GameObject kidneyLHitbox;
    public GameObject Jumgping1Hitbox;
    public GameObject Jumgping2Hitbox;
    public void EnableStomachHitbox() => stomachHitbox.SetActive(true);
    public void EnableHeadHitbox() => headHitbox.SetActive(true);
    public void EnableKidneyRHitbox() => kidneyRHitbox.SetActive(true);
    public void EnableKidneyLHitbox() => kidneyLHitbox.SetActive(true);
    public void EnableJumping1Hitbox() => Jumgping1Hitbox.SetActive(true);
    public void EnableJumping2Hitbox() => Jumgping2Hitbox.SetActive(true);

    public void DisableAllHitboxes()
    {
        stomachHitbox.SetActive(false);
        headHitbox.SetActive(false);
        kidneyRHitbox.SetActive(false);
        kidneyLHitbox.SetActive(false);
        Jumgping1Hitbox.SetActive(false);
        Jumgping2Hitbox.SetActive(false);
    }
}
