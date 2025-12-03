using UnityEngine;
using UnityEngine.UI;

public class CaseMedalsUI : MonoBehaviour
{
    [System.Serializable]
    public struct CaseMedalSlot
    {
        [Tooltip("El ID exacto del caso en el CaseData (ej: Case_01)")]
        public string caseID;

        [Tooltip("La imagen UI en la escena que cambiará")]
        public Image targetImage;

        [Header("Sprites")]
        public Sprite lockedSprite;   // Imagen gris/bloqueada
        public Sprite completedSprite; // Imagen a color/completada
    }

    [Header("Configuración de las 4 Medallas")]
    public CaseMedalSlot[] medals;

    private void Start()
    {
        UpdateMedals();
    }

    private void OnEnable()
    {
        UpdateMedals();
    }

    public void UpdateMedals()
    {
        if (GameProgressManager.Instance == null) return;

        foreach (var medal in medals)
        {
            // 1. Buscamos el progreso de este caso específico
            var progress = GameProgressManager.Instance.GetProgress(medal.caseID);

            if (progress != null && medal.targetImage != null)
            {
                // 2. Si está completado, ponemos el sprite de color. Si no, el bloqueado.
                if (progress.isCompleted)
                {
                    if (medal.completedSprite != null)
                        medal.targetImage.sprite = medal.completedSprite;
                }
                else
                {
                    if (medal.lockedSprite != null)
                        medal.targetImage.sprite = medal.lockedSprite;
                }
            }
        }
    }
}