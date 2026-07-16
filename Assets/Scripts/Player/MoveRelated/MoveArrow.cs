using UnityEngine;

public class MoveArrow : MonoBehaviour
{
    public LineRenderer line;
    public GameObject arrowEnd;
    public Material arrowMat;
    public GameObject arrowHead;

    private void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowEnd.transform.position);

        arrowHead.transform.LookAt(arrowEnd.transform);
    }
}
