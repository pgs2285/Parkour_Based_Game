using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
    private Animator animator;
    private CharacterController characterController;
    private FieldOfView _FOV;

    protected int hasMove = Animator.StringToHash("Move");
    protected int moveSpeed = Animator.StringToHash("MoveSpeed");

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        characterController = context.GetComponent<CharacterController>();
        _FOV = context.GetComponent<FieldOfView>();
    }

    public override void OnEnter()
    {
        Debug.Log("Idle State");
        animator?.SetBool(hasMove, false);
        animator?.SetFloat(moveSpeed, 0.0f);

    }
    public override void Update(float deltaTime)
    {
        Debug.Log(animator + " " + characterController + " " + _FOV);
        Transform enemy = _FOV.nearestTarget;
        if (enemy)
        {
            stateMachine.ChangeState<MoveState>();
        }

    }
}
