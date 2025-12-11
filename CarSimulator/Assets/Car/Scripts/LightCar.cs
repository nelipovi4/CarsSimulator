using UnityEngine;
using UnityEngine.InputSystem;

public class LightCar : MonoBehaviour
{
    public GameObject Forward;
    public GameObject Back;
    public GameObject Turnsignal_R;
    public GameObject Turnsignal_L;

    private Keyboard kb;
    private bool isRightSignalOn = false;
    private bool isLeftSignalOn = false;

    // Локальные состояния
    private bool forwardOn = false;
    private bool backOn = false;
    private bool rightBlink = false;
    private bool leftBlink = false;

    private void Start()
    {
        Forward.SetActive(false);
        Back.SetActive(false);
        Turnsignal_R.SetActive(false);
        Turnsignal_L.SetActive(false);

        kb = Keyboard.current;
    }

    void Update()
    {
        // Передний свет
        if (kb.hKey.wasPressedThisFrame)
            forwardOn = true;

        if (kb.gKey.wasPressedThisFrame)
            forwardOn = false;

        // Задний свет
        if (kb.sKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
            backOn = true;

        if (kb.sKey.wasReleasedThisFrame || kb.spaceKey.wasReleasedThisFrame)
            backOn = false;

        // Правый поворотник
        if (kb.eKey.wasPressedThisFrame)
        {
            if (!isRightSignalOn)
            {
                if (isLeftSignalOn)
                {
                    isLeftSignalOn = false;
                    leftBlink = false;
                    CancelInvoke(nameof(BlinkLeftSignal));
                    Turnsignal_L.SetActive(false);
                }

                isRightSignalOn = true;
                rightBlink = true;
                InvokeRepeating(nameof(BlinkRightSignal), 0f, 0.5f);
            }
            else
            {
                isRightSignalOn = false;
                rightBlink = false;
                CancelInvoke(nameof(BlinkRightSignal));
                Turnsignal_R.SetActive(false);
            }
        }

        // Левый поворотник
        if (kb.qKey.wasPressedThisFrame)
        {
            if (!isLeftSignalOn)
            {
                if (isRightSignalOn)
                {
                    isRightSignalOn = false;
                    rightBlink = false;
                    CancelInvoke(nameof(BlinkRightSignal));
                    Turnsignal_R.SetActive(false);
                }

                isLeftSignalOn = true;
                leftBlink = true;
                InvokeRepeating(nameof(BlinkLeftSignal), 0f, 0.5f);
            }
            else
            {
                isLeftSignalOn = false;
                leftBlink = false;
                CancelInvoke(nameof(BlinkLeftSignal));
                Turnsignal_L.SetActive(false);
            }
        }

        // Применяем локально
        Forward.SetActive(forwardOn);
        Back.SetActive(backOn);
    }

    void BlinkRightSignal()
    {
        Turnsignal_R.SetActive(!Turnsignal_R.activeSelf);
    }

    void BlinkLeftSignal()
    {
        Turnsignal_L.SetActive(!Turnsignal_L.activeSelf);
    }
}
