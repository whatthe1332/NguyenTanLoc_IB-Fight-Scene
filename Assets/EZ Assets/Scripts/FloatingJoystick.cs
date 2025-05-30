using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class FloatingJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;
    public float maxDistance = 100f;
    public Vector2 Direction { get; private set; }

    private Vector2 initialPosition;
    private Vector2 dragStartPos;
    private float tapThreshold = 5f;
    private float pointerDownTime; 

    public Animator playerAnimator;
    public Rigidbody playerRigidbody;
    public float jumpForce = 10f;
    public float airBoostForce = 15f;
    public float bigJumpForce = 20f;

    private bool isAttacking = false;
    private bool hasAirBoosted = false;
    private bool isBigJumping = false;

    public static bool isJumping = false; 

    void Start()
    {
        initialPosition = background.anchoredPosition;

        if (playerAnimator == null || playerRigidbody == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (playerAnimator == null)
                    playerAnimator = player.GetComponent<Animator>();
                if (playerRigidbody == null)
                    playerRigidbody = player.GetComponent<Rigidbody>();
            }
        }
    }

    void Update()
    {
        if (IsGrounded())
        {
            hasAirBoosted = false;
            isBigJumping = false;
            isJumping = false; 
        }
        if (playerRigidbody == null || playerAnimator == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                SetPlayer(player);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;

        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos))
        {
            background.anchoredPosition = pos;
            dragStartPos = pos;
        }

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsGrounded()) return;
        if (isBigJumping) return;

        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos))
        {
            pos -= background.anchoredPosition;
            pos = Vector2.ClampMagnitude(pos, maxDistance);
            handle.anchoredPosition = pos;
            Direction = pos / maxDistance;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = initialPosition;
        handle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;

        if (isAttacking && IsGrounded()) return;
        if (!IsGrounded() && hasAirBoosted) return;

        float holdDuration = Time.time - pointerDownTime;

        if (holdDuration > 0.15f) return;

        Vector2 releasePos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out releasePos))
        {
            Vector2 swipe = releasePos - dragStartPos;

            if (swipe.magnitude < tapThreshold)
            {
                PlayAttack("Head Punch", 0.6f);
            }
            else
            {
                float angle = Vector2.SignedAngle(Vector2.up, swipe);

                if (IsGrounded())
                {
                    if (angle >= -45f && angle <= 45f)
                    {
                        Jump();
                    }
                    else if (angle > 45f && angle < 135f)
                    {
                        PlayAttack("Kidney Punch Right", 0.6f);
                    }
                    else if (angle < -45f && angle > -135f)
                    {
                        PlayAttack("Kidney Punch Left", 0.6f);
                    }
                    else
                    {
                        PlayAttack("Stomach Punch", 0.6f);
                    }
                }
                else
                {
                    Vector3 boostDir = Vector3.zero;
                    string jumpAnim = "";
                    float force = airBoostForce;

                    if (angle >= -45f && angle <= 45f)
                    {
                        boostDir = playerRigidbody.transform.forward + Vector3.down * 0.5f;
                        jumpAnim = "Big Jump";
                        force = bigJumpForce;
                    }
                    else if (angle > 135f || angle < -135f)
                    {
                        boostDir = playerRigidbody.transform.forward + Vector3.down * 0.5f;
                        jumpAnim = "Jumping_1";
                        force = airBoostForce * 1.5f;
                    }
                    else
                    {
                        boostDir = playerRigidbody.transform.forward + Vector3.down * 0.5f;
                        jumpAnim = "Jumping_2";
                        force = airBoostForce * 0.8f;
                    }

                    AirBoost(boostDir.normalized, jumpAnim, force);
                }
            }
        }
    }

    private void PlayAttack(string triggerName, float duration)
    {
        if (playerAnimator != null && !isAttacking)
        {
            isAttacking = true;
            playerAnimator.SetTrigger(triggerName);
            StartCoroutine(ResetAttackAfterDelay(duration));
        }
    }

    private void Jump()
    {
        if (playerRigidbody != null && IsGrounded())
        {
            Vector3 velocity = playerRigidbody.velocity;
            velocity.y = 0;
            playerRigidbody.velocity = velocity;

            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Jump");
            }

            isJumping = true;
            isAttacking = true;
            StartCoroutine(ResetAttackAfterDelay(0.6f));
        }
    }

    private void AirBoost(Vector3 direction, string animationTrigger, float force)
    {
        if (playerRigidbody != null && !hasAirBoosted)
        {
            hasAirBoosted = true;

            if (animationTrigger == "Big Jump")
                isBigJumping = true;

            playerRigidbody.AddForce(direction * force, ForceMode.Impulse);

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(animationTrigger);
            }

            isJumping = true; 
            isAttacking = true;
            StartCoroutine(ResetAttackAfterDelay(0.6f));
        }
    }

    private bool IsGrounded()
    {
        return playerRigidbody != null &&
               playerRigidbody.gameObject != null &&
               Physics.Raycast(playerRigidbody.position, Vector3.down, 1.1f);
    }

    private IEnumerator ResetAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    public void SetPlayer(GameObject player)
    {
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
    }
}