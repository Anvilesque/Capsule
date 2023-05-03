using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTransition : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap floorMap, wallMap, transitionMap, interactableMap;
    private PlayerMovement playerMovement;
    public bool canTeleport;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        floorMap = tileManager.floorMap;
        wallMap = tileManager.wallMap;
        transitionMap = tileManager.transitionMap;
        interactableMap = tileManager.interactableMap;
        playerMovement = GetComponent<PlayerMovement>();
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
        Vector3Int newCoords = TransitionPoints.TransitionStartDests[playerMovement.currentPos - new Vector3Int(0, 0, 2)];
        newCoords.z += 2;
        playerMovement.UpdateCurrentPosition(newCoords);
        transform.position = transitionMap.CellToWorld(newCoords);
        yield return new WaitForSeconds(2f);
        canTeleport = false;
        playerMovement.EnableMovement();
        fadeController.FadeOut();
    }
}
