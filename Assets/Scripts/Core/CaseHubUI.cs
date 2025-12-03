using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CaseHubUI : MonoBehaviour
{
    [Header("Panel de detalle del caso")]
    [SerializeField] private GameObject caseDetailPanel;

    [Header("Imagen informativa del caso")]
    [SerializeField] private Image caseMainInfoImage;
    [SerializeField] private Sprite[] caseInfoImages;
    // 0 = Grooming, 1 = Cyberbullying, 2 = Nomofobia, etc.

    [Header("Textos")]
    [SerializeField] private TypewriterText clueCountTypewriter;
    [SerializeField] private TypewriterText estadoCasoTypewriter;

   // [Header("Progreso")]
    //[SerializeField] private Slider clueProgressSlider;

    [Header("Botón iniciar")]
    [SerializeField] private Button btnIniciar;
    //[SerializeField] private TypewriterText btnIniciarText; // Opcional, para cambiar texto si es Replay

    private CaseData selectedCase;

    // --------------------------------------------------------------------
    // ACTUALIZAR INFORMACIÓN DEL PANEL DEL CASO
    // --------------------------------------------------------------------
    public void UpdateCasePanel(CaseData data)
    {
        selectedCase = data;
        caseDetailPanel.SetActive(true);

        // 1. IMAGEN PRINCIPAL
        int idx = GetCaseIndex(data);
        if (idx >= 0 && idx < caseInfoImages.Length)
            caseMainInfoImage.sprite = caseInfoImages[idx];

        // 2. OBTENER PROGRESO GUARDADO
        var progress = GameProgressManager.Instance.GetProgress(data.caseID);
        if (progress == null)
        {
            Debug.LogWarning($"No hay progreso registrado para {data.caseID}");
            return;
        }

        int cluesFound = progress.cluesFound;
        int totalClues = data.clues.Length;

        // 3. ACTUALIZAR TEXTOS Y SLIDERS (Solo Pistas)
        if (clueCountTypewriter != null)
            clueCountTypewriter.ShowText($"{cluesFound}/{totalClues}");

        //if (clueProgressSlider != null)
        //{
        //    // Evitar división por cero
        //    float percent = (totalClues > 0) ? (float)cluesFound / totalClues : 0f;
        //    clueProgressSlider.value = percent;
        //}

        // 4. ESTADO DEL CASO
        string estadoTexto = "EN PROGRESO";
        if (progress.isCompleted)
        {
            estadoTexto = "FINALIZADO";
        }

        if (estadoCasoTypewriter != null)
            estadoCasoTypewriter.ShowText(estadoTexto);

        // 5. CONFIGURACIÓN DEL BOTÓN INICIAR
        // Eliminamos la restricción de Rango (Feedback #9 - No linealidad)
        // El botón siempre aparece, pero podemos cambiar el texto si ya se completó.

       
        btnIniciar.gameObject.SetActive(!progress.isCompleted);
        btnIniciar.onClick.RemoveAllListeners();

        //  cambiar el texto del botón cuando ya se completó:
        //if (btnIniciarText != null)
        //{
        //    btnIniciarText.ShowText(progress.isCompleted ? "Repetir Caso" : "Iniciar Caso");
        //}

        btnIniciar.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(data.sceneName);
        });
    }

    // --------------------------------------------------------------------
    private int GetCaseIndex(CaseData data)
    {
        var list = GameProgressManager.Instance.allCases;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == data) return i;
        }
        return -1;
    }

    public void HidePanel()
    {
        caseDetailPanel.SetActive(false);
    }
}