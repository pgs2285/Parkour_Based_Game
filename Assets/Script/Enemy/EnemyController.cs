using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables
    protected StateMachine<EnemyController> stateMachine;
    public StateMachine<EnemyController> StateMachine => stateMachine;
    public LayerMask targetMask;
    public Transform target;
    public float viewRadius;
    public float AttackRange;
    private FieldOfView _FOV;
    [HideInInspector]
    public PhotonView _photon;
    #endregion



    #region Unity Methods

    private void Awake()
    {
        _FOV = GetComponent<FieldOfView>();
        _photon = GetComponent<PhotonView>();
    }
    private void Start()
    {
        stateMachine = new StateMachine<EnemyController>(this, new IdleState());
        stateMachine.AddState(new MoveState());
    }
    private void Update()
    {
        target = _FOV.nearestTarget;
        stateMachine.Update(Time.deltaTime);    
    }
    #endregion



    #region Other Methods

    #endregion
}
