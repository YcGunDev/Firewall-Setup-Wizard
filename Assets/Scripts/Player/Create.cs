using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Create : MonoBehaviour
{
    public InputAction mouseDown;
    [Space]
    public bool isPressed = false;

    public GameObject spawnerPrefab;
    public GameObject currentSpawner;
    private NetworkObject networkSpawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        mouseDown.Enable();
    }

    private void OnDisable()
    {
        mouseDown.Disable();

        isPressed = false;
        if (!isPressed && currentSpawner != null)
        {
            Destroy(currentSpawner.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mouseDown.performed += MouseDown;

        if (isPressed && currentSpawner != null)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(MouseTracker.instance.currentMousePos);
            currentSpawner.transform.position = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
            //RequestMoveSpawnerServerRpc();
        }
    }

    private void MouseDown(InputAction.CallbackContext context)
    {
        isPressed = !isPressed;

        if (isPressed && currentSpawner == null)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(MouseTracker.instance.currentMousePos);
            Vector3 pos = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);

            currentSpawner = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        }

        if (!isPressed && currentSpawner != null)
        {
            Destroy(currentSpawner.gameObject);
        }

    }
}
