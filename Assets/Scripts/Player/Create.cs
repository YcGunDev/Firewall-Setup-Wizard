using UnityEngine;
using UnityEngine.InputSystem;

public class Create : MonoBehaviour
{
    public InputAction mouseDown;
    public InputAction mousePos;
    [Space]
    public bool isPressed = false;
    public Vector2 currentMousePos;

    public GameObject spawnerPrefab;
    public GameObject currentSpawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        mouseDown.Enable();
        mousePos.Enable();
    }

    private void OnDisable()
    {
        mouseDown.Disable();
        mousePos.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        mouseDown.performed += MouseDown;
        mousePos.performed += MousePos;

        if (isPressed && currentSpawner != null)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            currentSpawner.transform.position = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
        }
    }

    private void MouseDown(InputAction.CallbackContext context)
    {
        isPressed = !isPressed;

        if (isPressed && currentSpawner == null)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 pos = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
            currentSpawner = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        }

        if (!isPressed && currentSpawner != null)
        {
            Destroy(currentSpawner.gameObject);
        }

    }

    private void MousePos(InputAction.CallbackContext context)
    {
        currentMousePos = context.ReadValue<Vector2>();
    }
}
