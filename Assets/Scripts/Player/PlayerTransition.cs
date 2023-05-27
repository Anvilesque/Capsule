using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTransition : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap floorMap, wallMap, transitionMap, interactableMap;
    private PlayerMovement playerMovement;
    private List<TransitionPoint> transitionPoints;
    public bool canTeleport;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        transitionMap = tileManager.transitionMap;
        playerMovement = GetComponent<PlayerMovement>();
        transitionPoints = new List<TransitionPoint>(FindObjectsOfType<TransitionPoint>());
        foreach (TransitionPoint point in transitionPoints)
        {
            point.GetComponent<SpriteRenderer>().enabled = false;
        }
        canTeleport = true;
    }

    public void HandleTeleport()
    {
        bool onTeleportTile = tileManager.ScanForTile(transitionMap, playerMovement.currentPos);
        if (canTeleport)
        {
            if (onTeleportTile) DoTeleport();
        } 
        else if (!onTeleportTile) canTeleport = true;
    }

    private void DoTeleport()
    {
        StartCoroutine(TeleportFadeInOut());
    }

    IEnumerator TeleportFadeInOut()
    {
        playerMovement.DisableMovement();
        FadeController fadeController = FindObjectOfType<FadeController>();
        fadeController.FadeIn();
        while (fadeController.isFading) yield return null;
        Vector3Int blockUnderPlayer = playerMovement.currentPos - new Vector3Int(0, 0, 2);
        Vector3Int newCoords = GetTransitionPoint(blockUnderPlayer) + new Vector3Int(0, 0, 2);
        playerMovement.UpdateCurrentPosition(newCoords);
        transform.position = transitionMap.CellToWorld(newCoords);
        yield return new WaitForSeconds(2f);
        canTeleport = false;
        playerMovement.EnableMovement();
        fadeController.FadeOut();
    }

    private Vector3Int GetTransitionPoint(Vector3Int blockUnderPlayer)
    {
        TransitionPoint pointStart = transitionPoints.Find(pointStart => pointStart.pointType == TransitionPoint.PointTypes.Start && pointStart.positionCell == blockUnderPlayer);
        // Debug.Log(pointStart);
        if (pointStart == null) return blockUnderPlayer;
        TransitionPoint pointDest = pointStart.transform.GetChild(0).GetComponent<TransitionPoint>();
        return pointDest.positionCell;
    }
}
