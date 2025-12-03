using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CyberbullyingEvaluationUI : MonoBehaviour
{
    [Header("Elementos de Texto")]
    [SerializeField] private TMP_Text mainQuestionText; // <--- NUEVO: Para poner la pregunta del CaseData
    [SerializeField] private TMP_Text validationErrorText; // Para mensajes como "Selecciona ambos"

    [Header("Selectores de Víctima")]
    [SerializeField] private Button btnVictimRosa;
    [SerializeField] private Button btnVictimXuxi;
    [SerializeField] private Image highlightVictimRosa;
    [SerializeField] private Image highlightVictimXuxi;

    [Header("Selectores de Acosador")]
    [SerializeField] private Button btnBullyRosa;
    [SerializeField] private Button btnBullyXuxi;
    [SerializeField] private Image highlightBullyRosa;
    [SerializeField] private Image highlightBullyXuxi;

    [Header("Control")]
    [SerializeField] private Button btnComprobar;

    // Colores
    private Color colorSelected = Color.green;
    private Color colorUnselected = new Color(1, 1, 1, 0f);

    private string selectedVictim = "";
    private string selectedBully = "";

    // Variables para guardar los textos que vienen del CaseData
    private string successFeedback;
    private string failFeedback;

    // Callback: Devuelve (esCorrecto, textoAMostrar)
    private System.Action<bool, string> onResultCallback;

    private void Start()
    {
        btnVictimRosa.onClick.AddListener(() => SetVictim("Rosa"));
        btnVictimXuxi.onClick.AddListener(() => SetVictim("Xuxi"));

        btnBullyRosa.onClick.AddListener(() => SetBully("Rosa"));
        btnBullyXuxi.onClick.AddListener(() => SetBully("Xuxi"));

        btnComprobar.onClick.AddListener(CheckAnswers);
    }

    // --- NUEVO MÉTODO DE CONFIGURACIÓN ---
    public void SetupPanel(string question, string successText, string failText, System.Action<bool, string> callback)
    {
        // 1. Llenamos los textos desde el CaseData
        if (mainQuestionText != null) mainQuestionText.text = question;

        successFeedback = successText;
        failFeedback = failText;
        onResultCallback = callback;

        // 2. Reiniciamos la interfaz
        gameObject.SetActive(true);
        ResetUI();
    }

    private void ResetUI()
    {
        selectedVictim = "";
        selectedBully = "";
        if (validationErrorText != null) validationErrorText.text = "";
        UpdateVisuals();
    }

    private void SetVictim(string person)
    {
        selectedVictim = person;
        UpdateVisuals();
    }

    private void SetBully(string person)
    {
        selectedBully = person;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (highlightVictimRosa != null) highlightVictimRosa.color = (selectedVictim == "Rosa") ? colorSelected : colorUnselected;
        if (highlightVictimXuxi != null) highlightVictimXuxi.color = (selectedVictim == "Xuxi") ? colorSelected : colorUnselected;

        if (highlightBullyRosa != null) highlightBullyRosa.color = (selectedBully == "Rosa") ? colorSelected : colorUnselected;
        if (highlightBullyXuxi != null) highlightBullyXuxi.color = (selectedBully == "Xuxi") ? colorSelected : colorUnselected;
    }

    private void CheckAnswers()
    {
        // Validación de interfaz (si faltan selecciones)
        if (string.IsNullOrEmpty(selectedVictim) || string.IsNullOrEmpty(selectedBully))
        {
            if (validationErrorText != null) validationErrorText.text = "Debes seleccionar una víctima y un acosador.";
            return;
        }

        if (selectedVictim == selectedBully)
        {
            if (validationErrorText != null) validationErrorText.text = "La misma persona no puede ser ambos.";
            return;
        }

        // --- LÓGICA DE GANAR / PERDER ---
        // Correcto: Victima Rosa, Acosador Xuxi
        bool isCorrect = (selectedVictim == "Rosa" && selectedBully == "Xuxi");

        // Seleccionamos el texto correcto del CaseData
        string finalFeedbackText = isCorrect ? successFeedback : failFeedback;

        // Cerramos y enviamos al UIManager
        gameObject.SetActive(false);
        onResultCallback?.Invoke(isCorrect, finalFeedbackText);
    }
}