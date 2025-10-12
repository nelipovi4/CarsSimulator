using UnityEngine;

public class SignBreaker : MonoBehaviour
{
    public GameObject sign; // ������ �� ������ �����

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            float impact = collision.relativeVelocity.magnitude;

            if (impact > 10f) // ����� ���� �����
            {
                // ������� ����������
                FixedJoint joint = sign.GetComponent<FixedJoint>();
                if (joint != null) Destroy(joint);

                // �������� ������ � �����
                Rigidbody rb = sign.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(collision.relativeVelocity * 100f, ForceMode.Impulse);

                // ������� ���������
                Destroy(gameObject);
            }
        }
    }
}
