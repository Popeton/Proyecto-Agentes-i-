using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    public static OrientationManager Instance;

    [Header("Panel de Aviso")]
    [Tooltip("El panel que cubre toda la pantalla pidiendo girar el dispositivo.")]
    [SerializeField] private GameObject portraitWarningPanel;

    private void Awake()
    {
        // PATRÓN SINGLETON
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- CORRECCIÓN AQUÍ ---
    // Usamos Update() porque funciona aunque el Time.timeScale sea 0.
    private void Update()
    {
        CheckOrientation();
    }

    private void CheckOrientation()
    {
        if (portraitWarningPanel == null) return;

        // Si el Alto es mayor que el Ancho = Vertical (Portrait)
        bool isPortrait = Screen.height > Screen.width;

        // Solo actuamos si el estado cambió para no gastar recursos
        if (portraitWarningPanel.activeSelf != isPortrait)
        {
            portraitWarningPanel.SetActive(isPortrait);

            // AHORA SÍ FUNCIONA:
            // Al poner 0, pausamos el juego. Al poner 1, lo reanudamos.
            // Como estamos en Update(), el script sigue vivo para detectar el cambio a 1.
            Time.timeScale = isPortrait ? 0f : 1f;
        }
    }
}