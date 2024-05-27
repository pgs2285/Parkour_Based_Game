using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    
    bool isGrounded;
    bool hasControl = true;

    Vector3 desiredMoveDir;
    Vector3 moveDir;
    Vector3 velocity;

    public bool IsOnLedge { get; set; }
    public LedgeData LedgeData { get; set; }

    float ySpeed;
    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    EnvironmentScanner environmentScanner;
    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }


    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
        
        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        desiredMoveDir = cameraController.PlanarRotation * moveInput;
        moveDir = desiredMoveDir;   
        if (!hasControl) return;

        velocity = Vector3.zero;

        GroundCheck();
        animator.SetBool("IsGrounded", isGrounded);
        if(isGrounded)
        {
            velocity = desiredMoveDir * moveSpeed;
            ySpeed = -0.5f;
            IsOnLedge = environmentScanner.LedgeCheck(desiredMoveDir, out LedgeData ledgeData);
            if(IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }
            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            velocity = transform.forward * moveSpeed / 2;
        }

        velocity.y = ySpeed;
        characterController.Move(velocity* Time.deltaTime);

        if (moveAmount > 0 && moveDir.magnitude > 0.2f) // ledge의 각도가 90도 이상이 아니면 떨어지지 않게 했는데, zero가 되면 플레이어가 회전하니 방지하기 위한 magnitude값
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);



    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    void LedgeMovement()
    {
        float angle = Vector3.Angle(LedgeData.surfaceHit.normal, desiredMoveDir);
        if(Vector3.Angle(desiredMoveDir, transform.forward) >= 80)
        {
            velocity = Vector3.zero;
            return;
        }
        if(angle < 90)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if(!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    public bool HasControl
    {
        get => hasControl;
        set => hasControl = value;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;

}
