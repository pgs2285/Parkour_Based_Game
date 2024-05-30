using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] List<Neighbour> neighbours;

    private void Awake()
    { // is two Way 체크되어있으면 자동으로 반대쪽도 가능하게
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
    public Vector2 direction; // 이 디렉션으로 애니메이션이 달라짐
    public ConnectionType connectionType;
    public bool isTwoWay; // 갔으면 다시 돌아올수 있나
}
    

public enum ConnectionType
{
    Jump, Move
}