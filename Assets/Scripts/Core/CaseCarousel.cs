using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CaseCarousel : MonoBehaviour
{
    [Header("Posiciones del carrusel")]
    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform rightPos;

    [Header("Displays (3 imágenes que rotan)")]
    [SerializeField] private Image[] displays;

    [Header("Botones")]
    [SerializeField] private Button btnLeft;
    [SerializeField] private Button btnRight;
    [SerializeField] private Button btnOpen;
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnExit;

    [Header("Sprites de la insignia")]
    [SerializeField] private Sprite insigniaLockedSprite;
    [SerializeField] private Sprite insigniaUnlockedSprite;

    [Header("Paneles UI")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject caseOpenPanel;
    [SerializeField] private CaseHubUI caseHubUI;  
    [SerializeField] private GameObject Carousel;  
    
    private List<CaseData> cases = new List<CaseData>();
    private int index = 0;

    void Start()
    {
        cases.AddRange(GameProgressManager.Instance.allCases);

        btnLeft.onClick.AddListener(() => Move(-1));
        btnRight.onClick.AddListener(() => Move(1));
        btnOpen.onClick.AddListener(() => OpenCasePanel());
        btnStart.onClick.AddListener(() => StartSelectedCase());
        btnExit.onClick.AddListener(() => ReturnToMenu());

        UpdateCarousel();


        //AutoShowLastCaseSummary();
        AutoShowCurrentContext();
    }


    private void Move(int dir)
    {
        index = (index + dir + cases.Count + 1) % (cases.Count + 1);
        UpdateCarousel();
    }

    private void UpdateCarousel()
    {
        int leftIndex = (index - 1 + cases.Count + 1) % (cases.Count + 1);
        int rightIndex = (index + 1) % (cases.Count + 1);

        UpdateDisplay(displays[0], leftIndex, leftPos, 0.8f);
        UpdateDisplay(displays[1], index, centerPos, 1.0f);
        UpdateDisplay(displays[2], rightIndex, rightPos, 0.8f);

        UpdateStartButton();
    }

    private void UpdateDisplay(Image display, int caseIndex, Transform targetPos, float scale)
    {
        // La insignia es el último elemento (índice igual a la cantidad de casos)
        bool isInsignia = caseIndex == cases.Count;

        if (isInsignia)
        {
           
            // Preguntamos al Manager si YA se completaron todos los casos.
            bool allCompleted = GameProgressManager.Instance.AreAllCasesCompleted();

            // Si todo está listo -> Sprite Desbloqueado (Color/Brillante)
            // Si falta algo -> Sprite Bloqueado (Candado/Gris)
            display.sprite = allCompleted ? insigniaUnlockedSprite : insigniaLockedSprite;
        }
        else
        {
            // LÓGICA DE LOS CASOS (1 al 4):
            // Siempre mostramos el preview normal (NO LINEALIDAD).
            // Solo cambiamos a "CompletedPreview" si ya se jugó, pero nunca bloqueamos la entrada.
            var cd = cases[caseIndex];
            var progress = GameProgressManager.Instance.GetProgress(cd.caseID);

            if (progress.isCompleted)
                display.sprite = cd.completedPreview;
            else
                display.sprite = cd.previewImage;
        }

        display.transform.position = targetPos.position;
        display.transform.localScale = Vector3.one * scale;
    }

    private void AutoShowCurrentContext()
    {
        // 1. Buscamos dónde estaba el jugador la última vez
        string targetID = GameProgressManager.Instance.lastPlayedCaseID;

        // Si no ha jugado nada aún (primera vez que abre el juego), no hacemos nada
        if (string.IsNullOrEmpty(targetID)) return;

        // 2. Buscamos ese caso en nuestra lista
        for (int i = 0; i < cases.Count; i++)
        {
            if (cases[i].caseID == targetID)
            {
                // 3. Movemos el carrusel a esa posición
                index = i;
                UpdateCarousel();

                // 4. Abrimos el panel de detalle automáticamente (como pediste)
                // Usamos la misma lógica que el botón "Abrir"
                OpenCasePanel();
                break;
            }
        }
    }
    private void AutoShowLastCaseSummary()
    {
        var last = GameProgressManager.Instance.GetLastCompletedCase();
        if (last == null) return;

        // Mover el carrusel al caso correcto
        for (int i = 0; i < cases.Count; i++)
        {
            if (cases[i].caseID == last.caseID)
            {
                index = i;
                UpdateCarousel();
                break;
            }
        }

        // Abrir panel con info actualizada
        caseOpenPanel.SetActive(true);
        caseHubUI.UpdateCasePanel(last);
    }

    private void UpdateStartButton()
    {
        bool isInsignia = index == cases.Count;

        if (isInsignia)
        {
            // SOLO mostramos el botón si TODOS los casos están hechos .
            bool allCompleted = GameProgressManager.Instance.AreAllCasesCompleted(); 
            btnOpen.gameObject.SetActive(allCompleted);

            // Opcional: Si tienes un texto en el botón, podrías cambiarlo aquí
            // ej: btnOpenText.text = "Reclamar Insignia";
        }
        else
        {
            // Para los casos normales, el botón SIEMPRE está activo (No linealidad)
            btnOpen.gameObject.SetActive(true);
        }
    }

    private void OpenCasePanel()
    {
        // 1. CASO ESPECIAL: LA INSIGNIA
        if (index == cases.Count)
        {
            // Aquí verificamos si ya completó todo (o usamos el truco 'true' para probar)
            if (GameProgressManager.Instance.AreAllCasesCompleted()) // <--- Recuerda cambiar esto por  al terminar de probar
            {
                // Cargamos la escena directamente sin pasar por el Hub
                SceneManager.LoadScene("FinalBagdet");
            }
            return;
        }

        // 2. CASOS NORMALES (1 al 4)
        // Aquí sí abrimos el panel de detalle
        caseOpenPanel.SetActive(true);

        var cd = cases[index];
        if (caseHubUI != null)
            caseHubUI.UpdateCasePanel(cd);
        else
            Debug.LogError("Falta asignar CaseHubUI en el carrusel.");
    }

    private void ReturnToMenu()
    {

        Carousel.SetActive(false);
        startMenu.SetActive(true);
    }

    private void StartSelectedCase()
    {
        // Si estamos en la posición de la insignia
        if (index == cases.Count)
        {
            // Verificación de seguridad extra GameProgressManager.Instance.AreAllCasesCompleted()
            if (true)
            {
                // Cargar la escena final donde pondremos la lógica del nombre
                // Asegúrate de que este nombre sea EXACTO al de tu escena en Unity
                SceneManager.LoadScene("Insignia_Final");
            }
            return;
        }

        // Si es un caso normal, cargamos su escena desde el ScriptableObject
        var cd = cases[index];
        SceneManager.LoadScene(cd.sceneName);
    }
}
