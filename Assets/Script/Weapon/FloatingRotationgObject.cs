using UnityEngine;

public class FloatingRotationgObject : MonoBehaviour
{
    // ���߿� ���ִ� ����
    public float floatAmplitude = 0.5f;
    // ���ٴϴ� �ӵ�
    public float floatSpeed = 1.0f;
    // ȸ�� �ӵ�
    public Vector3 rotationSpeed = new Vector3(0, 30, 0);

    private Vector3 initialPosition;

    void Start()
    {
        // �ʱ� ��ġ ����
        initialPosition = transform.position;
    }

    void Update()
    {
        // ���ٴϴ� ���
        float newY = initialPosition.y + floatAmplitude * Mathf.Sin(floatSpeed * Time.time);
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

        // ȸ�� ���
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}