using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TransitionPoint : MonoBehaviour
{
    public Vector3Int positionCell;
    public enum PointTypes
    {
        Start,
        Destination
    }
    public PointTypes pointType = PointTypes.Start;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private Tilemap transitionMap;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        transitionMap = tileManager.transitionMap;
        positionCell = transitionMap.WorldToCell(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Doing this just for the Editor; would normally get floorMap through tileManager
        if (transitionMap == null)
        {
            transitionMap = new List<Tilemap>(FindObjectsOfType<Tilemap>()).Find((Tilemap tilemap) => tilemap.name.Contains("Transition"));
        }
        // Update position in the Editor, for easier editing
        if (Application.isEditor) positionCell = transitionMap.WorldToCell(transform.position);
    }
}
