using System.Collections;
using UnityEngine;

public class Clue : ClickableBase
{
    private ClueData data;
    private Coroutine autoHideRoutine;
    private string clueID;

    [Header("Tiempos")]
    [Tooltip("Tiempo EXTRA de lectura después de que termina de escribirse el texto.")]
    [SerializeField] private float readingBufferTime = 5f;
    [SerializeField] private float decisionDelay = 2.5f;

    [Header("Comportamiento")]
    [SerializeField] private bool disableInteractionAfterFound = true;

    public void Initialize(CaseController caseController, ClueData assignedData, int index, string caseID)
    {
        base.Initialize(caseController);
        data = assignedData;
        clueID = $"{caseID}_Clue_{index}";

        bool alreadyFound = PlayerPrefs.GetInt(clueID, 0) == 1;

        if (alreadyFound)
        {
            isFound = true;
            ApplyClickedSprite();
            if (disableInteractionAfterFound && iconImage != null)
                iconImage.raycastTarget = false;
        }
        else
        {
            isFound = false;
            if (iconImage != null) iconImage.raycastTarget = true;
        }
    }

    protected override void OnClicked()
    {
        if (controller == null || data == null) return;

        // 1. CHEQUEO DE SEGURIDAD
        if (controller.uiManager.IsUILocked() || controller.IsInteractionBusy())
            return;

        // 2. ACTIVAR VISUALES
        controller.SetInteractionBusy(true);
        HideAllTooltips();
        tooltipPanel.SetActive(true);

        // 3. CÁLCULO DINÁMICO DEL TIEMPO (Lo hacemos PRIMERO)
        float totalDuration = readingBufferTime;

        if (tooltipTypewriter != null)
        {
            tooltipTypewriter.ShowText(data.clueText);
            totalDuration = (data.clueText.Length * tooltipTypewriter.typingSpeed) + readingBufferTime;
        }
        else if (tooltipText != null)
        {
            tooltipText.text = data.clueText;
            totalDuration = (data.clueText.Length * 0.05f) + readingBufferTime;
        }

        // 4. LÓGICA DE JUEGO (Pasamos el 'totalDuration' al controlador)
        if (!isFound)
        {
            isFound = true;
            PlayerPrefs.SetInt(clueID, 1);
            PlayerPrefs.Save();

            // AQUÍ ESTÁ LA MAGIA: Le pasamos el tiempo calculado
            controller.OnClueFound(data, totalDuration);

            ApplyClickedSprite();
        }

        if (data.hasDecision)
        {
            controller.uiManager.LockUI(true);
            controller.StartCoroutine(ShowDecisionWithDelay());
        }

        if (autoHideRoutine != null)
            controller.StopCoroutine(autoHideRoutine);

        // Usamos el mismo tiempo para auto-ocultar el tooltip
        autoHideRoutine = controller.StartCoroutine(AutoHideTooltip(totalDuration));
    }

    private IEnumerator ShowDecisionWithDelay()
    {
        yield return new WaitForSeconds(decisionDelay);
        controller.uiManager.ShowDecision(
            data.decisionQuestion,
            data.decisionA,
            data.decisionB,
            data.feedbackA,
            data.feedbackB
        );
    }

    private IEnumerator AutoHideTooltip(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        controller.SetInteractionBusy(false);

        if (disableInteractionAfterFound && iconImage != null)
            iconImage.raycastTarget = false;
    }
    public bool IsAlreadyFound()
    {
        return isFound;
    }
    public void ResetClueState()
    {
        isFound = false;
        PlayerPrefs.DeleteKey(clueID);
        if (iconImage != null)
        {
            if (defaultSprite != null) iconImage.sprite = defaultSprite;
            iconImage.raycastTarget = true;
        }
    }
}