using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] List<Neighbour> neighbours;

    private void Awake()
    { // is two Way üũ�Ǿ������� �ڵ����� �ݴ��ʵ� �����ϰ�
        var twoWayNeighbours = neighbours.Where(n => n.isTwoWay);
        foreach (var neighbour in twoWayNeighbours)
        {
            neighbour.point?.CreateConnetion(this, -neighbour.direction, neighbour.connectionType, neighbour.isTwoWay);
        }

    }

    public void CreateConnetion(ClimbPoint point, Vector2 direction, ConnectionType connectionType,
        bool isTwoWay = true)
    {
        var neighbour = new Neighbour()
        {
            point = point,
            direction = direction,
            connectionType = connectionType,
            isTwoWay = isTwoWay
        };
        neighbours.Add(neighbour);

    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        foreach(var neighbour in neighbours)
        {
            if(neighbour.point != null)
            {
                Debug.DrawLine(transform.position, neighbour.point.transform.position, (neighbour.isTwoWay) ? Color.green : Color.red);
            }
        }
    }
}
[System.Serializable]
public class Neighbour
{
    public ClimbPoint point;
    public Vector2 direction; // �� �𷺼����� �ִϸ��̼��� �޶���
    public ConnectionType connectionType;
    public bool isTwoWay; // ������ �ٽ� ���ƿü� �ֳ�
}
    

public enum ConnectionType
{
    Jump, Move
}