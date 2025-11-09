using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class CarController : MonoBehaviourPun
{
    [Header("Wheel Transforms")]
    [SerializeField] private Transform _transformFL;
    [SerializeField] private Transform _transformFR;
    [SerializeField] private Transform _transformBL;
    [SerializeField] private Transform _transformBR;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;

    [Header("Car Settings")]
    [SerializeField] private float _force = 1500f;
    [SerializeField] private float _maxAngle = 30f;
    [SerializeField] private float _brakeForce = 3000f;

    private void FixedUpdate()
    {
        // Только владелец управляет машиной
        if (!photonView.IsMine) return;

        float vertical = 0f;
        if (Keyboard.current.wKey.isPressed) vertical += 1f;
        if (Keyboard.current.sKey.isPressed) vertical -= 1f;

        float horizontal = 0f;
        if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
        if (Keyboard.current.dKey.isPressed) horizontal += 1f;

        _colliderFL.motorTorque = vertical * _force;
        _colliderFR.motorTorque = vertical * _force;

        bool isHandbrake = Keyboard.current.spaceKey.isPressed;
        float brake = isHandbrake ? _brakeForce : 0f;

        _colliderFL.brakeTorque = brake;
        _colliderFR.brakeTorque = brake;
        _colliderBL.brakeTorque = brake;
        _colliderBR.brakeTorque = brake;

        if (isHandbrake)
        {
            ApplyDrift(_colliderBL);
            ApplyDrift(_colliderBR);
        }
        else
        {
            ResetFriction(_colliderBL);
            ResetFriction(_colliderBR);
        }

        _colliderFL.steerAngle = _maxAngle * horizontal;
        _colliderFR.steerAngle = _maxAngle * horizontal;

        RotateWheel(_colliderFL, _transformFL);
        RotateWheel(_colliderFR, _transformFR, true);
        RotateWheel(_colliderBL, _transformBL);
        RotateWheel(_colliderBR, _transformBR, true);
    }

    private void ApplyDrift(WheelCollider wheel)
    {
        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.stiffness = 0.5f;
        wheel.sidewaysFriction = sideways;
    }

    private void ResetFriction(WheelCollider wheel)
    {
        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.stiffness = 1f;
        wheel.sidewaysFriction = sideways;
    }

    private void RotateWheel(WheelCollider collider, Transform transform, bool invertRotation = false)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        if (invertRotation)
        {
            rotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        transform.position = position;
        transform.rotation = rotation;
    }
}
