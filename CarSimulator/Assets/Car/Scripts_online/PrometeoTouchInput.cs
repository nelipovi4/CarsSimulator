using System;
using UnityEngine;
using UnityEngine.Events;

public class PrometeoTouchInput_on : MonoBehaviour
{
    public bool changeScaleOnPressed = false;
    [HideInInspector] public bool buttonPressed = false;
    RectTransform rectTransform;
    Vector3 initialScale;
    float scaleDownMultiplier = 0.85f;

    public UnityEvent OnButtonDown;
    public UnityEvent OnButtonUp;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
    }

    public void ButtonDown()
    {
        buttonPressed = true;
        if (changeScaleOnPressed) rectTransform.localScale = initialScale * scaleDownMultiplier;

        OnButtonDown?.Invoke(); // Событие для машины
    }

    public void ButtonUp()
    {
        buttonPressed = false;
        if (changeScaleOnPressed) rectTransform.localScale = initialScale;

        OnButtonUp?.Invoke(); // Событие для машины
    }
}
