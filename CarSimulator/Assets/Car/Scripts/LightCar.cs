using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class LightCar : MonoBehaviourPun, IPunObservable
{
    public GameObject Forward;
    public GameObject Back;
    public GameObject Turnsignal_R;
    public GameObject Turnsignal_L;

    private Keyboard kb;
    private bool isRightSignalOn = false;
    private bool isLeftSignalOn = false;

    // Сетевые состояния
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

        if (photonView.IsMine)
            kb = Keyboard.current;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

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

    // Синхронизация состояний
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(forwardOn);
            stream.SendNext(backOn);
            stream.SendNext(Turnsignal_R.activeSelf);
            stream.SendNext(Turnsignal_L.activeSelf);
        }
        else
        {
            forwardOn = (bool)stream.ReceiveNext();
            backOn = (bool)stream.ReceiveNext();
            Turnsignal_R.SetActive((bool)stream.ReceiveNext());
            Turnsignal_L.SetActive((bool)stream.ReceiveNext());
        }
    }
}
