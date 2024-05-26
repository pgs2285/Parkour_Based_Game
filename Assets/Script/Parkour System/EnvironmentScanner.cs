using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 5f;
    [SerializeField] float ledgeRayLength = 10f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float ledgeHeightThreshold = 0.75f;

    public ObstacleHitData ObstacleCheck()
    {
        ObstacleHitData hitData = new ObstacleHitData();
        Vector3 forwardOrigin = transform.position + forwardRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, 
            out hitData.forwardHit, forwardRayLength, obstacleLayer); // �չ������� forwardRayLength��ŭ Ray�� ���. hitData.forwardHit�� ����.

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound)  ? Color.red : Color.green);
        if(hitData.forwardHitFound)
        {
            Vector3 heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down,
                out hitData.heightHit, heightRayLength, obstacleLayer); // ���� �浹 �������� heightRayLength ���̸�ŭ����, �� ���̸�ŭ�� ������� �浹���� �Ǵ�
            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.green);
        }
        return hitData;
    }


    public bool LedgeCheck(Vector3 moveDir, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();

        if(moveDir == Vector3.zero) return false;

        float originOffset = 0.5f;
        var origin = transform.position + moveDir * originOffset + Vector3.up;

        if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleLayer))
        {
            
            var surfaceRayOrigin = transform.position + moveDir - new Vector3(0, 0.1f, 0);
            if (Physics.Raycast(surfaceRayOrigin, -moveDir, out RaycastHit surfaceHit, 2, obstacleLayer))
            {

                float height = transform.position.y - hit.point.y; //  �������� �Ÿ�
                if (height > ledgeHeightThreshold) // �Ӱ谪�� ������ true
                {
                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal); // ǥ���� normal�� �÷��̾��� forwawrd���� ����
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;
                    return true;
                }
            }
        }

        return false;
    }
}



public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

public struct LedgeData
{
    public float height;
    public float angle;
    public RaycastHit surfaceHit;
}