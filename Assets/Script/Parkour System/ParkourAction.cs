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

    public Quaternion TargetRotation { get; set; } // inspector���� �Ⱥ��̰�

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y; // ĳ���� �տ��ִ� ��ֹ��� ���� ����
        if (height < minHeight || height > maxHeight) // �ش� ���̰� �����ȿ� ���ٸ�
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal); // ���� ������ üũ�� �Ǿ��ִٸ� TargetRotation�� normal�� �ݴ�������� ������Ʈ
        return true;
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
}
