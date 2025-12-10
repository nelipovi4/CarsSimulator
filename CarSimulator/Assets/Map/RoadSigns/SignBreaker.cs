using UnityEngine;

public class SignBreaker : MonoBehaviour
{
    [Header("Ссылка на сам знак (не на основание!)")]
    public GameObject sign;

    [Header("Сила отбрасывания")]
    public float forceMultiplier = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Car")) return;

        if (collision.relativeVelocity.magnitude > 8f)
        {
            // Ломаем соединение
            var joint = sign.GetComponent<FixedJoint>();
            if (joint != null) Destroy(joint);

            // Включаем физику знака
            var rb = sign.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(collision.relativeVelocity * forceMultiplier, ForceMode.Impulse);
            }

            // Уничтожаем основание с задержкой, чтобы скрипт успел всё сделать
            Destroy(gameObject, 0.3f);
        }
    }
}