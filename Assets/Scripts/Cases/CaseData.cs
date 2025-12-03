using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CaseProgress;

[CreateAssetMenu(fileName = "NewCaseData", menuName = "AgentesI/Case Data")]
public class CaseData : ScriptableObject
{
    [Header("Identificación del caso")]
    [Tooltip("Identificador único del caso (por ejemplo: Caso_01)")]
    public string caseID;

    [Header("Título e introducción")]
    public string caseTitle;
    [TextArea(2, 5)]
    public string caseIntro;

    [Header("Información contextual (Infos del caso)")]
    [TextArea(2, 5)]
    public string[] infoTexts;

    [Header("Pistas")]
    public ClueData[] clues;

    [Header("Evaluación final")]
    [TextArea(2, 5)]
    public string question;
    public string optionA;
    public string optionB;

    [TextArea(2, 5)]
    public string feedbackA;
    [TextArea(2, 5)]
    public string feedbackB;

    [Header("Detalles para el menú del Hub")]
    [TextArea(2, 5)]
    public string caseDescription;   // Breve texto para la previsualización en el menú

    public string sceneName;         // Nombre EXACTO de la escena en Build Settings

    [Header("Sprites del menú")]
    public Sprite previewImage;      // Imagen normal del caso
    public Sprite completedPreview;  
}

[System.Serializable]
public class ClueData
{
    [TextArea(2, 5)]
    public string clueText;

    public bool hasDecision;

    [TextArea(2, 5)]
    public string decisionQuestion;
    public string decisionA;
    public string decisionB;

    [TextArea(2, 5)]
    public string feedbackA;
    [TextArea(2, 5)]
    public string feedbackB;
}
