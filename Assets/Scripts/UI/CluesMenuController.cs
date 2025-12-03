using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CluesMenuController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private Button toggleButton;
    [SerializeField] private ClueSlotUI[] slots;

    [Header("Sprites Específicos del Caso")]
    [SerializeField] private Sprite[] clueIcons;

    private CaseController currentController;
    private bool isOpen = false;

    private void Start()
    {
        contentPanel.SetActive(false);
        isOpen = false;

        toggleButton.onClick.AddListener(ToggleMenu);
        currentController = FindObjectOfType<CaseController>();
    }

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        contentPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshSlots();
        }
    }

    
    public void CloseMenu()
    {
        if (isOpen)
        {
            isOpen = false;
            contentPanel.SetActive(false);
        }
    }

    public void RefreshSlots()
    {
        if (currentController == null) return;
        var data = currentController.GetCaseData();
        var uiManager = currentController.uiManager;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < data.clues.Length)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].SetupSlot(data.clues[i], i, data.caseID, uiManager);

                if (clueIcons != null && i < clueIcons.Length)
                {
                    string clueID = $"{data.caseID}_Clue_{i}";
                    if (PlayerPrefs.GetInt(clueID, 0) == 1)
                    {
                        slots[i].SetIcon(clueIcons[i]);
                    }
                }
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}