using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] bool mountPoint;
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

    public Neighbour GetNeighbour(Vector2 direction)
    { // (0,1)을 예시로, up,right arrow를 누른것도 받아주어야함. 즉 완전히 똑같지 않아도 받아야 하므로 고려해서 처리
        Neighbour neighbour = null;
        if(direction.y != 0)
            neighbour = neighbours.FirstOrDefault(n=>n.direction.y == direction.y); // 요소를 찾으면 반환 없으면 null
        if(neighbour == null && direction.x != 0)
            neighbour = neighbours.FirstOrDefault(n=>n.direction.x == direction.x); 

        return neighbour;
        
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

    public bool MountPoint => mountPoint;
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