using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControl controls;
    private Vector2 touchPosition;

    public float smoothSpeed = 15f;  // Speed at which the player moves
    public Camera mainCamera;  // Camera to convert screen position to world space

    private void Awake()
    {
        controls = new PlayerControl();
        controls.Player.Touch.performed += ctx => touchPosition = ctx.ReadValue<Vector2>();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        // Check if the touch is being pressed
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        // Convert the touch position from screen space to world space
        Vector3 screenPos = new Vector3(touchPosition.x, touchPosition.y, 10f);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        // Smooth movement to the touch position
        transform.position = Vector3.Lerp(transform.position, worldPos, smoothSpeed * Time.deltaTime);
    }
}
