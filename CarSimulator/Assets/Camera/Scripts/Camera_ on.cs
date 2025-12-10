using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class FollowCamera_on : MonoBehaviour
{
    [SerializeField] private Transform _car;

    [Header("Camera Offsets")]
    [SerializeField] private Vector3 behindOffset = new Vector3(0f, 2f, -5f);
    [SerializeField] private Vector3 frontOffset = new Vector3(0f, 2f, 5f);

    [SerializeField] private float _followSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 5f;

    private Camera _camera;
    private PhotonView _view;
    private bool isBehindView = true;
    private Vector3 currentOffset;

    private void Awake()
    {
        _view = GetComponentInParent<PhotonView>(); // Ищем PhotonView на игроке

        _camera = GetComponent<Camera>();

        // Камера включается только если объект — локальный игрок
        if (_view != null && !_view.IsMine)
        {
            if (_camera != null) _camera.enabled = false;

            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;

            enabled = false; // Полностью отключаем FollowCamera на чужих игроках
            return;
        }
    }

    private void Start()
    {
        currentOffset = behindOffset;
    }

    private void Update()
    {
        HandleViewSwitch();
    }

    private void FixedUpdate()
    {
        if (_car == null) return;

        Vector3 targetPos = _car.position + _car.rotation * currentOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, _followSpeed * Time.deltaTime);

        Quaternion targetRot = Quaternion.LookRotation(_car.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
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
