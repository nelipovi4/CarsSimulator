using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _car;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -4f);
    [SerializeField] private float _followSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 5f;

    private Camera _camera;
    private PhotonView _view;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        if (_car != null)
            _view = _car.GetComponent<PhotonView>();

        // Отключаем камеру и аудиослушатель у чужих игроков
        if (_view == null || !_view.IsMine)
        {
            if (_camera != null)
            {
                _camera.enabled = false;
                AudioListener listener = _camera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = false;
            }

            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (_car == null) return;

        // Позиция камеры относительно машины в мировых координатах
        Vector3 targetPosition = _car.position + _car.rotation * _offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

        // Плавный поворот камеры к машине
        Quaternion targetRotation = Quaternion.LookRotation(_car.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    // Позволяет задать машину программно
    public void SetTarget(Transform car)
    {
        _car = car;
    }
}
