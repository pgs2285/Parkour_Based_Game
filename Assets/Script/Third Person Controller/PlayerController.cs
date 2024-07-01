using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
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
    
    public bool InAction { get; private set; } 
    public bool IsHanging { get; set; }

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
    private PhotonView _photonView;
    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
        
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

    }
    private void Update()
    {
        if (!_photonView.IsMine)
            return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
        
        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        desiredMoveDir = cameraController.PlanarRotation * moveInput;
        moveDir = desiredMoveDir;   
        if (!hasControl) return;

        if (IsHanging) return;

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

        if (moveAmount > 0 && moveDir.magnitude > 0.2f) // ledge�� ������ 90�� �̻��� �ƴϸ� �������� �ʰ� �ߴµ�, zero�� �Ǹ� �÷��̾ ȸ���ϴ� �����ϱ� ���� magnitude��
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
    
    public IEnumerator DoAction(string animName, MatchTargetParams matchParams = null, Quaternion targetRotation = new Quaternion(),
        bool rotate=false, float postDelay = 0f, bool mirror = false)
    {
        InAction = true;

        //�߷� �� �ݸ��� ������ ����� �ö��� ���ϹǷ�, �ϴ� �̰��� ��Ȱ��ȭ ���ִ� �ڵ�


        animator.SetBool("mirrorAction", mirror);
        //animator.CrossFadeInFixedTime(animName, 0.2f); // cross fade�� �ִϸ��̼��� �ް��ϰ� �ٲ�� ������� �ʰ� ������ �Լ��� ���� �ڿ����� �������, �ι��� �μ��� fade out �ð�
        CrossFadeAnimation(animName, 0.2f);
        yield return null; //  �� �������� �ѱ����ν� ��ȯ

        var animState = animator.GetNextAnimatorStateInfo(0); // 0�� ���̾��� ��ȯ������ ������.
        if(!animState.IsName(animName))
        {
            Debug.LogError("�ִϸ��̼��� �������� �ʴ´�.");
        }

        float rotateStartTime = (matchParams != null) ? matchParams.startTime : 0f;

        float timer = 0f;
        while(timer <= animState.length)
        { // �ִϸ��̼ǵ���
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;
            if(rotate && normalizedTime > rotateStartTime)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }//�ִϸ��̼� ���ൿ�� ������Ʈ

            if (matchParams!=null)
                MatchTarget(matchParams);

            if(animator.IsInTransition(0) && timer> 1.0f) // Vault�������� �پ�Ѱ� ���߿� �����ϴ� ��ȯ���϶� �߷��� �������� ����. ���� ��ȯ���� break�ϸ� �ȵǴ� 0.5���� �������� ��������
                break;

            yield return null;
        }

        yield return new WaitForSeconds(postDelay); // �ִϸ��̼��� 2���� ����� ��� ��Ʈ�ѷ� �ѱ������ �� ������

       
        InAction = false;
    }

    void MatchTarget(MatchTargetParams mp)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(mp.pos, transform.rotation, mp.bodyPart,
            new MatchTargetWeightMask(mp.posWeight, 0),// vector�� xyz�� 1�ΰ͸� ��ġ�� match��Ų��. rotation�� match�Ƚ�ų�Ŵ� 0
            mp.startTime, mp.targetTime);
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

    public void EnableCharacterController(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void ResetTargetRotation()
    {
        targetRotation = transform.rotation;
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
    [PunRPC]
    private void CrossFadeAnimation(string animName, float transitionDuration)
    {
        animator.CrossFadeInFixedTime(animName, transitionDuration);
    }


}

public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWeight;
    public float startTime;
    public float targetTime;
}
