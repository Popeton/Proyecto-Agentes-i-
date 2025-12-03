using UnityEngine;

public class AutoTypeOnEnable : MonoBehaviour
{
    private TypewriterText[] typewriterTexts;

    private void Awake()
    {
        typewriterTexts = GetComponentsInChildren<TypewriterText>(true);
    }

    private void OnEnable()
    {
        foreach (var t in typewriterTexts)
        {
            if (t != null && t.enabled && t.gameObject.activeInHierarchy && t.playOnEnable)
            {
                t.PlayTypewriter();
            }
        }
    }
}
