using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Parkour System/New Parkour System")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight; // �� ��ġ ���̿� ������ ����

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y; // ĳ���� �տ��ִ� ��ֹ��� ���� ����
        if (height < minHeight || height > maxHeight) // �ش� ���̰� �����ȿ� ���ٸ�
            return false;
        return true;
    }

    public string AnimName => animName;
}
