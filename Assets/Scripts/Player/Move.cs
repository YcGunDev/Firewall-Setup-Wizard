using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] Select selectTool;

    public InputAction mouseDown;
    public InputAction mousePos;
    [Space]
    public bool isPressed = false;
    public Vector2 currentMousePos;

    [SerializeField] GameObject arrowStartPrefab;
    [SerializeField] GameObject arrowEndPrefab;

    [SerializeField] GameObject currentArrowStart;
    [SerializeField] GameObject currentArrowEnd;

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

        if (selectTool.selectedBlocks.Count > 0 && isPressed)
        {
            //get the middle point between the startPos and the current mouse pos
            Vector3 toWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 endPos = new Vector3(toWorld.x, toWorld.y, 0.0f);
            currentArrowEnd.transform.position = endPos;
        }
    }

    private void MouseDown(InputAction.CallbackContext context)
    {
        isPressed = !isPressed;

        if (isPressed && currentArrowStart == null && currentArrowEnd == null && selectTool.selectedBlocks.Count > 0)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 pos = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
            currentArrowStart = Instantiate(arrowStartPrefab, pos, Quaternion.identity);
            currentArrowEnd = Instantiate(arrowEndPrefab, pos, Quaternion.identity);
            currentArrowEnd.GetComponent<MoveArrow>().arrowEnd = currentArrowStart;
        }

        if (!isPressed && currentArrowStart != null && currentArrowEnd != null)
        {
            //magnitude and direction
            //oh yeah!
            float magnitude = Vector3.Distance(currentArrowEnd.transform.position, currentArrowStart.transform.position);
            Vector2 direction = Vector3.Normalize(currentArrowEnd.transform.position - currentArrowStart.transform.position);

            for(int i = 0; i < selectTool.selectedBlocks.Count; i++)
            {
                selectTool.selectedBlocks[i].speed = Mathf.Clamp(magnitude, 0.0f, 25.0f);
                selectTool.selectedBlocks[i].direction = direction;

                selectTool.selectedBlocks[i].health = (int)Mathf.Lerp((float)selectTool.selectedBlocks[i].health, 1.0f, selectTool.selectedBlocks[i].speed / 25.0f);
                selectTool.selectedBlocks[i].UpdateHPUI();
            }

            Destroy(currentArrowStart.gameObject);
            Destroy(currentArrowEnd.gameObject);
        }
    }

    private void MousePos(InputAction.CallbackContext context)
    {
        currentMousePos = context.ReadValue<Vector2>();
    }
}
