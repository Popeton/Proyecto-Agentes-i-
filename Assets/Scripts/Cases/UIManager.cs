using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Contador de pistas ")]
    [SerializeField] private Image clueCounterImage; // La imagen UI que cambiará
    [SerializeField] private Sprite[] clueCountSprites;

    [Header("Panel de Lectura Rápida pistas")]
    // Puedes usar el mismo tooltipPanel de las pistas o uno genérico
    [SerializeField] private GameObject readOnlyPanel;
    [SerializeField] private TypewriterText readOnlyText;
    [SerializeField] private Button btnCloseReadOnly;

    [Header("Evaluación Especial Caso 2")]
    [SerializeField] private CyberbullyingEvaluationUI case2Panel;

    [Header("Panel de Introducción")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private TypewriterText introTypewriter;
    [SerializeField] private Button btnIntroContinue;
    [SerializeField] private TypewriterText btnIntroContinueText;

    [Header("Panel de Decisión (pistas con opciones)")]
    [SerializeField] private GameObject decisionPanel;
    [SerializeField] private TypewriterText decisionQuestionTypewriter;
    [SerializeField] private Button optionAButton;
    [SerializeField] private TypewriterText optionATextTypewriter;
    [SerializeField] private Button optionBButton;
    [SerializeField] private TypewriterText optionBTextTypewriter;

    [Header("Panel de Evaluación Final")]
    [SerializeField] private GameObject evaluationPanel;
    [SerializeField] private TypewriterText evalQuestionTypewriter;
    [SerializeField] private Button evalOptionAButton;
    [SerializeField] private TypewriterText evalOptionATextTypewriter;
    [SerializeField] private Button evalOptionBButton;
    [SerializeField] private TypewriterText evalOptionBTextTypewriter;

    [Header("Popup de feedback (compartido entre decisiones y evaluación)")]
    [SerializeField] private GameObject feedbackPopup;
    [SerializeField] private TypewriterText feedbackTypewriter;
    [SerializeField] private Button btnPopupAction;
    [SerializeField] private TypewriterText btnPopupActionTextTypewriter;

    [Header("Configuración de tiempos")]
    [SerializeField, Tooltip("Tiempo de espera antes de auto-continuar tras una respuesta correcta.")]
    private float autoContinueDelay = 4f;
    [SerializeField, Tooltip("Tiempo de espera antes de auto-volver al menú tras evaluación correcta.")]
    private float autoReturnDelay = 4f;

    public event System.Action OnDecisionAnswered;

    private CaseController currentCase;
    private string evalFeedbackA, evalFeedbackB;
    private bool uiLocked = false;
    private Coroutine autoContinueRoutine;

    private void Start()
    {
        HideAllPanels();
        introPanel.SetActive(true);
    }

    // PANEL MANAGEMENT
    public void HideAllPanels()
    {
        introPanel.SetActive(false);
        decisionPanel.SetActive(false);
        evaluationPanel.SetActive(false);
        feedbackPopup.SetActive(false);
        uiLocked = false;
    }

    // INTRODUCCIÓN
    public void ShowIntro(string intro)
    {
        introPanel.SetActive(true);
        introTypewriter.ShowText(intro);
        btnIntroContinue.onClick.RemoveAllListeners();
        btnIntroContinueText.ShowText("Iniciar misión");
        btnIntroContinue.onClick.AddListener(() => introPanel.SetActive(false));
    }

    // CONTADOR DE PISTAS
    public void UpdateClueCounter(int found, int total)
    {
        if (clueCounterImage == null || clueCountSprites == null || clueCountSprites.Length == 0)
            return;

        // Aseguramos que el índice no se salga del array (por si hay errores de conteo)
        // Si found es 3, mostramos el sprite[3]
        int spriteIndex = Mathf.Clamp(found, 0, clueCountSprites.Length - 1);

        clueCounterImage.sprite = clueCountSprites[spriteIndex];

        // Se Activa 'Native Size' si los sprites tienen tamaños diferentes
        // clueCounterImage.SetNativeSize(); 
    }

    public void ShowReadOnlyTooltip(string text)
    {
        // Opción A: Usar un panel simple dedicado para re-leer
        if (readOnlyPanel != null && readOnlyText != null)
        {
            readOnlyPanel.SetActive(true);
            readOnlyText.ShowText(text);
        }
        // Opción B: Si prefieres reutilizar el popup de feedback genérico
        else if (feedbackPopup != null)
        {
            feedbackPopup.SetActive(true);
            feedbackTypewriter.ShowText(text);

            btnPopupActionTextTypewriter.ShowText("Cerrar");
            btnPopupAction.onClick.RemoveAllListeners();
            btnPopupAction.onClick.AddListener(() => feedbackPopup.SetActive(false));
        }
    }
    // PANEL DE DECISIONES
    public void ShowDecision(string question, string optionA, string optionB,
                             string feedbackA, string feedbackB)
    {
        LockUI(true);
        HideAllClues();

        decisionPanel.SetActive(true);
        feedbackPopup.SetActive(false);

        decisionQuestionTypewriter.ShowText(question);
        optionATextTypewriter.ShowText(optionA);
        optionBTextTypewriter.ShowText(optionB);

        optionAButton.onClick.RemoveAllListeners();
        optionBButton.onClick.RemoveAllListeners();

        optionAButton.onClick.AddListener(() => ShowFeedbackPopup(false, feedbackA, false));
        optionBButton.onClick.AddListener(() => ShowFeedbackPopup(true, feedbackB, false));
    }

    // PANEL DE EVALUACIÓN FINAL
    public void ShowEvaluation(string question, string optionA, string optionB,
                               string feedbackA, string feedbackB, CaseController controller)
    {
        LockUI(true);
        HideAllClues();

        evaluationPanel.SetActive(true);
        feedbackPopup.SetActive(false);

        evalQuestionTypewriter.ShowText(question);
        evalOptionATextTypewriter.ShowText(optionA);
        evalOptionBTextTypewriter.ShowText(optionB);

        currentCase = controller;
        evalFeedbackA = feedbackA;
        evalFeedbackB = feedbackB;
        

        evalOptionAButton.onClick.RemoveAllListeners();
        evalOptionBButton.onClick.RemoveAllListeners();

        evalOptionAButton.onClick.AddListener(() => ShowFeedbackPopup(false, evalFeedbackA, true));
        evalOptionBButton.onClick.AddListener(() => ShowFeedbackPopup(true, evalFeedbackB, true));
    }

    // Método actualizado para Case 2
    public void ShowCase2Evaluation(string question, string successText, string failText, CaseController controller)
    {
        LockUI(true);
        HideAllPanels();

        // --- CORRECCIÓN VITAL ---
        currentCase = controller; // Guardamos la referencia para usarla al salir
        // ------------------------

        if (case2Panel != null)
        {
            case2Panel.SetupPanel(question, successText, failText, (isCorrect, feedbackToShow) =>
            {
                ShowFeedbackPopup(isCorrect, feedbackToShow, true);
            });
        }
    }

    // POPUP COMPARTIDO DE FEEDBACK
    private void ShowFeedbackPopup(bool isCorrect, string feedback, bool isEvaluation)
    {
        decisionPanel.SetActive(false);
        evaluationPanel.SetActive(false);

        feedbackPopup.SetActive(true);
        feedbackTypewriter.ShowText(feedback);
        btnPopupAction.onClick.RemoveAllListeners();

        if (autoContinueRoutine != null)
            StopCoroutine(autoContinueRoutine);

        if (!isCorrect)
        {
            btnPopupActionTextTypewriter.ShowText(isEvaluation ? "Reiniciar caso" : "Intentar de nuevo");
            btnPopupAction.onClick.AddListener(() =>
            {
                feedbackPopup.SetActive(false);
                if (isEvaluation)
                {
                    LockUI(false);
                    currentCase.ResetCase();
                }
                else
                {
                    decisionPanel.SetActive(true);
                }
            });
        }
        else
        {
            btnPopupActionTextTypewriter.ShowText("Continuar");
            btnPopupAction.onClick.AddListener(() =>
            {
                if (isEvaluation)
                    ReturnToMainMenu();
                else
                    HideDecisionPopupAndContinue();
            });

            float delay = isEvaluation ? autoReturnDelay : autoContinueDelay;
            autoContinueRoutine = StartCoroutine(AutoContinueAfterDelay(delay, isEvaluation));
        }
    }

    private IEnumerator AutoContinueAfterDelay(float delay, bool isEvaluation)
    {
        yield return new WaitForSeconds(delay);
        if (isEvaluation)
            ReturnToMainMenu();
        else
            HideDecisionPopupAndContinue();
    }

    private void HideDecisionPopupAndContinue()
    {
        feedbackPopup.SetActive(false);
        LockUI(false);
        OnDecisionAnswered?.Invoke();
    }

    private void ReturnToMainMenu()
    {
        feedbackPopup.SetActive(false);
        LockUI(false);
        currentCase.CompleteCase();
        SceneManager.LoadScene("Menu");
    }

    // UTILIDADES
    public bool IsDecisionPanelActive() => decisionPanel != null && decisionPanel.activeSelf;
    public bool IsEvaluationPanelActive() => evaluationPanel != null && evaluationPanel.activeSelf;
    public bool IsUILocked() => uiLocked;
    public void LockUI(bool value) => uiLocked = value;

    private void HideAllClues()
    {
        Clue[] clues = FindObjectsOfType<Clue>();
        foreach (var clue in clues)
            clue.HideTooltipInstant();
    }
}
