using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.TextCore.Text;

public class BackgroundSwap : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] GameObject Character;
    [SerializeField] Animator CharacterAnimator;
    [SerializeField] AnimationClip[] CharacterAnimations;


    [SerializeField] RawImage Static;
    [SerializeField] float Duration = 0.4f;
    [SerializeField] float Delay = 0.4f;
    private float StartAlpha;
    private Coroutine FadeInOutCoroutine;

    private enum Buttons
    {
        NewGame,
        Continue,
        Settings,
        Exit
    }
    private void Awake()
    {
        StartAlpha = Static.color.a;
        CharacterAnimation(0); 
    }

    private IEnumerator FadeInOut(float TGTAlpha)
    {
        float CurrentTime = 0;
        Color StartCol = Static.color;
        Color TGTCol = new(StartCol.r, StartCol.g, StartCol.b, TGTAlpha);
        while (CurrentTime < 0.06f)
        {
            CurrentTime += Time.deltaTime;
            Static.color = Color.Lerp(StartCol, TGTCol, CurrentTime / 0.01f);
            yield return null;
        }
        Static.color = TGTCol;

        yield return new WaitForSeconds(Delay);

        CurrentTime = 0;
        StartCol = Static.color;
        TGTCol = new Color(StartCol.r, StartCol.g, StartCol.b, StartAlpha);
        while (CurrentTime < Duration)
        {
            CurrentTime += Time.deltaTime;
            Static.color = Color.Lerp(StartCol, TGTCol, CurrentTime / Duration);
            yield return null;
        }
        Static.color = TGTCol;
    }

    private void CharacterAnimation(int Index)
    {
        if (CharacterAnimator != null && CharacterAnimations.Length > Index) CharacterAnimator.Play(CharacterAnimations[Index].name);
    }

    private void SwitchBackgroundImage(string buttonName)
    {
        if (FadeInOutCoroutine != null)
        {
            StopCoroutine(FadeInOutCoroutine);
        }

        FadeInOutCoroutine = StartCoroutine(FadeInOut(1f));

        switch (buttonName)
        {
            case nameof(Buttons.NewGame):
                CharacterAnimation(0);
                break;
            case nameof(Buttons.Continue):
                CharacterAnimation(1);
                break;
            case nameof(Buttons.Settings):
                CharacterAnimation(2);
                break;
            case nameof(Buttons.Exit):
                CharacterAnimation(3);
                break;
            default:               
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string buttonName = gameObject.name;
        SwitchBackgroundImage(buttonName);
    }
}
