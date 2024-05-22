using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Parkour System/New Parkour System")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] string obstacleTag; // 같은 높이의 장애물이라도 태그의 여부에 따라서 모션 다르게

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight; // 두 수치 사이에 있으면 실행
    
    [SerializeField] bool rotateToObstacle;
    [SerializeField] float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart; // AvatarTarget은 Root, Body, Left Foot, Right Foot, Left Hand, Right Hand로 구성된 enum
    [SerializeField] float matchStartTime; // 점프 등 시작하는 시간
    [SerializeField] float matchTargetTime; // 해당 부위가 닿길 원하는 시간
    [SerializeField] Vector3 matchPosWeight = new Vector3(0, 1, 0);

    public Quaternion TargetRotation { get; set; } // inspector에서 안보이게
    public Vector3 MatchPos { get; set; } // 타겟 위치 지정
    public bool Mirror { get; set; }

    public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if(!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
        { // 태그가 있는데, 장애물 태그와 동일하지 않다면
            return false;
        }

        float height = hitData.heightHit.point.y - player.position.y; // 캐릭터 앞에있는 장애물의 높이 측정
        if (height < minHeight || height > maxHeight) // 해당 높이가 범위안에 없다면
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal); // 만약 변수가 체크가 되어있다면 TargetRotation을 normal의 반대방향으로 업데이트
        
        if(enableTargetMatching)
        {
            MatchPos = hitData.heightHit.point;
        }

        return true;
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart; 
    public float MatchStartTime => matchStartTime; 
    public float MatchTargetTime => matchTargetTime; 
    public Vector3 MatchPosWeight => matchPosWeight;
}
