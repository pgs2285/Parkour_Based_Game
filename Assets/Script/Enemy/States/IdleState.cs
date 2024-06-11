using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
    private Animator animator;
    private CharacterController characterController;

    protected int hasMove = Animator.StringToHash("Move");
    protected int moveSpeed = Animator.StringToHash("MoveSpeed");

    public override void OnInitialiezed()
    {
        animator = context.GetComponent<Animator>();
        characterController = context.GetComponent<CharacterController>();
    }

    public override void OnEnter()
    {
        Debug.Log("Idle State");
        animator?.SetBool(hasMove, false);
        animator?.SetFloat(moveSpeed, 0.0f);

    }
    public override void Update(float deltaTime)
    {
        throw new System.NotImplementedException();
    }
}
