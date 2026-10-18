using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class TablewareItem : MonoBehaviour
{
    [SerializeField] protected string itemName;
    protected PlacementSlot targetSlot;

    private bool placedCorrectly;
    private bool isHeld;
    private bool moved;
    private Vector3 grabOffset;
    private Vector3 pickupPosition;
    private Vector3 trayPosition;
    private Quaternion trayRotation;
    private const float LiftHeight = 0.03f;
    private const float MoveThreshold = 0.02f;

    public static TablewareItem Selected { get; private set; }
    public string ItemName => itemName;
    public bool IsLocked => placedCorrectly;

    public abstract bool IsOrientationValid();
    public abstract string GetHint();

    // Called by the factory when the item is created or reused.
    public void Setup(PlacementSlot slot, Vector3 trayPos, Quaternion trayRot)
    {
        targetSlot = slot;
        trayPosition = trayPos;
        trayRotation = trayRot;
        placedCorrectly = false;
        isHeld = false;
        transform.SetPositionAndRotation(trayPos, trayRot);
    }

    private void Update()
    {
        if (placedCorrectly || targetSlot == null) return;
        Touchscreen touch = Touchscreen.current;
        if (touch == null) return;

        var primary = touch.primaryTouch;
        Vector2 pos = primary.position.ReadValue();

        if (!isHeld)
        {
            if (primary.press.wasPressedThisFrame && HitsThisItem(pos, primary.touchId.ReadValue()))
            {
                grabOffset = Vector3.zero;
                if (TryGetTablePoint(pos, out Vector3 p))
                {
                    grabOffset = transform.position - p;
                    grabOffset.y = 0f;
                }
                PickUp();
            }
        }
        else if (primary.press.isPressed)
        {
            DragTo(pos);
        }
        else
        {
            Drop();
        }
    }

    public void PickUp()
    {
        isHeld = true;
        moved = false;
        pickupPosition = transform.position;
        Selected = this;
        GameEvents.RaiseItemPickedUp(this);
    }

    public void Drop()
    {
        isHeld = false;
        Vector3 p = transform.position;
        p.y = targetSlot.transform.position.y;
        transform.position = p;

        // A simple tap only selects the item, so it is not checked.
        if (moved) GameEvents.RaiseItemDropped(this);
    }

    public void Rotate()
    {
        transform.Rotate(0f, 90f, 0f, Space.World);
    }

    public bool IsPlacedCorrectly()
    {
        return targetSlot != null
            && targetSlot.IsWithinTolerance(transform.position)
            && IsOrientationValid();
    }

    public void LockInSlot()
    {
        transform.position = targetSlot.transform.position;
        placedCorrectly = true;
        if (Selected == this) Selected = null;
    }

    public void ReturnToTray()
    {
        transform.SetPositionAndRotation(trayPosition, trayRotation);
    }

    private void DragTo(Vector2 screenPos)
    {
        if (!TryGetTablePoint(screenPos, out Vector3 p)) return;

        Vector3 target = p + grabOffset;
        target.y = targetSlot.transform.position.y + LiftHeight;
        transform.position = target;
        Vector3 a = target;
        Vector3 b = pickupPosition;
        a.y = 0f;
        b.y = 0f;
        if (Vector3.Distance(a, b) > MoveThreshold) moved = true;
    }

    private bool TryGetTablePoint(Vector2 screenPos, out Vector3 point)
    {
        Plane tablePlane = new Plane(Vector3.up, targetSlot.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (tablePlane.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            return true;
        }
        point = default;
        return false;
    }

    private bool HitsThisItem(Vector2 screenPos, int touchId)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
            return false;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out RaycastHit hit, 10f)
            && hit.collider.transform.IsChildOf(transform);
    }
}