using UnityEngine;

public class SignBreaker : MonoBehaviour
{
    public GameObject sign; // ссылка на объект знака

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            float impact = collision.relativeVelocity.magnitude;

            if (impact > 10f) // порог силы удара
            {
                // Удаляем соединение
                FixedJoint joint = sign.GetComponent<FixedJoint>();
                if (joint != null) Destroy(joint);

                // Включаем физику у знака
                Rigidbody rb = sign.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(collision.relativeVelocity * 100f, ForceMode.Impulse);

                // Удаляем основание
                Destroy(gameObject);
            }
        }
    }
}
