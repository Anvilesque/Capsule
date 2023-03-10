using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    private const float ROOT_WIDTH_PERCENTAGE = 0.2f;
    private float rootWidth;
    private Coroutine runningCoroutine;

    private Label timeTime;
    private Label timeDay;
    private TimeController timeController;

    private Button buttonMenu;
    private Button buttonTask;
    private Button buttonInventory;
    private Button buttonMood;
    private Button buttonSettings;
    public UIDocument tasklist;
    private VisualElement taskRoot;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        taskRoot = tasklist.rootVisualElement;
        taskRoot.style.display = DisplayStyle.None;

        timeController = FindObjectOfType<TimeController>();
        timeTime = root.Q<Label>("timeTime");
        timeDay = root.Q<Label>("timeDay");
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;

        buttonMenu = root.Q<Button>("buttonMenu");
        buttonTask = root.Q<Button>("buttonTask");
        buttonInventory = root.Q<Button>("buttonInventory");
        buttonMood = root.Q<Button>("buttonMood");
        buttonSettings = root.Q<Button>("buttonSettings");

        buttonMenu.clicked += buttonMenuPressed;
        buttonTask.clicked += buttonTaskPressed;
        buttonInventory.clicked += buttonInventoryPressed;
        buttonMood.clicked += buttonMoodPressed;
        buttonSettings.clicked += buttonSettingsPressed;
    }

    private void Update()
    {
        rootWidth = (ROOT_WIDTH_PERCENTAGE * 1080f) * ((float)Screen.width / (float)Screen.height);
        timeTime.text = timeController.timeTextTime;
        timeDay.text = timeController.timeTextDay;
    }

    void buttonMenuPressed() {

    }

    void buttonTaskPressed() {
        if (taskRoot.style.display == DisplayStyle.Flex) taskRoot.style.display = DisplayStyle.None;
        else taskRoot.style.display = DisplayStyle.Flex;
    }

    void buttonInventoryPressed() {

    }

    void buttonMoodPressed() {

    }

    void buttonSettingsPressed() {

    }

    public void TranslateHUD(bool translateIn, float duration)
    {
        // if (runningCoroutine != null) StopCoroutine(runningCoroutine);
        runningCoroutine = StartCoroutine(TranslateRoot(translateIn, duration));
    }

    private IEnumerator TranslateRoot(bool translateIn, float duration)
    {
        float timer = 0;
        if (translateIn)
        {
            while (timer < duration)
            {
                float newValue = Mathf.Lerp(-rootWidth, 0, (timer / duration));
                root.style.translate = new Translate(newValue, root.style.translate.value.y);
                timer += Time.deltaTime;
                yield return null;
            }
            root.style.translate = new Translate(0, root.style.translate.value.y);
        }
        else
        {
            while (timer < duration)
            {          
                float newValue = Mathf.Lerp(0, -rootWidth, (timer / duration));
                root.style.translate = new Translate(newValue, root.style.translate.value.y);
                timer += Time.deltaTime;
                yield return null;
            }
            root.style.translate = new Translate(-rootWidth, root.style.translate.value.y);
        }
    }
}
