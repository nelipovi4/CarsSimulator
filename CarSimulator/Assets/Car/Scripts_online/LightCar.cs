using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class LightCar_on : MonoBehaviourPun
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
        if (!photonView.IsMine) return; // Только локальный игрок управляет вводом

        // Передний свет
        if (kb.hKey.wasPressedThisFrame)
        {
            forwardOn = true;
            photonView.RPC("RPC_SetForward", RpcTarget.All, forwardOn);
        }

        if (kb.gKey.wasPressedThisFrame)
        {
            forwardOn = false;
            photonView.RPC("RPC_SetForward", RpcTarget.All, forwardOn);
        }

        // Задний свет
        if (kb.sKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
        {
            backOn = true;
            photonView.RPC("RPC_SetBack", RpcTarget.All, backOn);
        }

        if (kb.sKey.wasReleasedThisFrame || kb.spaceKey.wasReleasedThisFrame)
        {
            backOn = false;
            photonView.RPC("RPC_SetBack", RpcTarget.All, backOn);
        }

        // Правый поворотник
        if (kb.eKey.wasPressedThisFrame)
        {
            if (!isRightSignalOn)
            {
                if (isLeftSignalOn) TurnOffLeftSignal();

                isRightSignalOn = true;
                photonView.RPC("RPC_StartRightSignal", RpcTarget.All);
            }
            else
            {
                TurnOffRightSignal();
            }
        }

        // Левый поворотник
        if (kb.qKey.wasPressedThisFrame)
        {
            if (!isLeftSignalOn)
            {
                if (isRightSignalOn) TurnOffRightSignal();

                isLeftSignalOn = true;
                photonView.RPC("RPC_StartLeftSignal", RpcTarget.All);
            }
            else
            {
                TurnOffLeftSignal();
            }
        }
    }

    // Локальное выключение правого сигнала
    void TurnOffRightSignal()
    {
        isRightSignalOn = false;
        photonView.RPC("RPC_StopRightSignal", RpcTarget.All);
    }

    // Локальное выключение левого сигнала
    void TurnOffLeftSignal()
    {
        isLeftSignalOn = false;
        photonView.RPC("RPC_StopLeftSignal", RpcTarget.All);
    }

    #region RPCs

    [PunRPC]
    void RPC_SetForward(bool state)
    {
        Forward.SetActive(state);
    }

    [PunRPC]
    void RPC_SetBack(bool state)
    {
        Back.SetActive(state);
    }

    [PunRPC]
    void RPC_StartRightSignal()
    {
        CancelInvoke(nameof(BlinkRightSignal));
        InvokeRepeating(nameof(BlinkRightSignal), 0f, 0.5f);
    }

    [PunRPC]
    void RPC_StopRightSignal()
    {
        CancelInvoke(nameof(BlinkRightSignal));
        Turnsignal_R.SetActive(false);
    }

    [PunRPC]
    void RPC_StartLeftSignal()
    {
        CancelInvoke(nameof(BlinkLeftSignal));
        InvokeRepeating(nameof(BlinkLeftSignal), 0f, 0.5f);
    }

    [PunRPC]
    void RPC_StopLeftSignal()
    {
        CancelInvoke(nameof(BlinkLeftSignal));
        Turnsignal_L.SetActive(false);
    }

    #endregion

    void BlinkRightSignal()
    {
        Turnsignal_R.SetActive(!Turnsignal_R.activeSelf);
    }

    void BlinkLeftSignal()
    {
        Turnsignal_L.SetActive(!Turnsignal_L.activeSelf);
    }
}
