using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveNPC : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap floorMap;
    private NonPC nonPC;
    public List<NodeNPC> nodesReady;
    public List<Vector3Int> positionsTried;
    public List<Vector3Int> test;
    private bool testbool;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        nonPC = GetComponent<NonPC>();
        nodesReady = new List<NodeNPC>();
        positionsTried = new List<Vector3Int>();
        testbool = true;
    }

    void Update()
    {
        if (testbool)
        StartCoroutine("yes");
        testbool = false;
    }

    IEnumerator yes()
    {
        yield return new WaitForSeconds(1f);
        test = CalculateRoute(new Vector3Int(0, 0, 2));
        yield break;
    }

    public void MoveNPCToDest(Vector3Int destination)
    {
        List<Vector3Int> directions = CalculateRoute(destination);
    }

    List<Vector3Int> CalculateRoute(Vector3Int destination)
    {
        nodesReady.Clear();
        positionsTried.Clear();
        int totalDistance = Mathf.Abs(destination.x - nonPC.position.x) + Mathf.Abs(destination.y - nonPC.position.y);
        if (totalDistance == 0) return null;
        NodeNPC startNode = new NodeNPC(destination, nonPC.position, 0, totalDistance);
        nodesReady.Add(startNode);
        return BranchOut(startNode, destination);
    }

    List<Vector3Int> BranchOut(NodeNPC rootNode, Vector3Int destination)
    {
        nodesReady.Remove(rootNode);
        positionsTried.Add(rootNode.position);
        // Destination reached
        if (rootNode.path[rootNode.path.Count - 1] == destination) return rootNode.pathDirections;

        List<Vector3Int> allAdjacentDirections = new List<Vector3Int>(TileManager.cardinalDirections);
        foreach (Vector3Int direction in TileManager.cardinalDirections)
        {
            allAdjacentDirections.Add(direction + new Vector3Int(0, 0, 2));
        }
        foreach (Vector3Int direction in TileManager.cardinalDirections)
        {
            allAdjacentDirections.Add(direction + new Vector3Int(0, 0, -2));
        }
        foreach (Vector3Int direction in allAdjacentDirections)
        {
            Vector3Int newPosition = rootNode.position + direction;
            if (positionsTried.Contains(newPosition)) continue;
            if (tileManager.tilesStandable.Contains(newPosition))
            {
                NodeNPC node = new NodeNPC(rootNode, direction, destination);
                nodesReady.Add(node);
            }
        }
        if (nodesReady.Count == 0) return null;
        nodesReady.Sort((NodeA, NodeB) => NodeA.totalCost.CompareTo(NodeB.totalCost));
        return BranchOut(nodesReady[1], destination);
    }
}

public class NodeNPC
{
    public NodeNPC prevNode;
    public Vector3Int position;
    public List<Vector3Int> path;
    public List<Vector3Int> pathDirections;
    public int currentCost;
    public int futureCost;
    public int totalCost;

    public NodeNPC(NodeNPC prevNode, Vector3Int direction, Vector3Int destination)
    {
        this.prevNode = prevNode;
        position = prevNode.position + direction;
        path = this.prevNode.path;
        path.Add(position);
        pathDirections = this.prevNode.pathDirections;
        pathDirections.Add(direction);
        currentCost = prevNode.currentCost + 1;
        futureCost = Mathf.Abs(destination.x - position.x) + Mathf.Abs(destination.y - position.y);
        totalCost = currentCost + futureCost;
        // Debug.Log("Future Cost = " + futureCost);
    }

    public NodeNPC(Vector3Int destination, Vector3Int startingNodePosition, int startingCurrentCost, int startingFutureCost)
    {
        position = startingNodePosition;
        currentCost = startingCurrentCost;
        futureCost = startingFutureCost;
        path = new List<Vector3Int>() {position};
        pathDirections = new List<Vector3Int>();
    }
}
