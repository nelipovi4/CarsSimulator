using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _car;

    [Header("Camera Offsets")]
    [SerializeField] private Vector3 behindOffset = new Vector3(0f, 2f, -5f); // позади машины
    [SerializeField] private Vector3 frontOffset = new Vector3(0f, 2f, 5f);   // перед капотом

    [SerializeField] private float _followSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 5f;

    private Camera _camera;
    private bool isBehindView = true;
    private Vector3 currentOffset;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        if (_camera != null)
        {
            _camera.enabled = true;
            AudioListener listener = _camera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }

        currentOffset = behindOffset;
    }

    private void Update()
    {
        HandleViewSwitch();
    }

    private void FixedUpdate()
    {
        if (_car == null) return;

        Vector3 targetPosition = _car.position + _car.rotation * currentOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(_car.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void HandleViewSwitch()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isBehindView = !isBehindView;
            currentOffset = isBehindView ? behindOffset : frontOffset;
        }
    }

    public void SetTarget(Transform car)
    {
        _car = car;
    }
}
