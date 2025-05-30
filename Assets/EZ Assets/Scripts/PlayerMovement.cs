using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float pushForce = 15f;
    public float smoothing = 0.1f;
    public FloatingJoystick joystick;

    [Header("Collision")]
    public LayerMask barrier;
    public float skinWidth = 0.1f;

    private Animator animator;
    private Rigidbody rb;
    private Vector3 smoothVelocity;
    private bool wasTouchingBarrier = false;
    private Vector3 storedPushDirection;

    [Header("Targeting")]
    public Transform targetEnemy;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (joystick == null)
        {
            joystick = FindObjectOfType<FloatingJoystick>();
        }
    }

    void Update()
    {
        if (joystick == null) return;
        Vector3 inputDirection = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);

        if (inputDirection.magnitude > 0.1f && !FloatingJoystick.isJumping)
        {
            animator.SetBool("IsMoving", true);

            if (targetEnemy == null)
                FindClosestEnemy();

            if (targetEnemy != null)
            {
                Vector3 lookDirection = targetEnemy.position - transform.position;
                lookDirection.y = 0;
                if (lookDirection.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(lookDirection),
                        Time.deltaTime * 10f
                    );
                }
            }
            else
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(inputDirection),
                    Time.deltaTime * 10f
                );
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);

            if (wasTouchingBarrier)
            {
                ApplyPushForce();
                wasTouchingBarrier = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (FloatingJoystick.isJumping) return;

        Vector3 moveDir = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);
        Vector3 currentVel = rb.velocity;

        if (moveDir.magnitude > 0.1f)
        {
            if (!Physics.SphereCast(transform.position, skinWidth, moveDir,
                out _, moveDir.magnitude * speed * Time.fixedDeltaTime, barrier))
            {
                Vector3 targetVel = moveDir * speed;
                targetVel.y = currentVel.y;
                rb.velocity = Vector3.SmoothDamp(currentVel, targetVel, ref smoothVelocity, smoothing);
            }
        }
        else
        {
            Vector3 targetVel = new Vector3(0, currentVel.y, 0);
            rb.velocity = Vector3.Lerp(currentVel, targetVel, smoothing);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & barrier) != 0)
        {
            wasTouchingBarrier = true;

            Vector3 inputDir = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);
            if (inputDir.magnitude > 0.1f)
            {
                storedPushDirection = -inputDir.normalized; 
            }
            else
            {
                storedPushDirection = (transform.position - collision.contacts[0].point).normalized;
                storedPushDirection.y = 0;
            }
        }
    }

    void ApplyPushForce()
    {
        if (storedPushDirection.sqrMagnitude > 0.001f)
        {
            rb.AddForce(storedPushDirection.normalized * pushForce, ForceMode.Impulse);
            animator.SetTrigger("Stomach Punch");
            storedPushDirection = Vector3.zero;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = wasTouchingBarrier ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skinWidth);
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform closest = null;
        foreach (var e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = e.transform;
            }
        }
        targetEnemy = closest;
    }
}
