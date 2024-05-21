using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Parkour System/New Parkour System")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight; // �� ��ġ ���̿� ������ ����

    [SerializeField] bool rotateToObstacle;


    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart; // AvatarTarget�� Root, Body, Left Foot, Right Foot, Left Hand, Right Hand�� ������ enum
    [SerializeField] float matchStartTime; // ���� �� �����ϴ� �ð�
    [SerializeField] float matchTargetTime; // �ش� ������ ��� ���ϴ� �ð�

    public Quaternion TargetRotation { get; set; } // inspector���� �Ⱥ��̰�
    public Vector3 MatchPos { get; set; } // Ÿ�� ��ġ ����

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y; // ĳ���� �տ��ִ� ��ֹ��� ���� ����
        if (height < minHeight || height > maxHeight) // �ش� ���̰� �����ȿ� ���ٸ�
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal); // ���� ������ üũ�� �Ǿ��ִٸ� TargetRotation�� normal�� �ݴ�������� ������Ʈ
        
        if(enableTargetMatching)
        {
            MatchPos = hitData.heightHit.point;
        }

        return true;
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart; 
    public float MatchStartTime => matchStartTime; 
    public float MatchTargetTime => matchTargetTime; 
}
