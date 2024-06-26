using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVisibilityController : MonoBehaviour
{
    // Set the visibility of the item
    public void SetVisibility(bool isVisible)
    {
        Renderer rendererComponent = GetComponent<Renderer>();
        CanvasRenderer canvasRendererComponent = GetComponent<CanvasRenderer>();

        if (rendererComponent != null)
        {
            // For regular GameObjects with Renderer component
            rendererComponent.enabled = isVisible;
        }
        else if (canvasRendererComponent != null)
        {
            // For UI elements with CanvasRenderer component
            canvasRendererComponent.SetAlpha(isVisible ? 1.0f : 0.0f);
        }
        else
        {
            Debug.LogError("No Renderer or CanvasRenderer Components");
        }
    }
}