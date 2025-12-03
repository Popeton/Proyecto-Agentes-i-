using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterText : MonoBehaviour
{
    [Header("Configuración general")]
    public float typingSpeed = 0.04f;

    [Tooltip("Si está activo, el texto se escribirá automáticamente al habilitar el objeto.")]
    [SerializeField] private bool _playOnEnable = true;
    public bool playOnEnable => _playOnEnable; // propiedad pública de solo lectura

    [Header("Cursor de tipeo")]
    [SerializeField] private bool showCursor = true;
    [SerializeField] private string cursorChar = "|";

    [Header("Cursor parpadeante al final")]
    [SerializeField] private bool blinkCursorAtEnd = false;
    [SerializeField] private float blinkSpeed = 0.5f;

    private TMP_Text textComponent;
    private Coroutine typingRoutine;
    private Coroutine blinkRoutine;
    private string fullText = "";

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        if (textComponent != null)
            fullText = textComponent.text;
    }

    private void OnEnable()
    {
        if (_playOnEnable)
            PlayTypewriter();
    }

    /// <summary>
    /// Muestra un texto nuevo aplicando el efecto de tipeo.
    /// </summary>
    public void ShowText(string newText)
    {
        if (textComponent == null) return;
        fullText = newText;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText());
    }

    /// <summary>
    /// Reproduce el efecto de tipeo con el texto actual.
    /// </summary>
    public void PlayTypewriter()
    {
        if (textComponent == null) return;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        textComponent.text = "";

        for (int i = 0; i <= fullText.Length; i++)
        {
            string current = fullText.Substring(0, i);
            if (showCursor && i < fullText.Length)
                current += cursorChar;

            textComponent.text = current;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Si debe parpadear el cursor al final
        if (blinkCursorAtEnd)
        {
            blinkRoutine = StartCoroutine(BlinkCursor());
        }
        else
        {
            textComponent.text = fullText; // mostrar texto completo sin cursor
        }
    }

    private IEnumerator BlinkCursor()
    {
        bool visible = true;
        while (true)
        {
            textComponent.text = visible ? $"{fullText}{cursorChar}" : fullText;
            visible = !visible;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
