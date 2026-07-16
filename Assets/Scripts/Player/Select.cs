using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Select : MonoBehaviour
{
    public InputAction mouseDown;
    public InputAction mousePos;
    [Space]
    public bool isPressed = false;
    public Vector2 currentMousePos;
    public Vector2 selectStartPos;

    [SerializeField] GameObject selectBoxPrefab;
    [SerializeField] GameObject currentSelectBox;
    SelectBox sb;

    public List<Block> selectedBlocks;

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

        if(currentSelectBox != null)
        {
            //get the middle point between the startPos and the current mouse pos
            Vector3 startToWorld = Camera.main.ScreenToWorldPoint(selectStartPos);
            Vector3 startPos = new Vector3(startToWorld.x, startToWorld.y, 0.0f);

            Vector3 currentToWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 currentPos = new Vector3(currentToWorld.x, currentToWorld.y, 0.0f);

            Vector3 midpoint = Vector3.Lerp(startPos, currentPos, 0.5f);
            currentSelectBox.transform.position = midpoint;

            float xDist = Mathf.Abs(startPos.x - currentPos.x);
            float yDist = Mathf.Abs(startPos.y - currentPos.y);
            currentSelectBox.transform.localScale = new Vector3(xDist, yDist, 1.0f);
        }
    }

    private void MouseDown(InputAction.CallbackContext context)
    {
        isPressed = !isPressed;

        if (isPressed && currentSelectBox == null)
        {
            selectStartPos = currentMousePos;
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 pos = new Vector3(screenToWorld.x, screenToWorld.y, 0.0f);
            currentSelectBox = Instantiate(selectBoxPrefab, pos, Quaternion.identity);
            currentSelectBox.transform.localScale = Vector3.zero;
            sb = currentSelectBox.GetComponent<SelectBox>();
            sb.selectParent = this;
        }

        if (!isPressed && currentSelectBox != null)
        {
            sb.SendBlockList();
            Destroy(currentSelectBox.gameObject);
        }


        //save the location of where u moused downed
    }

    private void MousePos(InputAction.CallbackContext context)
    {
        currentMousePos = context.ReadValue<Vector2>();
    }
}
