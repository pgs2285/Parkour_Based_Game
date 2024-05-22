using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName =  "Parkour System/Custom Actions/New vault action")]
public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if (!base.CheckIfPossible(hitData, player))
            return false;
        var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point); // hit된 물체의 transform을 기준으로(즉 로컬좌표계) 왼쪽인지 오른쪽인지
        if(hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x >0) // player 기준 왼쪽에 있으면 mirror (앞 뒤 고려)
        {
            // Mirror Animation
            Mirror = true;
            matchBodyPart = AvatarTarget.RightHand;
        }
        else
        {
            Mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }
        return true;
    }


}
