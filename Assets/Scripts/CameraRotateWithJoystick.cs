using UnityEngine;

public class CameraRotateWithJoystick : MonoBehaviour
{
    public Joystick joystick;
    public float sensitivity = 100f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        // Lectura del joystick
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Ajusta las rotaciones
        rotationX += horizontal * sensitivity * Time.deltaTime;
        rotationY -= vertical * sensitivity * Time.deltaTime;

        // Limita el ángulo vertical
        rotationY = Mathf.Clamp(rotationY, -80f, 80f);

        // Aplica la rotación
        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
