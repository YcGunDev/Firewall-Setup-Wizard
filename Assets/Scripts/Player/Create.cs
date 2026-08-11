using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Create : MonoBehaviour
{
    public InputAction mouseDown;
    public InputAction mousePos;
    [Space]
    public bool isPressed = false;
    public Vector2 currentMousePos;

    public GameObject spawnerPrefab;
    public GameObject currentSpawner;
    private NetworkObject networkSpawner;
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
            //RequestMoveSpawnerServerRpc();
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
            //RequestSpawnSpawnerServerRpc(pos);
        }

        if (!isPressed && currentSpawner != null)
        {
            Destroy(currentSpawner.gameObject);
            //RequestDespawnSpawnerServerRpc();
        }

    }

    private void MousePos(InputAction.CallbackContext context)
    {
        currentMousePos = context.ReadValue<Vector2>();
    }

    //this is pretty cool, but i think i need to change up how the system works, it should only replicate the exact placement of the block, not the 
    //movement and function of the spawner, so this functionality should be chucked into the block spawner
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSpawnSpawnerServerRpc(Vector3 spawnPosition)
    {
        // This code executes strictly on the Server
        currentSpawner = Instantiate(spawnerPrefab, spawnPosition, Quaternion.identity);

        // 2. Get the NetworkObject component
        networkSpawner = currentSpawner.GetComponent<NetworkObject>();

        // 3. Spawn across the network to all clients
        networkSpawner.Spawn();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestDespawnSpawnerServerRpc()
    {
        networkSpawner.Despawn();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestMoveSpawnerServerRpc()
    {
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
        currentSpawner.transform.position = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
    }
}
