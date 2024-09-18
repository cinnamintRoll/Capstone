using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class AnimatedButton : Button
{
    [Header("Sprite Swap")]
    public Sprite normalSprite;
    public Sprite highlightedSprite;
    public Sprite pressedSprite;
    public Sprite disabledSprite;
    private Image buttonImage;

    [Header("Animation")]
    public Animator buttonAnimator;
    public string normalState = "Normal";
    public string highlightedState = "Highlighted";
    public string pressedState = "Pressed";
    public string disabledState = "Disabled";

    protected override void Awake()
    {
        base.Awake();
        buttonImage = GetComponent<Image>();
        if (buttonAnimator == null)
        {
            Debug.LogError("Animator component is not assigned!");
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(highlightedState);
        }
        if (buttonImage != null)
        {
            buttonImage.sprite = highlightedSprite;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(normalState);
        }
        if (buttonImage != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(pressedState);
        }
        if (buttonImage != null)
        {
            buttonImage.sprite = pressedSprite;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(normalState);
        }
        if (buttonImage != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(disabledState);
        }
        if (buttonImage != null)
        {
            buttonImage.sprite = disabledSprite;
        }
    }
    /*
    protected override void Press()
    {
        base.Press();
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(pressedState);
        }
    }
    */
}
