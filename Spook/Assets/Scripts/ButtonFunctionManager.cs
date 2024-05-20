using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonFunctionManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    public Color OnHoverAlpha;

    [Header("Audio Settings")]
    [SerializeField] AudioClip ClickSound;
    [SerializeField] AudioClip HoverSound;
    private Color DefaultAlpha;
    private Image ButtonIMG;
    private Button Button;

    private void Awake()
    {
        Button = GetComponent<Button>();
        ButtonIMG = GetComponent<Image>();
        Button.onClick.AddListener(OnClickSound);

        DefaultAlpha = ButtonIMG.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ButtonIMG.color = OnHoverAlpha; AudioManager.Instance.PlaySound(HoverSound);                  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ButtonIMG.color = DefaultAlpha;
    }

    public void OnClickSound()
    {
        if (ClickSound != null) AudioManager.Instance.PlaySound(ClickSound);
        else Debug.LogError("Click sound not assigned in ButtonFunctionManager.");
    }
}
