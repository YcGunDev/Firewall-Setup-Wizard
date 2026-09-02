using UnityEngine;
using UnityEngine.InputSystem;

public class MouseTracker : MonoBehaviour
{
    public static MouseTracker instance;
    public Vector2 currentMousePos;
    public InputAction mousePos;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        mousePos.Enable();
    }

    private void OnDisable()
    {
        mousePos.Disable();
    }

    private void Update()
    {
        mousePos.performed += MousePos;
    }

    private void MousePos(InputAction.CallbackContext context)
    {
        currentMousePos = context.ReadValue<Vector2>();
    }
}
