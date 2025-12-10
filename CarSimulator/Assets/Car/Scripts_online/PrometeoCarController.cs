using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PrometeoCarController_on : MonoBehaviourPun
{
    [Space(10)]
    [Range(20, 190)] public int maxSpeed = 90;
    [Range(10, 120)] public int maxReverseSpeed = 45;
    [Range(1, 10)] public int accelerationMultiplier = 2;

    [Space(10)]
    [Range(10, 45)] public int maxSteeringAngle = 27;
    [Range(0.1f, 1f)] public float steeringSpeed = 0.5f;

    [Space(10)]
    [Range(100, 600)] public int brakeForce = 350;
    [Range(1, 10)] public int decelerationMultiplier = 2;
    [Range(1, 10)] public int handbrakeDriftMultiplier = 5;

    [Space(10)] public Vector3 bodyMassCenter;

    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;

    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;

    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;

    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    [Space(10)] public bool useEffects = false;
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;
    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    [Space(10)] public bool useUI = false;
    public Text carSpeedText;

    [Space(10)] public bool useSounds = false;
    public AudioSource carEngineSound;
    public AudioSource tireScreechSound;
    float initialCarEngineSoundPitch;

    Rigidbody carRigidbody;
    float steeringAxis;
    float throttleAxis;
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;

    WheelFrictionCurve FLwheelFriction; float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction; float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction; float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction; float RRWextremumSlip;

    [HideInInspector] public float carSpeed;
    [HideInInspector] public bool isDrifting;
    [HideInInspector] public bool isTractionLocked;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        SetupWheelFriction();

        if (carEngineSound != null) initialCarEngineSoundPitch = carEngineSound.pitch;

        if (useUI) InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        if (useSounds) InvokeRepeating("UpdateCarSounds", 0f, 0.1f);
    }

    void SetupWheelFriction()
    {
        FLwheelFriction = frontLeftCollider.sidewaysFriction; FLWextremumSlip = FLwheelFriction.extremumSlip;
        FRwheelFriction = frontRightCollider.sidewaysFriction; FRWextremumSlip = FRwheelFriction.extremumSlip;
        RLwheelFriction = rearLeftCollider.sidewaysFriction; RLWextremumSlip = RLwheelFriction.extremumSlip;
        RRwheelFriction = rearRightCollider.sidewaysFriction; RRWextremumSlip = RRwheelFriction.extremumSlip;
    }

    void Update()
    {
        if (!photonView.IsMine) return; // ������ ��������� ����� ��������� �������

        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        HandleKeyboardInput();
        AnimateWheelMeshes();
        if (useEffects) UpdateDriftEffects();
    }

    void HandleKeyboardInput()
    {
        if (Keyboard.current.wKey.isPressed) GoForward();
        if (Keyboard.current.sKey.isPressed) GoReverse();

        if (Keyboard.current.aKey.isPressed) TurnLeft();
        if (Keyboard.current.dKey.isPressed) TurnRight();

        if (Keyboard.current.spaceKey.isPressed) Handbrake();
        if (Keyboard.current.spaceKey.wasReleasedThisFrame) RecoverTraction();

        if (!Keyboard.current.wKey.isPressed && !Keyboard.current.sKey.isPressed) ThrottleOff();

        if ((!Keyboard.current.wKey.isPressed && !Keyboard.current.sKey.isPressed && !Keyboard.current.spaceKey.isPressed) && !deceleratingCar)
        {
            InvokeRepeating("DecelerateCar", 0f, 0.1f);
            deceleratingCar = true;
        }

        if (!Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed && steeringAxis != 0f)
            ResetSteeringAngle();
    }

    void AnimateWheelMeshes()
    {
        Quaternion FLWRot; Vector3 FLWPos; frontLeftCollider.GetWorldPose(out FLWPos, out FLWRot);
        frontLeftMesh.transform.position = FLWPos; frontLeftMesh.transform.rotation = FLWRot;

        Quaternion FRWRot; Vector3 FRWPos; frontRightCollider.GetWorldPose(out FRWPos, out FRWRot);
        frontRightMesh.transform.position = FRWPos; frontRightMesh.transform.rotation = FRWRot * Quaternion.Euler(0f, 180f, 0f);

        Quaternion RLWRot; Vector3 RLWPos; rearLeftCollider.GetWorldPose(out RLWPos, out RLWRot);
        rearLeftMesh.transform.position = RLWPos; rearLeftMesh.transform.rotation = RLWRot;

        Quaternion RRWRot; Vector3 RRWPos; rearRightCollider.GetWorldPose(out RRWPos, out RRWRot);
        rearRightMesh.transform.position = RRWPos; rearRightMesh.transform.rotation = RRWRot * Quaternion.Euler(0f, 180f, 0f);
    }

    public void CarSpeedUI()
    {
        if (!useUI) return;
        carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(carSpeed)).ToString();
    }

    void UpdateCarSounds()
    {
        if (!useSounds) return;

        float enginePitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
        photonView.RPC("RPC_SetEngineSoundPitch", RpcTarget.All, enginePitch);

        bool shouldPlayScreech = (isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f));
        photonView.RPC("RPC_SetTireScreech", RpcTarget.All, shouldPlayScreech);
    }

    #region Movement

    public void GoForward()
    {
        throttleAxis += Time.deltaTime * 3f; if (throttleAxis > 1f) throttleAxis = 1f;
        if (localVelocityZ < -1f) Brakes();
        else ApplyMotorTorque(throttleAxis);
    }

    public void GoReverse()
    {
        throttleAxis -= Time.deltaTime * 3f; if (throttleAxis < -1f) throttleAxis = -1f;
        if (localVelocityZ > 1f) Brakes();
        else ApplyMotorTorque(throttleAxis);
    }

    void ApplyMotorTorque(float axis)
    {
        if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxSpeed)
        {
            frontLeftCollider.motorTorque = axis * accelerationMultiplier * 50f;
            frontRightCollider.motorTorque = axis * accelerationMultiplier * 50f;
            rearLeftCollider.motorTorque = axis * accelerationMultiplier * 50f;
            rearRightCollider.motorTorque = axis * accelerationMultiplier * 50f;
        }
        else
        {
            frontLeftCollider.motorTorque = 0; frontRightCollider.motorTorque = 0; rearLeftCollider.motorTorque = 0; rearRightCollider.motorTorque = 0;
        }
    }

    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    public void DecelerateCar()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f) isDrifting = true;
        else isDrifting = false;

        if (throttleAxis != 0f)
        {
            throttleAxis = Mathf.MoveTowards(throttleAxis, 0f, Time.deltaTime * 10f);
        }

        carRigidbody.linearVelocity *= (1f / (1f + (0.025f * decelerationMultiplier)));

        if (carRigidbody.linearVelocity.magnitude < 0.25f) { carRigidbody.linearVelocity = Vector3.zero; CancelInvoke("DecelerateCar"); }
    }

    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    public void TurnLeft()
    {
        steeringAxis = Mathf.Clamp(steeringAxis - Time.deltaTime * 10f * steeringSpeed, -1f, 1f);
        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, angle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, angle, steeringSpeed);
    }

    public void TurnRight()
    {
        steeringAxis = Mathf.Clamp(steeringAxis + Time.deltaTime * 10f * steeringSpeed, -1f, 1f);
        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, angle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, angle, steeringSpeed);
    }

    public void ResetSteeringAngle()
    {
        steeringAxis = Mathf.MoveTowards(steeringAxis, 0f, Time.deltaTime * 10f * steeringSpeed);
        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, angle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, angle, steeringSpeed);
    }

    public void Handbrake()
    {
        isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
        isTractionLocked = true;
        // ����� �������� ������ � ���������� WheelFriction ��� � ���������
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;
        isDrifting = false;
    }

    void UpdateDriftEffects()
    {
        if (!useEffects) return;
        if (isDrifting)
        {
            if (RLWParticleSystem != null) RLWParticleSystem.Play();
            if (RRWParticleSystem != null) RRWParticleSystem.Play();
        }
        else
        {
            if (RLWParticleSystem != null) RLWParticleSystem.Stop();
            if (RRWParticleSystem != null) RRWParticleSystem.Stop();
        }

        if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
        {
            if (RLWTireSkid != null) RLWTireSkid.emitting = true;
            if (RRWTireSkid != null) RRWTireSkid.emitting = true;
        }
        else
        {
            if (RLWTireSkid != null) RLWTireSkid.emitting = false;
            if (RRWTireSkid != null) RRWTireSkid.emitting = false;
        }
    }

    #endregion

    #region Photon RPCs

    [PunRPC]
    void RPC_SetEngineSoundPitch(float pitch)
    {
        if (carEngineSound != null) carEngineSound.pitch = pitch;
        if (!carEngineSound.isPlaying) carEngineSound.Play();
    }

    [PunRPC]
    void RPC_SetTireScreech(bool play)
    {
        if (tireScreechSound == null) return;
        if (play && !tireScreechSound.isPlaying) tireScreechSound.Play();
        if (!play && tireScreechSound.isPlaying) tireScreechSound.Stop();
    }

    #endregion
}
