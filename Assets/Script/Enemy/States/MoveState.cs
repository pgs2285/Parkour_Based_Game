using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MoveState : State<EnemyController>
{
    
    #region Variables

    private Animator _animator;
    private CharacterController _characterController;
    private UnityEngine.AI.NavMeshAgent _agent;

    private int moveHash = Animator.StringToHash("Move");
    private int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private FieldOfView _FOV;
    #endregion Variables

    #region Methods

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _characterController = context.GetComponent<CharacterController>();
        _agent = context.GetComponent<UnityEngine.AI.NavMeshAgent>();
        _FOV = context.GetComponent<FieldOfView>();

    }

    public override void OnEnter()
    {
        _agent?.SetDestination(context.target.position);
        _animator?.SetBool(moveHash, true);
        Debug.Log("MoveState");
    }
    public override void Update(float deltaTime)
    {
        Transform enemy = _FOV.nearestTarget;
        
        if (enemy)
        {
            _agent.SetDestination(enemy.transform.position);
            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _characterController.Move(_agent.velocity * deltaTime);
                _animator.SetFloat(moveSpeedHash, _agent.velocity.magnitude / _agent.speed, 1f, deltaTime);
                
                return;
            }
        }
        stateMachine.ChangeState<IdleState>();
    }

    public override void OnExit()
    {
        _animator.SetBool(moveHash, false);
        _agent.ResetPath();
    }
    #endregion Methods
}
