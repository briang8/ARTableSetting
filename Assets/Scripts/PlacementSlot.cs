using UnityEngine;

public class PlacementSlot : MonoBehaviour
{
    [SerializeField] private string slotId;
    [SerializeField] private float tolerance = 0.04f;
    [SerializeField] private float expectedRotation;

    public string SlotId => slotId;
    public float ExpectedYaw => transform.eulerAngles.y + expectedRotation;

    // True when the position is within the tolerance (4 cm) of this slot, ignoring height.
    public bool IsWithinTolerance(Vector3 pos)
    {
        Vector3 a = transform.position;
        Vector3 b = pos;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b) <= tolerance;
    }
}