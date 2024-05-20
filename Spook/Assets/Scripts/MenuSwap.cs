using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public struct ButtonImagePair
    {
        public Button button;
        public Texture defaultImage;
        public Texture hoverImage;
    }

    public ButtonImagePair[] buttonImagePairs;

    private Texture defaultTexture;
    private Button currentButton;

    private void Awake()
    {
        defaultTexture = buttonImagePairs[0].defaultImage; // Assuming the first image in the array represents the default image
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentButton = eventData.pointerEnter.GetComponent<Button>();
        if (currentButton != null)
        {
            foreach (ButtonImagePair pair in buttonImagePairs)
            {
                if (pair.button == currentButton)
                {
                    RawImage rawImage = pair.button.GetComponentInChildren<RawImage>();
                    if (rawImage != null)
                    {
                        rawImage.texture = pair.hoverImage;
                        break;
                    }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentButton != null)
        {
            foreach (ButtonImagePair pair in buttonImagePairs)
            {
                if (pair.button == currentButton)
                {
                    RawImage rawImage = pair.button.GetComponentInChildren<RawImage>();
                    if (rawImage != null)
                    {
                        rawImage.texture = pair.defaultImage;
                        break;
                    }
                }
            }
            currentButton = null;
        }
    }
}
