using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSounds_on : MonoBehaviourPun
{
    [Header("Engine Settings")]
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody carRb;
    private AudioSource carAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;

    [Header("Extra Sounds")]
    public AudioClip brakeSound;
    public AudioClip leftIndicatorSound;
    public AudioClip rightIndicatorSound;

    private AudioSource brakeAudio;
    private AudioSource leftIndicatorAudio;
    private AudioSource rightIndicatorAudio;

    [Header("Lights")]
    public GameObject leftIndicatorLight;
    public GameObject rightIndicatorLight;
    public GameObject brakeLight;

    private bool leftIndicatorOn = false;
    private bool rightIndicatorOn = false;
    private bool brakeOn = false;

    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();

        // ��������� ��������� ��� ������������ ���������������
        brakeAudio = gameObject.AddComponent<AudioSource>();
        brakeAudio.loop = true;

        leftIndicatorAudio = gameObject.AddComponent<AudioSource>();
        leftIndicatorAudio.loop = true;

        rightIndicatorAudio = gameObject.AddComponent<AudioSource>();
        rightIndicatorAudio.loop = true;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            EngineSound();
            HandleBrake();
            HandleIndicators();
        }
    }

    void EngineSound()
    {
        currentSpeed = carRb.linearVelocity.magnitude;
        pitchFromCar = carRb.linearVelocity.magnitude / 60f;

        float pitch;
        if (currentSpeed < minSpeed)
            pitch = minPitch;
        else if (currentSpeed <= maxSpeed)
            pitch = minPitch + pitchFromCar;
        else
            pitch = maxPitch;

        carAudio.pitch = pitch;

        // �������������� ���� ��������� ��� ����
        photonView.RPC("RPC_SetEnginePitch", RpcTarget.Others, pitch);
    }

    void HandleBrake()
    {
        if (Keyboard.current.sKey.isPressed)
        {
            if (!brakeOn)
            {
                brakeOn = true;
                photonView.RPC("RPC_PlayBrakeSound", RpcTarget.All, true);
                photonView.RPC("RPC_SetBrakeLights", RpcTarget.All, true);
            }
        }
        else
        {
            if (brakeOn)
            {
                brakeOn = false;
                photonView.RPC("RPC_PlayBrakeSound", RpcTarget.All, false);
                photonView.RPC("RPC_SetBrakeLights", RpcTarget.All, false);
            }
        }
    }

    void HandleIndicators()
    {
        // ����� ���������� (Q)
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            leftIndicatorOn = !leftIndicatorOn;
            photonView.RPC("RPC_SetLeftIndicator", RpcTarget.All, leftIndicatorOn);
        }

        // ������ ���������� (E)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            rightIndicatorOn = !rightIndicatorOn;
            photonView.RPC("RPC_SetRightIndicator", RpcTarget.All, rightIndicatorOn);
        }
    }

    // RPC ��� ��������� �����
    [PunRPC]
    void RPC_SetBrakeLights(bool state)
    {
        if (brakeLight != null)
            brakeLight.SetActive(state);
    }

    // RPC ��� ��������������� ���������� �����
    [PunRPC]
    void RPC_PlayBrakeSound(bool play)
    {
        if (brakeSound == null) return;

        if (play)
        {
            brakeAudio.clip = brakeSound;
            brakeAudio.Play();
        }
        else
        {
            brakeAudio.Stop();
        }
    }

    // RPC ��� ������ ����������
    [PunRPC]
    void RPC_SetLeftIndicator(bool state)
    {
        if (leftIndicatorLight != null)
            leftIndicatorLight.SetActive(state);

        if (leftIndicatorSound != null)
        {
            if (state)
            {
                leftIndicatorAudio.clip = leftIndicatorSound;
                leftIndicatorAudio.Play();
            }
            else
            {
                leftIndicatorAudio.Stop();
            }
        }
    }

    // RPC ��� ������� ����������
    [PunRPC]
    void RPC_SetRightIndicator(bool state)
    {
        if (rightIndicatorLight != null)
            rightIndicatorLight.SetActive(state);

        if (rightIndicatorSound != null)
        {
            if (state)
            {
                rightIndicatorAudio.clip = rightIndicatorSound;
                rightIndicatorAudio.Play();
            }
            else
            {
                rightIndicatorAudio.Stop();
            }
        }
    }

    // RPC ��� ������������� ����� ���������
    [PunRPC]
    void RPC_SetEnginePitch(float pitch)
    {
        if (!photonView.IsMine)
            carAudio.pitch = pitch;
    }
}
