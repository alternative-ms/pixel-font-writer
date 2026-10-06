using UnityEngine;

public class PosFixer : MonoBehaviour
{
    [SerializeField]
    private Vector3 _defaultPos;

    public void MoveLeftRight(float offsetLeftRight)
    {
        transform.localPosition = new Vector3(transform.localPosition.x + offsetLeftRight, transform.localPosition.y, transform.localPosition.z);
    }

    public void MoveUpDown(float offsetUpDown)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + offsetUpDown, transform.localPosition.z);
    }

    public void ResetPos()
    {
        transform.localPosition = _defaultPos;
    }
}
