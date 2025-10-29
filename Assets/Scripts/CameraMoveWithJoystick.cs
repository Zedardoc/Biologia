using UnityEngine;

public class CameraMoveWithJoystick : MonoBehaviour
{
    public Joystick joystick; // Asigna tu joystick desde el inspector
    public float speed = 5f;

    void Update()
    {
        // Obtiene dirección desde el joystick
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Movimiento relativo a la orientación actual de la cámara
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;

        // Mueve la cámara en el mundo
        transform.position += direction * speed * Time.deltaTime;
    }
}
