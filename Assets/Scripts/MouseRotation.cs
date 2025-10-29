using UnityEngine;

public class MouseRotation : MonoBehaviour
{
    [Header("Ajustes")]
    [Tooltip("Velocidad de rotación (prueba 50 - 200)")]
    public float rotationSpeed = 100f; // Esto debería aparecer en el Inspector

    private float rotationX;
    private float rotationY;
    private bool rotationAllowed = false;

    void Update()
    {
        // Si quieres que rote solo con click, descomenta:
        if (!(Input.GetMouseButton(0) && rotationAllowed)) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX -= mouseY * rotationSpeed * Time.deltaTime;
        rotationY += mouseX * rotationSpeed * Time.deltaTime;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    public void canRotate()
    {
        if (rotationAllowed)
        {
            rotationAllowed = false;
        }
        else
        {
            rotationAllowed = true;
        }
    }
}