using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PlayerTransition : MonoBehaviour
{
    private TileManager tileManager;
    private Tilemap transitionMap;
    private Tilemap transitionMapFloor;
    private PlayerMovement playerMovement;
    private TimeController timeController;
    private List<TransitionPoint> transitionPoints;
    public bool canTeleport;
    public bool isTeleporting {get; private set;}
    public bool isInRoom;
    public GameObject shopClosedDisplay;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        transitionMap = tileManager.transitionMap;
        transitionMapFloor = tileManager.transitionMapFloor;
        playerMovement = GetComponent<PlayerMovement>();
        timeController = FindObjectOfType<TimeController>();
        transitionPoints = new List<TransitionPoint>(FindObjectsOfType<TransitionPoint>());
        foreach (TransitionPoint point in transitionPoints)
        {
            point.GetComponent<SpriteRenderer>().enabled = false;
        }
        canTeleport = true;
        isTeleporting = false;
        isInRoom = FindObjectOfType<SaveManager>().myData.playerIsInRoom;
        shopClosedDisplay.SetActive(false);
    }

    public void HandleTeleportFromTransitionTilemap()
    {
        bool onTeleportTile = tileManager.ScanForTile(transitionMap, playerMovement.currentPos)
                           || tileManager.ScanForTile(transitionMapFloor, playerMovement.currentPos);
        if (canTeleport)
        {
            if (onTeleportTile) DoTeleport();
        } 
        else if (!onTeleportTile) canTeleport = true;
    }

    private void DoTeleport()
    {
        playerMovement.DisableMovement();
        Vector3Int blockUnderPlayer = playerMovement.currentPos - new Vector3Int(0, 0, 2);
        TransitionPoint destinationPoint = GetTransitionPoint(blockUnderPlayer);
        if (destinationPoint == null)
        {
            StartCoroutine(TeleportFadeInOut(playerMovement.currentPos));
            return;
        }
        if (!CheckCanTeleportSpecial(destinationPoint)) return;
        Vector3Int newCoords = destinationPoint.positionCell + new Vector3Int(0, 0, 2);
        StartCoroutine(TeleportFadeInOut(newCoords));
    }

    private bool CheckCanTeleportSpecial(TransitionPoint destinationPoint)
    {
        string nameTransition = destinationPoint.transform.parent.name;
        if (nameTransition.Contains("Room"))
        {
            if (timeController.hours == 20 && timeController.mins >= 30)
            {
                StartCoroutine(timeController.CloseShop());
                return false;
            }
        }

        if (nameTransition.Contains("RoomReturn"))
        {
            if (timeController.isShopClosed) 
            {
                DisplayShopClosedError();
                return false;
            }
            timeController.mins += 30;
            isInRoom = false;
            return true;
        }

        if (nameTransition.Contains("RoomGo"))
        {
            timeController.mins += 30;
            if (timeController.hours >= 21) timeController.isShopClosed = true;
            isInRoom = true;
            return true;
        }

        return true;
    }

    private void DisplayShopClosedError()
    {
        shopClosedDisplay.SetActive(true);
        StartCoroutine(FadeUpErrorText(shopClosedDisplay));
        playerMovement.EnableMovement();
    }

    IEnumerator FadeUpErrorText(GameObject errorDisplay)
    {
        float holdTimer = 0f;
        float holdDuration = 0.5f;
        float fadeTimer = 0f;
        float fadeDuration = 0.5f;
        float moveUpInterval = 0.1f;
        TMP_Text text = errorDisplay.GetComponentInChildren<TMP_Text>();
        Vector3 originalPosition = text.rectTransform.position;
        while (holdTimer < holdDuration)
        {
            text.rectTransform.position = new Vector3(text.rectTransform.position.x, text.rectTransform.position.y + moveUpInterval, text.rectTransform.position.z);
            holdTimer += Time.deltaTime;
            yield return null;
        }
        while (text.color.a > 0)
        {
            text.rectTransform.position = new Vector3(text.rectTransform.position.x, text.rectTransform.position.y + moveUpInterval, text.rectTransform.position.z);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - fadeTimer / fadeDuration);
            fadeTimer += Time.deltaTime;
            yield return null;
        }
        errorDisplay.gameObject.SetActive(false);
        text.rectTransform.position = originalPosition;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }

    public IEnumerator TeleportFadeInOut(Vector3Int destination)
    {
        timeController.canUpdateTime = false;
        isTeleporting = true;
        FadeController fadeController = FindObjectOfType<FadeController>();
        fadeController.FadeIn();
        while (fadeController.isFading) yield return null;
        playerMovement.currentPos = destination;
        transform.position = transitionMap.CellToWorld(destination);
        yield return new WaitForSeconds(2f);
        canTeleport = false;
        isTeleporting = false;
        playerMovement.EnableMovement();
        fadeController.FadeOut();
        timeController.canUpdateTime = true;
    }

    private TransitionPoint GetTransitionPoint(Vector3Int blockUnderPlayer)
    {
        TransitionPoint pointStart = transitionPoints.Find(pointStart => pointStart.pointType == TransitionPoint.PointTypes.Start && pointStart.positionCell == blockUnderPlayer);
        // Debug.Log(pointStart);
        if (pointStart == null) return null;
        TransitionPoint pointDest = pointStart.transform.GetChild(0).GetComponent<TransitionPoint>();
        return pointDest;
    }
}
