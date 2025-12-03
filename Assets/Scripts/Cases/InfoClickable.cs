using System.Collections;
using UnityEngine;

public class InfoClickable : ClickableBase
{
    private string infoTextContent;
    private Coroutine hideRoutine;

    [Tooltip("Tiempo EXTRA de lectura después de que termina de escribirse el texto.")]
    [SerializeField] private float readingBufferTime = 5f;

    private bool alreadyCounted = false;
    private string infoKey;

    public void SetInfoText(string text)
    {
        infoTextContent = text;
    }

    public void Initialize(CaseController controllerRef, int index, string caseID)
    {
        base.Initialize(controllerRef);
        infoKey = $"{caseID}_Info_{index}";

        alreadyCounted = PlayerPrefs.GetInt(infoKey, 0) == 1;

        if (tooltipTypewriter == null && tooltipPanel != null)
            tooltipTypewriter = tooltipPanel.GetComponentInChildren<TypewriterText>();

        if (alreadyCounted)
            ApplyClickedSprite();
    }

    protected override void OnClicked()
    {
        if (controller == null) return;

        // 1. CHEQUEO: Si está ocupado, no hacemos nada (y no cerramos lo anterior)
        if (controller.uiManager.IsUILocked() || controller.IsInteractionBusy())
            return;

        // 2. EJECUCIÓN
        controller.SetInteractionBusy(true);
        HideAllTooltips(); // <--- Cierra otros tooltips aquí

        tooltipPanel.SetActive(true);

        // 3. CÁLCULO DINÁMICO
        float totalDuration = readingBufferTime;

        if (tooltipTypewriter != null)
        {
            tooltipTypewriter.ShowText(infoTextContent);
            // Usamos la longitud del texto real
            totalDuration = (infoTextContent.Length * tooltipTypewriter.typingSpeed) + readingBufferTime;
        }
        else if (tooltipText != null)
        {
            tooltipText.text = infoTextContent;
            totalDuration = (infoTextContent.Length * 0.05f) + readingBufferTime;
        }

        if (!alreadyCounted)
        {
            alreadyCounted = true;
            PlayerPrefs.SetInt(infoKey, 1);
            PlayerPrefs.Save();
            controller.OnInfoFound();
            ApplyClickedSprite();
        }

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(AutoHide(totalDuration));
    }

    private IEnumerator AutoHide(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        controller.SetInteractionBusy(false);
    }
}