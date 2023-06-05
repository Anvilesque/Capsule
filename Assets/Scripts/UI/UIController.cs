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
    private SaveDiary saveDiary;

    private Button buttonMenu;
    private Button buttonTask;
    private Button buttonDiary;
    private Button buttonMood;
    private Button buttonSettings;
    // private Button buttonQuit;
    public UIDocument tasklist;
    public UIDocument diaryEntries;
    private VisualElement taskRoot;
    private VisualElement diaryRoot;
    #region Entry Tags
    public const string tagIRLDate = "irldate";
    public const string tagIRLTime = "irltime";
    public const string tagGameDay = "gametime";
    public const string tagGameTime = "gamedate";
    public const string tagGameText = "gametext";
    #endregion
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        taskRoot = tasklist.rootVisualElement;
        taskRoot.style.display = DisplayStyle.None;
        diaryRoot = diaryEntries.rootVisualElement;
        diaryRoot.style.display = DisplayStyle.None;
        
        // timeController = FindObjectOfType<TimeController>();
        // timeTime = root.Q<Label>("timeTime");
        // timeDay = root.Q<Label>("timeDay");
        // timeTime.text = timeController.timeTextTime;
        // timeDay.text = timeController.timeTextDay;
        
        saveDiary = FindObjectOfType<SaveDiary>();

        // buttonMenu = root.Q<Button>("buttonMenu");
        buttonTask = root.Q<Button>("buttonTask");
        buttonDiary = root.Q<Button>("buttonDiary");
        buttonMood = root.Q<Button>("buttonMood");
        // buttonSettings = root.Q<Button>("buttonSettings");
        // buttonQuit = root.Q<Button>("buttonQuit");

        // buttonMenu.clicked += buttonMenuPressed;
        buttonTask.clicked += buttonTaskPressed;
        buttonDiary.clicked += buttonDiaryPressed;
        buttonMood.clicked += buttonMoodPressed;
        // buttonSettings.clicked += buttonSettingsPressed;
        // buttonQuit.clicked += buttonQuitPressed;

        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        rootWidth = (ROOT_WIDTH_PERCENTAGE * 1080f) * ((float)Screen.width / (float)Screen.height);
        // timeTime.text = timeController.timeTextTime;
        // timeDay.text = timeController.timeTextDay;
        if (Input.GetButtonDown("Open Menu"))
        {
            if (root.style.display == DisplayStyle.Flex) root.style.display = DisplayStyle.None;
            else root.style.display = DisplayStyle.Flex;
        }
    }

    void buttonMenuPressed() {

    }

    void buttonTaskPressed() {
        if (taskRoot.style.display == DisplayStyle.Flex) taskRoot.style.display = DisplayStyle.None;
        else taskRoot.style.display = DisplayStyle.Flex;
    }

    void buttonDiaryPressed() {
        VisualElement entries = diaryRoot.Q<VisualElement>("entries");   
        if (diaryRoot.style.display == DisplayStyle.Flex) diaryRoot.style.display = DisplayStyle.None;
        else
        {
            diaryRoot.style.display = DisplayStyle.Flex;
            if (entries.childCount == 0) {}
            else while (entries.childCount > 0)
            {
                entries.RemoveAt(0);
            }
            foreach (Dictionary<string, string> entry in saveDiary.previousDiaryEntries)
            {
                Label label = new Label($@"Day {entry[tagGameDay]} - {entry[tagGameTime]}: {entry[tagGameText]}" + "\n");
                label.AddToClassList("text-diary-general");
                label.style.whiteSpace = WhiteSpace.Normal;
                entries.Add(label);
            }
        }
    }

    void buttonMoodPressed() {

    }

    void buttonSettingsPressed() {

    }

    //  void buttonQuitPressed() 
    // {
    //     Application.Quit();
    // }

}
