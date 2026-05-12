using UnityEngine;
using System.Collections.Generic;

public class PathGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public bool anchorToBottomLeft = false; // Toggle to switch between Center-based and Corner-based
    
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        
        // Calculate the starting point. 
        // If anchored to Bottom Left, the GameObject's position IS the bottom left.
        // Otherwise, the GameObject's position is the center.
        Vector2 worldBottomLeft;
        if (anchorToBottomLeft)
        {
            worldBottomLeft = (Vector2)transform.position;
        }
        else
        {
            worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                
                // We use a slightly smaller radius (90% of nodeRadius) for the physics check.
                // This prevents "grazing" a wall from marking the whole node as unwalkable.
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius * 0.9f, unwalkableMask));
                
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX, percentY;

        if (anchorToBottomLeft)
        {
            percentX = (worldPosition.x - transform.position.x) / gridWorldSize.x;
            percentY = (worldPosition.y - transform.position.y) / gridWorldSize.y;
        }
        else
        {
            percentX = (worldPosition.x - transform.position.x) / gridWorldSize.x + 0.5f;
            percentY = (worldPosition.y - transform.position.y) / gridWorldSize.y + 0.5f;
        }

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        if (anchorToBottomLeft)
        {
            Gizmos.DrawWireCube((Vector2)transform.position + gridWorldSize / 2, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        }

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? new Color(1, 1, 1, 0.3f) : new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .05f));
            }
        }
    }
}
