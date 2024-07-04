using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 5f;
    [SerializeField] float ledgeRayLength = 10f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask climbLedgeLayer;
    [SerializeField] float climbLedgeRayLength = 1.5f;
    [SerializeField] float ledgeHeightThreshold = 0.75f;

    private bool isPicking = false;

    public ObstacleHitData ObstacleCheck()
    {
        ObstacleHitData hitData = new ObstacleHitData();
        Vector3 forwardOrigin = transform.position + forwardRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, 
            out hitData.forwardHit, forwardRayLength, obstacleLayer); // 앞방향으로 forwardRayLength만큼 Ray를 쏜다. hitData.forwardHit에 저장.

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound)  ? Color.red : Color.green);
        if(hitData.forwardHitFound)
        {
            Vector3 heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down,
                out hitData.heightHit, heightRayLength, obstacleLayer); // 이전 충돌 지점에서 heightRayLength 높이만큼부터, 그 길이만큼의 빛을쏘고 충돌여부 판단
            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.green);
        }
        return hitData;
    }

    public bool ClimbLedgeCheck(Vector3 dir, out RaycastHit ledgeHit)
    {
        ledgeHit = new RaycastHit();
        if (dir == Vector3.zero)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 offset = new Vector3(0, 0.18f, 0);

        for(int i = 0; i < 10; i++) // 어느 높이까지 탐색할지. 단위는 offset (블럭크기가 offset보다 작으면 곤란할지도?)
        {
            Debug.DrawRay(origin + offset * i, dir);
            if (Physics.Raycast(origin + offset * i, dir, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer)) // ledge를 순차적으로 점점 높혀가며 가장 가까운 climbLedge발견
            {
                ledgeHit = hit;
                return true;
            }

        }
        return false;
    }

    public bool DropLedgeCheck(out RaycastHit ledgeHit)
    { // ledge 끝에서 매달리기가 가능한지 체크
        ledgeHit = new RaycastHit();
        var origin = transform.position + Vector3.down * 0.1f + transform.forward * 2f;
        if(Physics.Raycast(origin, -transform.forward, out RaycastHit hit, 3, climbLedgeLayer))
        {
            ledgeHit = hit;
            return true;
        }

        return false;
    }

    public bool LedgeCheck(Vector3 moveDir, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();

        if(moveDir == Vector3.zero) return false;

        float originOffset = 0.5f;
        var origin = transform.position + moveDir * originOffset + Vector3.up;

        if(PhysicsUtil.ThreeRaycasts(origin, Vector3.down, 0.25f, transform,
            out List<RaycastHit> hits, ledgeRayLength, obstacleLayer, true))
        {
            List<RaycastHit> validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList(); // 리스트에 있는것들을 대입하며 조건에 맞으면 선택

            if (validHits.Count > 0)
            {

                Vector3 surfaceRayOrigin = validHits[0].point;
                surfaceRayOrigin.y = transform.position.y - 0.1f;
                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2, obstacleLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);
                    // player 이동방향으로 앞으로 가서 player위치로
                    float height = transform.position.y - validHits[0].point.y; //  땅까지의 거리
                    ledgeData.angle = 
                        Vector3.Angle(transform.forward, surfaceHit.normal); // 표면의 normal과 플레이어의 forwawrd각도 측정
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;
                    return true;
                    
                }
            }
        }

        return false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Item"))
        {
            Debug.Log("Hit with Item");
            hit.gameObject.GetComponent<IWeapon>().onCollision();
            PickUpItem(hit.gameObject); // 게임 설계에 바뀌면 나중에 배열로 아이템 리스트 저장해도 될듯?
            
        }
    }
    
    #region Item Function

    public void PickUpItem(GameObject item)
    {
        isPicking = true;
        SetEquip(item, true);
    }
    void SetEquip(GameObject item, bool isEquip)
    {
        Collider[] itemCollider = item.GetComponents<Collider>();
        foreach (Collider col in itemCollider)
        {
            col.enabled = !isEquip;
        }
    }
    #endregion Item Function
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