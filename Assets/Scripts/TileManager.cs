using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    public float distX {get; private set;}
    public float distY {get; private set;}

    [SerializeField]
    private List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;

    // Start is called before the first frame update
    void Start()
    {
        distX = FindObjectOfType<Grid>().cellSize.x / 2; // X-dist to next cell = half of cell width
        distY = FindObjectOfType<Grid>().cellSize.y / 2; // Y-dist to next cell = half of cell height
        CreateTileDictionary();
    }

    private void CreateTileDictionary()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach(var tileData in tileDatas) // iterates through Scriptable Objects
        {
            foreach(var tile in tileData.tiles) // iterates through TileBases to which Scriptable Object assigns TileData
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public Vector3Int WorldCoordsToGridCoords(Vector3 localCoords)
    {
        Vector3Int gridCoordsInt = new Vector3Int();
        gridCoordsInt.x = (int)(localCoords.x / distX + localCoords.y / distY) / 2;
        gridCoordsInt.y = (int)(localCoords.y / distY - localCoords.x / distX) / 2;
        gridCoordsInt.z = (int)localCoords.z;
        return gridCoordsInt;
    }

    public TileData GetTileData(Tilemap map, Vector3Int tilePosition)
    {
        TileBase tile = map.GetTile(tilePosition);

        if (tile == null)
            return null;
        else
            return dataFromTiles[tile];
    }
}
