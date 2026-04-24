using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControl controls;
    private Vector2 touchPosition;

    public float smoothSpeed = 15f;
    public Camera mainCamera;

    private void Awake()
    {
        controls = new PlayerControl();

        controls.Player.Action.performed += ctx => touchPosition = ctx.ReadValue<Vector2>();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        // Check if finger is touching
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        // Convert screen position ? world position
        Vector3 screenPos = new Vector3(touchPosition.x, touchPosition.y, 10f);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, worldPos, smoothSpeed * Time.deltaTime);
    }
}
