using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    private List<Tilemap> tilemaps;
    public float distX {get; private set;}
    public float distY {get; private set;}

    [SerializeField]
    private Tilemap map;
    [SerializeField]
    private List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;

    // Start is called before the first frame update
    void Start()
    {
        distX = FindObjectOfType<Grid>().cellSize.x / 2; // X-dist to next cell = half of cell width
        distY = FindObjectOfType<Grid>().cellSize.y / 2; // Y-dist to next cell = half of cell height
        // tilemaps = new List<Tilemap>(FindObjectsOfType<Tilemap>());
        // SceneManager.sceneLoaded += ProcessTilemaps;
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach(var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    // void ProcessTilemaps(Scene scene, LoadSceneMode mode)
    // {
    //     foreach (Tilemap tilemap in tilemaps)
    //     {
    //         Vector3Int gridMin = WorldCoordsToGridCoords(tilemap.localBounds.min);
    //         Vector3Int gridMax = WorldCoordsToGridCoords(tilemap.localBounds.max);
    //     }
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    public Vector3Int WorldCoordsToGridCoords(Vector3 localCoords)
    {
        Vector3Int gridCoordsInt = new Vector3Int();
        gridCoordsInt.x = (int)(localCoords.x / distX + localCoords.y / distY) / 2;
        gridCoordsInt.y = (int)(localCoords.y / distY - localCoords.x / distX) / 2;
        return gridCoordsInt;
    }

    public TileData GetTileData(Vector3Int tilePosition)
    {
        TileBase tile = map.GetTile(tilePosition);

        if (tile == null)
            return null;
        else
            return dataFromTiles[tile];
    }
}
