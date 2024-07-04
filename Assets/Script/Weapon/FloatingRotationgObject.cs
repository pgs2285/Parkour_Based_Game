using UnityEngine;

public class FloatingRotationgObject : MonoBehaviour
{
    // 공중에 떠있는 정도
    public float floatAmplitude = 0.5f;
    // 떠다니는 속도
    public float floatSpeed = 1.0f;
    // 회전 속도
    public Vector3 rotationSpeed = new Vector3(0, 30, 0);

    private Vector3 initialPosition;

    void Start()
    {
        // 초기 위치 저장
        initialPosition = transform.position;
    }

    void Update()
    {
        // 떠다니는 계산
        float newY = initialPosition.y + floatAmplitude * Mathf.Sin(floatSpeed * Time.time);
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

        // 회전 계산
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}