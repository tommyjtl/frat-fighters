using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using BeatEmUpTemplate2D;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class PanelGroup
    {
        public List<GameObject> panels;
    }

    [Header("Panel Groups")]
    public List<PanelGroup> panelGroups = new List<PanelGroup>();

    [Header("Scene to Load After Cutscene")]
    public string nextSceneName;

    [Header("Panel Timing")]
    public float panelDisplayInterval = 3f; // seconds between panels

    private int currentGroupIndex = 0;
    private int panelIndexInGroup = 0;
    private float panelTimer = 0f;

    private InputAction anyKeyAction;

    void Awake()
    {
        anyKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/anyKey");
        anyKeyAction.performed += ctx => HandleInput();
    }

    void OnEnable()
    {
        anyKeyAction.Enable();
    }

    void OnDisable()
    {
        anyKeyAction.Disable();
    }

    void Start()
    {
        DisableAllPanels();
        ShowCurrentPanel();
    }

    void Update()
    {
        if (currentGroupIndex >= panelGroups.Count)
            return;

        var group = panelGroups[currentGroupIndex].panels;

        // If not at last panel, auto-progress after delay
        if (panelIndexInGroup < group.Count - 1)
        {
            panelTimer += Time.deltaTime;

            if (panelTimer >= panelDisplayInterval)
            {
                panelIndexInGroup++;
                ShowCurrentPanel();
                panelTimer = 0f;
            }
        }
    }

    void HandleInput()
    {
        if (currentGroupIndex >= panelGroups.Count)
            return;

        var group = panelGroups[currentGroupIndex].panels;

        // If not all panels shown, reveal next panel & reset timer
        if (panelIndexInGroup < group.Count - 1)
        {
            panelIndexInGroup++;
            ShowCurrentPanel();
            panelTimer = 0f;
        }
        else
        {
            // All panels shown → go to next group or load scene
            if (currentGroupIndex == panelGroups.Count - 1)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                DisableAllInGroup(currentGroupIndex);
                currentGroupIndex++;
                panelIndexInGroup = 0;
                panelTimer = 0f;
                ShowCurrentPanel();
            }
        }
    }

    void ShowCurrentPanel()
    {
        var group = panelGroups[currentGroupIndex].panels;

        for (int i = 0; i <= panelIndexInGroup && i < group.Count; i++)
        {
            group[i].SetActive(true);
        }

        AudioController.PlaySFX("CutsceneNoise");
    }

    void DisableAllPanels()
    {
        foreach (var group in panelGroups)
        {
            foreach (var panel in group.panels)
            {
                panel.SetActive(false);
            }
        }
    }

    void DisableAllInGroup(int groupIndex)
    {
        foreach (var panel in panelGroups[groupIndex].panels)
        {
            panel.SetActive(false);
        }
    }
}
