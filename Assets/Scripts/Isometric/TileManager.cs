using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    public static float distX {get; private set;}
    public static float distY {get; private set;}

    [SerializeField] private List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public List<Vector3Int> tilesStandable;
    public Tilemap floorMap {get; private set;}
    public Tilemap wallMap {get; private set;}
    public Tilemap transitionMap {get; private set;}
    public Tilemap interactableMap {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        distX = FindObjectOfType<Grid>().cellSize.x / 2; // X-dist to next cell = half of cell width
        distY = FindObjectOfType<Grid>().cellSize.y / 2; // Y-dist to next cell = half of cell height
        foreach (Tilemap tilemap in FindObjectsOfType<Tilemap>())
        {
            tilemap.CompressBounds();
            if (tilemap.gameObject.name.Contains("Floor")) floorMap = tilemap;
            if (tilemap.gameObject.name.Contains("Wall")) wallMap = tilemap;
            if (tilemap.gameObject.name.Contains("Transition")) transitionMap = tilemap;
            if (tilemap.gameObject.name.Contains("Interactable")) interactableMap = tilemap;
        }
        CreateTileDictionary();
        CreateStandableList();
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

    private void CreateStandableList()
    {
        for (int x = floorMap.cellBounds.xMin; x <= floorMap.cellBounds.xMax; x++)
        {
            for (int y = floorMap.cellBounds.yMin; y <= floorMap.cellBounds.yMax; y++)
            {
                Vector3Int floorTilePos = new Vector3Int(x, y, 1); // Floor is assumed to have z = 1
                if (!floorMap.HasTile(floorTilePos)) continue;

                bool standable = true;
                Vector3Int tilePos = new Vector3Int(floorTilePos.x, floorTilePos.y);
                for (int z = wallMap.cellBounds.zMin; z <= wallMap.cellBounds.zMax; z++)
                {
                    tilePos.z = z;
                    if (wallMap.HasTile(tilePos))
                    {
                        standable = false;
                        break;
                    }
                }
                for (int z = interactableMap.cellBounds.zMin; z <= interactableMap.cellBounds.zMax; z++)
                {
                    tilePos.z = z;
                    if (interactableMap.HasTile(tilePos))
                    {
                        standable = false;
                        break;
                    }
                }
                if (standable) tilesStandable.Add(new Vector3Int(floorTilePos.x, floorTilePos.y, floorTilePos.z));
            }
        } 
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
