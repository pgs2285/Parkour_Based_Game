using System.Collections;
using System.Collections.Generic;
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
    #endregion



    #region Unity Methods

    private void Start()
    {
        
    }
    private void Update()
    {
        stateMachine.Update(Time.deltaTime);    
    }
    #endregion



    #region Other Methods
    public Transform SearchEnemy()
    {
        target = null;

        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask); // 적을 찾음
        foreach(Collider target in targetInViewRadius)
        {

        }


        return target;
    }
    #endregion
}
