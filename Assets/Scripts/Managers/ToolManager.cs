using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolManager : MonoBehaviour
{
    public static ToolManager instance;
    public enum Tools { Create, Select, Move };
    public Tools equippedTool = Tools.Create;

    public GameObject player;//probably move this later

    public GameObject createComponent;
    public GameObject selectComponent;
    public GameObject moveComponent;

    public InputAction createTools;
    public InputAction selectTools;
    public InputAction moveTools;

    private void OnEnable()
    {
        createTools.Enable();
        selectTools.Enable();
        moveTools.Enable();
    }

    private void OnDisable()
    {
        createTools.Disable();
        selectTools.Disable();
        moveTools.Disable();
    }

    private void Awake()
    {
        instance = this;
        player = FindFirstObjectByType<IAmPlayer>(FindObjectsInactive.Exclude).gameObject;

        createComponent = FindFirstObjectByType<Create>(FindObjectsInactive.Include).gameObject;
        selectComponent = FindFirstObjectByType<Select>(FindObjectsInactive.Include).gameObject;
        moveComponent = FindFirstObjectByType<Move>(FindObjectsInactive.Include).gameObject;
        switch (equippedTool)
        {
            case Tools.Create:
                createComponent.SetActive(true);
                break;
            case Tools.Select:
                selectComponent.SetActive(true);
                break;
            case Tools.Move:
                moveComponent.SetActive(true);
                break;
        }
    }

    void Update()
    {
        createTools.performed += CreateHotKey;
        selectTools.performed += SelectHotKey;
        moveTools.performed += MoveHotKey;
    }

    private void CreateHotKey(InputAction.CallbackContext context)
    {
        Debug.Log("Create");
        equippedTool = Tools.Create;
        ProcessCurrentTool();
    }

    private void SelectHotKey(InputAction.CallbackContext context)
    {
        Debug.Log("Select");
        equippedTool = Tools.Select;
        ProcessCurrentTool();
    }

    private void MoveHotKey(InputAction.CallbackContext context)
    {
        Debug.Log("Move");
        equippedTool = Tools.Move;
        ProcessCurrentTool();
    }

    private void ProcessCurrentTool()
    {
        switch (equippedTool)
        {
            case Tools.Create:
                createComponent.SetActive(true);
                selectComponent.SetActive(false);
                moveComponent.SetActive(false);
                break;
            case Tools.Select:
                createComponent.SetActive(false);
                selectComponent.SetActive(true);
                moveComponent.SetActive(false);
                break;
            case Tools.Move:
                createComponent.SetActive(false);
                selectComponent.SetActive(false);
                moveComponent.SetActive(true);
                break;
        }
    }

}
