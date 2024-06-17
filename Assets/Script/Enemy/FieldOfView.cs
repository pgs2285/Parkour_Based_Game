using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    
    #region Variables
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTarget = new List<Transform>();
    public Transform nearestTarget = null;
    public float distanceToTarget;
    #endregion Variables
    
    #region Methods

    void FindVisibleTarget()
    {
        visibleTarget.Clear();
        nearestTarget = null;
        distanceToTarget = 0f;

        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask); // overlap sphere에 충돌한 적들 찾기
        foreach (Collider target in targetInViewRadius)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(target.transform.position, transform.position);
                if (!Physics.Raycast(transform.position, target.transform.position, distToTarget, obstacleMask)) // Enemy와 target사이에 걸리는게 없다면(장애물이 없다면)
                {
                    visibleTarget.Add(target.transform);
                    if (nearestTarget == null || distanceToTarget > distToTarget)
                    {
                        nearestTarget = target.transform;
                        distanceToTarget = distToTarget;
                        Debug.Log(nearestTarget.name);
                    }
                }
            }
        }
        
    }
    
    #endregion Methods
    
    #region Unity Methods
    void Update()
    {
        FindVisibleTarget();
    }
    #endregion Unity Methods
}
