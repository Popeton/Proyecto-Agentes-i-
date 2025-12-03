using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class ClickableBase : MonoBehaviour, IPointerClickHandler
{
    [Header("Tooltip")]
    [SerializeField] protected GameObject tooltipPanel;
    [SerializeField] protected TMP_Text tooltipText;
    [SerializeField] protected TypewriterText tooltipTypewriter;

    [Header("Sprite Visual")]
    [SerializeField] protected Image iconImage;
    [SerializeField] protected Sprite defaultSprite;
    [SerializeField] protected Sprite clickedSprite;

    protected CaseController controller;
    protected bool isFound = false;

    protected virtual void Awake()
    {
        if (iconImage == null) iconImage = GetComponent<Image>();
        if (iconImage != null) iconImage.raycastTarget = true;
    }

    protected virtual void Start()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (iconImage != null && defaultSprite != null && !isFound)
            iconImage.sprite = defaultSprite;
    }

    public virtual void Initialize(CaseController caseController)
    {
        controller = caseController;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // CORRECCIÓN: Ya NO llamamos a HideAllTooltips() aquí.
            // Dejamos que OnClicked decida si debe cerrar cosas o no.
            OnClicked();
        }
    }

    protected abstract void OnClicked();

    protected void ApplyClickedSprite()
    {
        if (iconImage != null && clickedSprite != null)
            iconImage.sprite = clickedSprite;
    }

    // Cambiamos a 'protected' para que los hijos puedan usarlo
    protected void HideAllTooltips()
    {
        var allClickables = FindObjectsOfType<ClickableBase>();
        foreach (var click in allClickables)
        {
            if (click != this)
                click.HideTooltipInstant();
        }
    }

    public void HideTooltipInstant()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}