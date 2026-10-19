using UnityEngine;

public class PlacementSlot : MonoBehaviour
{
    [SerializeField] private string slotId;
    [SerializeField] private float tolerance = 0.04f;
    [SerializeField] private float expectedRotation;

    private Transform marker;

    public string SlotId => slotId;
    public float ExpectedYaw => transform.eulerAngles.y + expectedRotation;

    // Shows or hides the coloured guide on the table for this slot.
    public void SetMarkerVisible(bool visible)
    {
        if (marker == null) marker = transform.Find("Marker");
        if (marker != null) marker.gameObject.SetActive(visible);
    }

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