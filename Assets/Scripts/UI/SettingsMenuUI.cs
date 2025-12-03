using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Botones Transparentes")]
    [SerializeField] private Button btnContinuar; // El área invisible sobre "Continuar"
    [SerializeField] private Button btnSalir;     // El área invisible sobre "Salir"

    [Header("Botones de Audio")]
    [SerializeField] private Button btnAudioOn;   // Área sobre el parlante con sonido
    [SerializeField] private Button btnAudioOff;  // Área sobre el parlante tachado

    [Header("Opcional: Feedback Visual")]
    [Tooltip("Arrastra aquí una imagen (marco o check) que se mueva sobre la opción elegida.")]
    [SerializeField] private RectTransform selectorHighlight;
    [Tooltip("Posición del selector en ON")]
    [SerializeField] private Transform posAudioOn;
    [Tooltip("Posición del selector en OFF")]
    [SerializeField] private Transform posAudioOff;

    private void Start()
    {
        // 1. Asignar los clicks
        btnContinuar.onClick.AddListener(CloseSettings);
        btnSalir.onClick.AddListener(HandleExit);

        btnAudioOn.onClick.AddListener(() => SetAudioState(true));
        btnAudioOff.onClick.AddListener(() => SetAudioState(false));

        // 2. Cargar estado inicial del audio
        bool isAudioOn = PlayerPrefs.GetInt("MasterAudio", 1) == 1;
        UpdateVisualSelector(isAudioOn);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f; // Pausa el juego al abrir
    }

    // ---------------------------------------------------------
    // LÓGICA DE AUDIO
    // ---------------------------------------------------------
    private void SetAudioState(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
        PlayerPrefs.SetInt("MasterAudio", isOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateVisualSelector(isOn);
    }

    private void UpdateVisualSelector(bool isOn)
    {
        // Si pusiste un selector (ej: un circulito o marco), lo movemos
        if (selectorHighlight != null && posAudioOn != null && posAudioOff != null)
        {
            selectorHighlight.gameObject.SetActive(true);
            selectorHighlight.position = isOn ? posAudioOn.position : posAudioOff.position;
        }
    }

    // ---------------------------------------------------------
    // LÓGICA DE SALIR (Contextual)
    // ---------------------------------------------------------
    private void HandleExit()
    {
        Time.timeScale = 1f; // Reanudar tiempo
        string currentScene = SceneManager.GetActiveScene().name;

        // Si estamos en el menú principal -> Cierra la App
        if (currentScene == "Menu" || currentScene == "MainMenu")
        {
            Debug.Log("Cerrando aplicación...");
            Application.Quit();
        }
        else
        {
            // Si estamos en un caso -> Vuelve al menú
            Debug.Log("Volviendo al Menú...");
            SceneManager.LoadScene("Menu");
        }
    }

    public void CloseSettings()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}