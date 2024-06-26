using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    public static List<Vector3Int> cardinalDirections = new List<Vector3Int>()
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };
    [SerializeField] private List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public List<Vector3Int> tilesStandable;
    public Tilemap floorMap, stairsMap, wallMap, transitionMap, transitionMapFloor, interactableMap;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap[] tilemaps = new Tilemap[] {floorMap, stairsMap, wallMap, transitionMap, transitionMapFloor, interactableMap};
        foreach (Tilemap tilemap in FindObjectsOfType<Tilemap>())
        {
            tilemap.CompressBounds();
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
        tilesStandable = new List<Vector3Int>();
        List<Tilemap> groundMaps = new List<Tilemap>() {floorMap, stairsMap, transitionMap, transitionMapFloor};
        // Do for both floorMap and transitionMap
        foreach (Tilemap groundMap in groundMaps)
        {
            foreach (Vector3Int tilePos in groundMap.cellBounds.allPositionsWithin)
            {
                if (!groundMap.HasTile(tilePos)) continue;
                Vector3Int legLevel = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 2);
                Vector3Int headLevel = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 4);
                if (wallMap.HasTile(legLevel) || wallMap.HasTile(headLevel)) continue;
                if (interactableMap.HasTile(legLevel) || interactableMap.HasTile(headLevel)) continue;
                if (floorMap.HasTile(headLevel)) continue;
                if (ScanForTile(stairsMap, tilePos) && GetTilePosition(stairsMap, tilePos).z > tilePos.z) continue;
                tilesStandable.Add(new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 2));
            }
        }
    }

    public TileData GetTileData(Tilemap map, Vector3Int tilePosition)
    {
        TileBase tile = map.GetTile(tilePosition);

        if (tile == null) return null;
        else return dataFromTiles[tile];
    }

    public List<Vector3Int> GetAdjacentCellsPositions(Tilemap map, Vector3Int centerTilePosition)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        foreach (Vector3Int direction in cardinalDirections)
        {
            adjacentTiles.Add(centerTilePosition + direction);
        }
        return adjacentTiles;
    }

    public bool ScanForTile(Tilemap map, Vector3Int tilePosition)
    {
        for (int z = map.cellBounds.zMin; z <= map.cellBounds.zMax; z++)
        {
            tilePosition.z = z;
            if (map.HasTile(tilePosition))
            {
                return true;
            }
        }
        return false;
    }

    public Vector3Int GetTilePosition(Tilemap map, Vector3Int tilePosition)
    {
        for (int z = map.cellBounds.zMin; z <= map.cellBounds.zMax; z++)
        {
            tilePosition.z = z;
            if (map.HasTile(tilePosition))
            {
                return tilePosition;
            }
        }
        return Vector3Int.zero;
    }
}
