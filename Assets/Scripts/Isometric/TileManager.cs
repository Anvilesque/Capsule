using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
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
        foreach (Tilemap tilemap in FindObjectsOfType<Tilemap>())
        {
            tilemap.CompressBounds();
            if (tilemap.gameObject.name.Contains("Floor")) floorMap = tilemap;
            else if (tilemap.gameObject.name.Contains("Wall")) wallMap = tilemap;
            else if (tilemap.gameObject.name.Contains("Transition")) transitionMap = tilemap;
            else if (tilemap.gameObject.name.Contains("Interactable")) interactableMap = tilemap;
        }
        floorMap.CompressBounds();
        wallMap.CompressBounds();
        transitionMap.CompressBounds();
        interactableMap.CompressBounds();
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
        List<Tilemap> groundMaps = new List<Tilemap>() {floorMap, transitionMap};
        List<Tilemap> collisionMaps = new List<Tilemap>() {wallMap, interactableMap};
        // Do for both floorMap and transitionMap
        foreach (Tilemap groundMap in groundMaps)
        {
            foreach (Vector3Int tilePos in groundMap.cellBounds.allPositionsWithin)
            {
                if (!groundMap.HasTile(tilePos)) continue;
                bool standable = true;
                foreach (Tilemap collisionMap in collisionMaps)
                {
                    Vector3Int legLevel = new Vector3Int(tilePos.x - 1, tilePos.y - 1, tilePos.z + 4);
                    Vector3Int headLevel = new Vector3Int(tilePos.x - 1, tilePos.y - 1, tilePos.z + 6);
                    if (collisionMap.HasTile(legLevel) || collisionMap.HasTile(headLevel))
                    {
                        standable = false;
                        break;
                    }
                }
                if (standable) tilesStandable.Add(new Vector3Int(tilePos.x - 1, tilePos.y - 1, tilePos.z + 4));
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
