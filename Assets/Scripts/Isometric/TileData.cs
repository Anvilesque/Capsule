using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    [Header("Transitions")]
    public Vector3Int newPos;

    [Header("Interactables")]
    public string taskName;
    
}
