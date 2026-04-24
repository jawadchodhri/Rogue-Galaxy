using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float[] parallaxSpeeds;  // Speed for each background layer (larger values = closer layers)
    private Transform[] layers;  // Store the background layers (child objects)

    public float backgroundSpeed = 1f;  // Speed at which the background moves down
    public float resetThreshold = -20f;  // Y position threshold to reset layer (adjust as needed)

    private void Start()
    {
        // Initialize the layers array to hold all child objects (layers) of the background
        layers = new Transform[transform.childCount];

        // Store references to all child layers in the array
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        // Move the background downward by a certain speed
        float moveDistance = backgroundSpeed * Time.deltaTime;

        // Apply the parallax effect to each layer
        for (int i = 0; i < layers.Length; i++)
        {
            // Calculate the amount to move the layer based on its speed and the background speed
            float layerMoveDistance = moveDistance * parallaxSpeeds[i];
            layers[i].position += new Vector3(0, -layerMoveDistance, 0);  // Move downward on the Y axis

            // Check if the layer has moved off-screen, then reset its position
            if (layers[i].position.y < resetThreshold)
            {
                // Reset the layer's Y position back to the start position
                Vector3 newPosition = layers[i].position;
                newPosition.y = Mathf.Abs(layers[i].position.y);  // Reset to starting position above the screen
                layers[i].position = newPosition;
            }
        }
    }
}
