using UnityEngine;
using System.Collections;

public class MenuTransitionManager : MonoBehaviour
{
    public GameObject MenuA;
    public GameObject MenuB;
    public float FadeTime = 1.5F;

    public void TransitionOut()
    {
        StartCoroutine(FadeInOut(MenuA, MenuB));
    }

    public void TransitionIn()
    {
        StartCoroutine(FadeInOut(MenuB, MenuA));
    }

    private IEnumerator FadeInOut(GameObject FadeOutMenu, GameObject FadeInMenu)
    {
        CanvasGroup FadeOutGroup = FadeOutMenu.GetComponent<CanvasGroup>();
        CanvasGroup FadeInGroup = FadeInMenu.GetComponent<CanvasGroup>();

        float TimeElapsed = 0f;

        // Fade out the current screen
        while (TimeElapsed < FadeTime)
        {
            FadeOutGroup.alpha = Mathf.Lerp(1f, 0f, TimeElapsed / FadeTime);
            TimeElapsed += Time.deltaTime;
            yield return null;
        }
        FadeOutGroup.alpha = 0f;
        FadeOutMenu.SetActive(false);

        // Activate and fade in the new screen
        FadeInMenu.SetActive(true);
        TimeElapsed = 0f;
        while (TimeElapsed < FadeTime)
        {
            FadeInGroup.alpha = Mathf.Lerp(0f, 1f, TimeElapsed / FadeTime);
            TimeElapsed += Time.deltaTime;
            yield return null;
        }
        FadeInGroup.alpha = 1f;
    }
}
