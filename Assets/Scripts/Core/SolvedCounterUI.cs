using UnityEngine;
using TMPro;

public class SolvedCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private string prefixText = "Casos Resueltos: ";

    private void Start()
    {
        UpdateCounter();
    }

    private void OnEnable()
    {
        UpdateCounter();
    }

    public void UpdateCounter()
    {
        if (GameProgressManager.Instance == null) return;

        // Ahora son 4 casos jugables (excluyendo la insignia)
        // Ojo: Asegúrate de que 'allCases' en GameProgressManager tenga los 4 casos asignados.
        int totalCases = 4;
        int solvedCount = GameProgressManager.Instance.GetCompletedCasesCount();

        // Evitamos que muestre más de 4 si hay algún error de conteo
        solvedCount = Mathf.Min(solvedCount, totalCases);

        if (counterText != null)
        {
            counterText.text = $"{prefixText}{solvedCount} / {totalCases}";
        }
    }
}