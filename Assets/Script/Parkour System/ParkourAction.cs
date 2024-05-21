using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Parkour System/New Parkour System")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight; // 두 수치 사이에 있으면 실행

    [SerializeField] bool rotateToObstacle;

    public Quaternion TargetRotation { get; set; } // inspector에서 안보이게

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y; // 캐릭터 앞에있는 장애물의 높이 측정
        if (height < minHeight || height > maxHeight) // 해당 높이가 범위안에 없다면
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal); // 만약 변수가 체크가 되어있다면 TargetRotation을 normal의 반대방향으로 업데이트
        return true;
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
}
