using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    [Header("Interactables")]
    public string taskName;
    
    [Header("PathsNPC")]
    public PathType pathType = PathType.Path;
    public enum PathType
    {
        Path, Node
    }
}
