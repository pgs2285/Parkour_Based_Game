using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil
{
    public static bool ThreeRaycasts(Vector3 origin, Vector3 dir, float spacing, Transform transform,
        out List<RaycastHit> hits, float distance, LayerMask layer, bool debugDraw = false)
    {
        // 1. 먼저 중앙에서 내려오게 체크
        bool centerHitFound = Physics.Raycast(origin, Vector3.down, out RaycastHit centerHit, distance, layer);
        // 2. 왼쪽 체크
        bool leftHitFound = Physics.Raycast(origin - transform.right * spacing, Vector3.down, out RaycastHit leftHit,
            distance, layer);
        // 3. 오른쪽 체크
        bool rightHitFound = Physics.Raycast(origin + transform.right * spacing, Vector3.down, out RaycastHit rightHit,
            distance, layer);
        
        hits = new List<RaycastHit>() { centerHit, leftHit, rightHit };
        
        bool hitFound = centerHitFound || leftHitFound || rightHitFound;
        if (hitFound && debugDraw)
        {
            Debug.DrawLine(origin, centerHit.point, Color.red);
            Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
            Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        }
        return hitFound;
    }

}
