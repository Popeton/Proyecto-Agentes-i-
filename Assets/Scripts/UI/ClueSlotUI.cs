using UnityEngine;
using UnityEngine.UI;

public class ClueSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Button btnReRead;

    [Header("Sprites")]
    [SerializeField] private Sprite emptySprite; // La estrella punteada

    private ClueData myData;
    private UIManager uiManager;

    // Inicializa el slot. Se llama desde el CluesMenuController
    public void SetupSlot(ClueData data, int index, string caseID, UIManager manager)
    {
        myData = data;
        uiManager = manager;

        // 1. Verificamos si esta pista YA fue encontrada usando la misma clave que en Clue.cs
        string clueID = $"{caseID}_Clue_{index}";
        bool isFound = PlayerPrefs.GetInt(clueID, 0) == 1;

        // 2. Configuración Visual
        if (isFound)
        {
            // Si la encontramos: Ponemos su sprite real (asumiendo que ClueData tuviera sprite, 
            // OJO: Como ClueData no tiene sprite por defecto en tu script anterior, 
            // necesitamos asignarlo o usar un genérico. 
            // *Solución:* Asumiremos que el botón tiene una imagen hija o usamos el sprite del Clue asociado.
            // Para este ejemplo, activamos el botón para leer.

            // NOTA: Si tus ClueData NO tienen campo de Sprite, deberás agregarlo o 
            // pasarle el sprite desde el controlador. 
            // Por ahora, activaremos el color o interactividad.

            iconImage.color = Color.white; // Visible
            if (emptySprite != null) iconImage.sprite = emptySprite; // O el sprite específico si lo agregas al SO

            btnReRead.interactable = true;
        }
        else
        {
            // Si NO la encontramos: Estrella punteada y botón desactivado
            iconImage.sprite = emptySprite;
            iconImage.color = new Color(1, 1, 1, 0.5f); // Semi transparente
            btnReRead.interactable = false;
        }

        // 3. Configurar evento de click (Releer)
        btnReRead.onClick.RemoveAllListeners();
        if (isFound)
        {
            btnReRead.onClick.AddListener(ShowInfoAgain);
        }
    }

    // Método para inyectar el sprite correcto desde fuera (ya que ClueData no tiene sprite en tu código actual)
    public void SetIcon(Sprite icon)
    {
        if (iconImage != null && icon != null)
            iconImage.sprite = icon;
    }

    private void ShowInfoAgain()
    {
        if (uiManager != null && myData != null)
        {
            // Llamamos a una función nueva en UIManager para mostrar solo texto
            uiManager.ShowReadOnlyTooltip(myData.clueText);
        }
    }
}