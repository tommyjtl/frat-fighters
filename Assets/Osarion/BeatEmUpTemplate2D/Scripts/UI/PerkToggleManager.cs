using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PerkToggleController : MonoBehaviour
{
    public List<Toggle> perkToggles; // assign all toggles in order from Inspector

    void OnEnable()
    {
        // Subscribe to the event
        // GlobalVariables.OnSPChanged += UpdatePerkToggles;
    }

    void Start()
    {
        // Add listeners to each toggle
        foreach (Toggle toggle in perkToggles)
        {
            toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle, isOn));
        }
    }

    void OnToggleChanged(Toggle changedToggle, bool isOn)
    {
        if (isOn)
        {
            // Disable all other toggles
            foreach (Toggle toggle in perkToggles)
            {
                if (toggle != changedToggle)
                    toggle.isOn = false;

                // // check if the perk is unlocked
                // if (GlobalVariables.Instance != null)
                // {
                //     if (GlobalVariables.Instance.perks[perkToggles.IndexOf(toggle)].Item1)
                //         SetToggleInteractable(perkToggles.IndexOf(toggle), false);
                //     else
                //         SetToggleInteractable(perkToggles.IndexOf(toggle), true);
                // }

                // // get the background object under the toggle
                // GameObject background = toggle.transform.Find("Background").gameObject;
                // // print if the background object is null
                // if (background == null)
                // {
                //     Debug.LogWarning("Background object is null for toggle: " + toggle.name);
                // }
                // else
                // {
                //     Debug.Log("Background object found for toggle: " + toggle.name);
                //     // set the color of the background object
                //     Image backgroundImage = background.GetComponent<Image>();
                //     if (backgroundImage != null)
                //     {
                //         ColorBlock cb = toggle.colors;
                //         backgroundImage.color = cb.normalColor;
                //     }
                //     else
                //     {
                //         Debug.LogWarning("Background image component is null for toggle: " + toggle.name);
                //     }
                // }

            }
        }

        int selectedIndex = GetSelectedPerkIndex();
        if (GlobalVariables.Instance != null)
            GlobalVariables.Instance.perkIdxSelected = selectedIndex.ToString();
    }

    // Get selected toggle index
    public int GetSelectedPerkIndex()
    {
        for (int i = 0; i < perkToggles.Count; i++)
        {
            if (perkToggles[i].isOn)
                return i; // zero-based index
        }
        return -1; // none selected
    }

    // Set the color of a specific toggle
    public void SetToggleColor(int index, Color newColor)
    {
        if (index >= 0 && index < perkToggles.Count)
        {
            Toggle toggle = perkToggles[index];
            ColorBlock cb = toggle.colors;
            cb.normalColor = newColor;
            toggle.colors = cb;
        }
        else
        {
            Debug.LogWarning("Invalid toggle index.");
        }
    }

    public void SetToggleInteractable(int index, bool isInteractable)
    {
        if (index >= 0 && index < perkToggles.Count)
        {
            Toggle toggle = perkToggles[index];
            toggle.interactable = isInteractable;
        }
        else
        {
            Debug.LogWarning("Invalid toggle index.");
        }
    }



}
