using System.Collections.Generic;
using UnityEngine;

public class PosManager : MonoBehaviour
{
    static PosManager instance;
    public static PosManager Inst { get { return instance; } }

    [Header("이동 불가능 지역의 레이어 마스크")]
    public LayerMask unwalkableMask; //지나갈 수 없는 지역을 나타내는 레이어마스크
    [Header("격자의 크기 : 맵을 덮도록 설정")]
    public Vector2 gridWorldSize;//격자의 전체 사이즈
    [Header("정점의 반지름")]
    public float nodeRadius; //노드의 반지름
    [HideInInspector]
    public int gridSizeX, gridSizeY; //격자의 밑변과 높이 크기

    float nodeDiameter; // 노드의 지름(격자 한칸의 변의 길이를 설정해줄 크기)

    public Node[,] nodeArray;
    Vector3 worldBottomLeft;

    private void Awake()
    {
        instance = this;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //(격자 월드 밑변 / 노드의 지름)으로 격자에 노드가 몇개나 들어갈 수 있는지 여부 계산.
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);//(격자 월드 높이 / 노드의 지름)으로 격자에 노드가 몇개나 들어갈 수 있는지 여부 계산.
        CreateGrid();
    }

    //Method : 맵에 격자 상 좌표를 지정하는 함수
    void CreateGrid()
    {
        nodeArray = new Node[gridSizeX, gridSizeY]; //start에서 계산한 크기만큼 노드 2차원 배열 생성
        worldBottomLeft = transform.position - Vector3.right * (gridWorldSize.x / 2) - Vector3.up * (gridWorldSize.y / 2); //격자에서 왼쪽 아래 꼭지점
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                nodeArray[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }

    //Method : 월드 공간 좌표를 격자 상 좌표로 변환 함수
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {

        float percentX = (worldPosition.x - worldBottomLeft.x) / gridWorldSize.x;
        float percentY = (worldPosition.y - worldBottomLeft.y) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        for (int a = 0; a < nodeArray.GetLength(0); a++)
        {
            for (int b = 0; b < nodeArray.GetLength(1); b++)
            {
                if (a == x && y == b)
                {
                    return nodeArray[x, y];
                }
            }
        }
        return null;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

    //    if (nodeArray != null)
    //    {
    //        foreach (Node n in nodeArray)
    //        {
    //            Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //            if (path != null)
    //            {
    //                if (path.Contains(n))
    //                {
    //                    Gizmos.color = Color.black;
    //                }
    //            }
    //            else
    //            {
    //            }
    //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));

    //        }
    //    }

    //}
}