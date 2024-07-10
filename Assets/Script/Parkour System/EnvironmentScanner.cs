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
            out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.green);
        if(hitData.forwardHitFound)
        {
            Vector3 heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down,
                out hitData.heightHit, heightRayLength, obstacleLayer);
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

        for(int i = 0; i < 10; i++)
        {
            Debug.DrawRay(origin + offset * i, dir);
            if (Physics.Raycast(origin + offset * i, dir, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer))
            {
                ledgeHit = hit;
                return true;
            }
        }
        return false;
    }

    public bool DropLedgeCheck(out RaycastHit ledgeHit)
    {
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
            List<RaycastHit> validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList();

            if (validHits.Count > 0)
            {
                Vector3 surfaceRayOrigin = validHits[0].point;
                surfaceRayOrigin.y = transform.position.y - 0.1f;
                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2, obstacleLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);
                    float height = transform.position.y - validHits[0].point.y;
                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
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
