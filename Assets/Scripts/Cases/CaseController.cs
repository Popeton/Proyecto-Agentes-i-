using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CaseController : MonoBehaviour
{
    [Header("Datos del caso")]
    [SerializeField] private CaseData caseData;
    [SerializeField] private UIManager _uiManager;

    [Header("Elementos interactivos")]
    [SerializeField] private Clue[] clues;
    [SerializeField] private InfoClickable[] infoClickables;

    [Header("Referencias de UI Extra")]
    [SerializeField] private CluesMenuController cluesMenu;

    private int cluesFound = 0;
    private int infosFound = 0;
    private bool caseCompleted = false;
    private bool waitingForDecision = false;

    // VARIABLES NUEVAS PARA LAS CORRECCIONES
    private int totalActiveClues = 0; // El número real de pistas en la escena
    private bool isInteractionBusy = false; // Bloqueo temporal mientras lees una pista

    public UIManager uiManager => _uiManager;

    private void Start()
    {
        if (caseData == null)
        {
            Debug.LogError("Falta asignar CaseData en el inspector.");
            return;
        }

        GameProgressManager.Instance.lastPlayedCaseID = caseData.caseID;

        AssignInfoClickables();
        LoadSavedInfos(); // Este ya funcionaba bien para las infos

        // El orden es importante: Primero asignamos y contamos pistas...
        AssignCluesData();


        // ... Y LUEGO actualizamos el UI con el valor correcto (ya no será 0)
        _uiManager.UpdateClueCounter(cluesFound, totalActiveClues);

        _uiManager.ShowIntro(caseData.caseIntro);
    }

    public CaseData GetCaseData()
    {
        return caseData;
    }

    private void AssignCluesData()
    {
        if (clues == null || caseData.clues == null) return;

        // 1. Calcular total real
        totalActiveClues = Mathf.Min(clues.Length, caseData.clues.Length);

        // 2. Resetear contador local para evitar duplicados al reiniciar
        cluesFound = 0;

        // 3. Desactivar sobrantes
        for (int i = totalActiveClues; i < clues.Length; i++)
        {
            clues[i].gameObject.SetActive(false);
        }

        // 4. Inicializar y CONTAR
        for (int i = 0; i < totalActiveClues; i++)
        {
            clues[i].gameObject.SetActive(true);
            clues[i].Initialize(this, caseData.clues[i], i, caseData.caseID);

            // --- CORRECCIÓN CLAVE ---
            // Preguntamos a la pista si ya fue encontrada en una sesión anterior.
            // Si sí, sumamos al contador del controlador inmediatamente.
            if (clues[i].IsAlreadyFound())
            {
                cluesFound++;
            }
        }
    }

    private void AssignInfoClickables()
    {
        if (infoClickables == null || caseData.infoTexts == null) return;

        int total = Mathf.Min(infoClickables.Length, caseData.infoTexts.Length);
        for (int i = 0; i < total; i++)
        {
            infoClickables[i].Initialize(this, i, caseData.caseID);
            infoClickables[i].SetInfoText(caseData.infoTexts[i]);
        }
    }

    private void LoadSavedInfos()
    {
        infosFound = 0;
        for (int i = 0; i < caseData.infoTexts.Length; i++)
        {
            string key = $"{caseData.caseID}_Info_{i}";
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                infosFound++;
            }
        }
    }

    // -------------------------------------------------------------
    // SISTEMA DE BLOQUEO (NUEVO)
    // -------------------------------------------------------------
    public bool IsInteractionBusy()
    {
        return isInteractionBusy;
    }

    public void SetInteractionBusy(bool busy)
    {
        isInteractionBusy = busy;

        if (busy && cluesMenu != null)
        {
            cluesMenu.CloseMenu();
        }
    }

    // -------------------------------------------------------------
    // PISTAS
    // -------------------------------------------------------------
    public void OnClueFound(ClueData data, float waitDuration) // <--- NUEVO PARÁMETRO
    {
        if (caseCompleted) return;

        cluesFound++;
        GameProgressManager.Instance.AddClueFound(caseData.caseID);

        _uiManager.UpdateClueCounter(cluesFound, totalActiveClues);

        if (data.hasDecision)
        {
            waitingForDecision = true;
            _uiManager.OnDecisionAnswered += OnDecisionResolved;
        }

        // Si es la última pista, iniciamos la evaluación
        if (cluesFound >= totalActiveClues)
        {
            if (data.hasDecision) return; // Si tiene decisión interna, esperamos a que se resuelva esa primero

            // USAMOS EL TIEMPO CALCULADO POR LA PISTA + un pequeño margen (0.5s)
            StartCoroutine(ShowEvaluationWithDelay(waitDuration + 0.5f));
        }
    }

    private void OnDecisionResolved()
    {
        if (!waitingForDecision) return;

        waitingForDecision = false;
        _uiManager.OnDecisionAnswered -= OnDecisionResolved;

        if (cluesFound >= totalActiveClues)
        {
            // Si venimos de una decisión, usamos un tiempo estándar o corto
            // ya que el jugador acaba de interactuar con el panel de decisión.
            StartCoroutine(ShowEvaluationWithDelay(1.0f));
        }
    }

    public void OnInfoFound()
    {
        if (caseCompleted) return;
        infosFound++;
        GameProgressManager.Instance.AddInfoFound(caseData.caseID);
    }

    
    private IEnumerator ShowEvaluationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Clue[] allClues = FindObjectsOfType<Clue>();
        foreach (var c in allClues) c.HideTooltipInstant();

        if (caseData.caseID == "Caso_02_CyberBullying")
        {
            // Pasamos 'this' al final
            _uiManager.ShowCase2Evaluation(
                caseData.question,
                caseData.feedbackA, // Recuerda: Feedback A es el Correcto en tu lógica actual
                caseData.feedbackB, // Feedback B es el Incorrecto
                this                // <--- CORRECCIÓN AQUÍ: Pasamos el controlador
            );
        }
        else
        {
            _uiManager.ShowEvaluation(
                caseData.question,
                caseData.optionA,
                caseData.optionB,
                caseData.feedbackA,
                caseData.feedbackB,
                this
            );
        }
    }

    public void ResetCase()
    {
        cluesFound = 0;
        infosFound = 0;
        caseCompleted = false;
        waitingForDecision = false;
        isInteractionBusy = false; // Resetear bloqueo

        foreach (var clue in clues)
            clue.ResetClueState();

        _uiManager.HideAllPanels();
        // CORRECCIÓN: Resetear contador con total real
        _uiManager.UpdateClueCounter(0, totalActiveClues);
    }

    public void CompleteCase()
    {
        caseCompleted = true;
        GameProgressManager.Instance.UpdateCaseProgress(
            caseData.caseID,
            infosFound,
            cluesFound,
            true
        );
        SceneManager.LoadScene("Menu");
    }
}