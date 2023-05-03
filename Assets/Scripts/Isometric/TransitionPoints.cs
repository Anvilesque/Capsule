using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoints
{
    public static Dictionary<Vector3Int, Vector3Int> TransitionStartDests = new Dictionary<Vector3Int, Vector3Int>()
    {
        {new Vector3Int(), new Vector3Int()},
        
        {new Vector3Int(3, -8, 6), new Vector3Int(26, -5, 0)},
        {new Vector3Int(3, -9, 6), new Vector3Int(26, -5, 0)},
        {new Vector3Int(3, -10, 6), new Vector3Int(26, -5, 0)},
        {new Vector3Int(26, -5, 0), new Vector3Int(3, -9, 6)},

        {new Vector3Int(-12, -8, 0), new Vector3Int(-32, -8, 0)},
        {new Vector3Int(-12, -9, 0), new Vector3Int(-32, -9, 0)},
        {new Vector3Int(-12, -10, 0), new Vector3Int(-32, -10, 0)},
        {new Vector3Int(-32, -8, 0), new Vector3Int(-12, -8, 0)},
        {new Vector3Int(-32, -9, 0), new Vector3Int(-12, -9, 0)},
        {new Vector3Int(-32, -10, 0), new Vector3Int(-12, -10, 0)},
        
        
        
    };
}
