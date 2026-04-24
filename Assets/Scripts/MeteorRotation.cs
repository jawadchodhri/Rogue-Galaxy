using UnityEngine;

public class MeteorRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;  // Rotation speed

    private void Update()
    {
        // Rotate the parent object (which should be the "background") and all its child meteors
        foreach (Transform child in transform)
        {
            child.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);  // Rotate around its own axis (Z-axis)
        }
    }
}
