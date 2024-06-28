using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimationManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Movement Settings")]
    [SerializeField] private float MoveDistance = 10;
    private Button Button;
    private Vector3 InitialPosition;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        InitialPosition = Button.transform.position;
        Vector3 TargetPosition = InitialPosition + new Vector3(MoveDistance, 0, 0);
        currentCoroutine = StartCoroutine(MoveButton(TargetPosition));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        Vector3 TargetPosition = InitialPosition;
        currentCoroutine = StartCoroutine(MoveButton(TargetPosition));
    }

    private IEnumerator MoveButton(Vector3 TargetPosition)
    {
        float TimeElapsed = 0;
        float MoveDuration = 0.2F;

        Vector3 currentPosition = InitialPosition;

        while (TimeElapsed < MoveDuration)
        {
            currentPosition = Vector3.Lerp(InitialPosition, TargetPosition, TimeElapsed / MoveDuration);
            Button.transform.position = currentPosition;

            TimeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        currentCoroutine = null; // Reset the coroutine reference
    }
}
