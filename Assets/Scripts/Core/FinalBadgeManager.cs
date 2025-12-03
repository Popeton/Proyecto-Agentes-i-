using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions; // <--- VITAL PARA LA LIMPIEZA

public class FinalBadgeManager : MonoBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private GameObject badgePanel;

    [Header("Elementos de Entrada")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button btnConfirm;

    [Header("Feedback al Usuario")]
    [SerializeField] private TMP_Text errorFeedbackText;

    [Header("Elementos del Diploma")]
    [SerializeField] private TMP_Text badgeAgentNameText;
    [SerializeField] private Button btnReturnToMenu;

    private const string PLAYER_NAME_KEY = "AgentName";

    // LISTA NEGRA AMPLIADA (Raíces de palabras)
    // Nota: Al usar Contains, "puta" bloqueará "diputado". 
    // Para un juego infantil es mejor ser estricto, pero ten cuidado con palabras cortas.
    private readonly string[] badWordsRoots = { 
        // --- ESPAÑOL ---
        "puta", "puto", "mierda", "verga", "pene", "vagina", "culo", "ano",
        "idiot", "estupid", "imbecil", "tarad", "mongol", "retrasad",
        "boba", "bobo", "inutil", "pendej", "burra", "burro",
        "bestia", "bruto", "bruta", "babos", "tont",
        
        // --- INGLÉS  ---
        "fuck", "shit", "bitch", "asshole", "cunt", "whore", "slut",
        "dick", "cock", "pussy", "bastard", "nigger", "faggot", "suck",
        "crap", "damn", "sex", "porn", "nude",
        
        // --- TEMAS SENSIBLES  ---
        "matar", "muerte", "suicid", "asesin", "sangre", "mori", "kill", "die",
        "sexo", "sexual", "xxx", "tetas", "senos", "boobs",
        "droga", "cocaina", "marihuana", "vicio", "weed", "drugs",
        "nazi", "hitler", "kkk", "violad", "abusad", "pedof", "rape",
        
        // ---  LATINOS ---
        "malparid", "gonorrea", "pirobo", "marica", "zorra", "perra",
        "cabron", "mamaguevo", "joder", "coño", "culero", "boludo",
        "pinche", "vergu", "mamad"
    };

    private void Start()
    {
        inputPanel.SetActive(true);
        badgePanel.SetActive(false);

        if (errorFeedbackText != null) errorFeedbackText.text = "";

        // Límite visual de caracteres
        nameInputField.characterLimit = 12;

        if (PlayerPrefs.HasKey(PLAYER_NAME_KEY))
        {
            nameInputField.text = PlayerPrefs.GetString(PLAYER_NAME_KEY);
        }

        btnConfirm.onClick.AddListener(ValidateAndGenerate);
        btnReturnToMenu.onClick.AddListener(ReturnToMenu);
    }

    public void ValidateAndGenerate()
    {
        string rawName = nameInputField.text.Trim();

        // 1. VALIDACIÓN: Vacío
        if (string.IsNullOrEmpty(rawName))
        {
            ShowError("El nombre no puede estar vacío.");
            return;
        }

        // 2. VALIDACIÓN: Muy corto (menos de 3 letras reales)
        string cleanForLength = Regex.Replace(rawName, @"\s+", ""); // Quitar solo espacios
        if (cleanForLength.Length < 3)
        {
            ShowError("El nombre es muy corto.");
            return;
        }

        // 3. VALIDACIÓN PROFUNDA: Palabras ofensivas
        // Aquí pasamos el nombre "sucio" (ej: P.u.t.0) y lo convertimos a limpio (puto)
        string sanitizedName = SanitizeString(rawName);

        if (ContainsBadWords(sanitizedName))
        {
            ShowError("Ese nombre no es apropiado para un Agente.");
            Debug.Log($"Bloqueado: {rawName} -> Interpretado como: {sanitizedName}");
            nameInputField.text = "";
            return;
        }

        // SI PASA TODAS LAS PRUEBAS:
        GenerateBadge(rawName); // Usamos el nombre original para el diploma (si quieren poner Puntos o Emojis permitidos)
    }

    private void GenerateBadge(string validName)
    {
        PlayerPrefs.SetString(PLAYER_NAME_KEY, validName);
        PlayerPrefs.Save();

        badgeAgentNameText.text = $"Agente\n{validName.ToUpper()}";

        inputPanel.SetActive(false);
        badgePanel.SetActive(true);
    }

    // --- EL CEREBRO DE LA LIMPIEZA ---
    private string SanitizeString(string input)
    {
        // 1. Convertir a minúsculas
        string text = input.ToLower();

        // 2. Reemplazo de Leetspeak (Números/Símbolos que parecen letras)
        text = text.Replace("0", "o")
                   .Replace("1", "i")
                   .Replace("!", "i")
                   .Replace("3", "e")
                   .Replace("4", "a")
                   .Replace("@", "a")
                   .Replace("5", "s")
                   .Replace("$", "s")
                   .Replace("7", "t")
                   .Replace("+", "t")
                   .Replace("8", "b")
                   .Replace("9", "g");

        // 3. Normalización (Quitar tildes y caracteres especiales)
        // Á -> A, ñ -> n (aproximación para filtrar), etc.
        text = text.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u");

        // 4. ELIMINAR BASURA: Usamos Regex para borrar TODO lo que no sea una letra de la a-z
        // Esto elimina puntos, comas, guiones, espacios, emojis, etc.
        // Ej: "p.u - t. a" se convierte en "puta"
        text = Regex.Replace(text, "[^a-z]", "");

        return text;
    }

    private bool ContainsBadWords(string cleanInput)
    {
        foreach (string badWord in badWordsRoots)
        {
            if (cleanInput.Contains(badWord))
            {
                return true;
            }
        }
        return false;
    }

    private void ShowError(string message)
    {
        if (errorFeedbackText != null)
        {
            errorFeedbackText.text = message;
        }
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}