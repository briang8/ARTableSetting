using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject tablePrefab;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button repositionButton;
    [SerializeField] private Button beginButton;

    private enum Step { Scanning, Previewing, Placed }

    private Step step = Step.Scanning;
    private GameObject table;
    private ARAnchor tableAnchor;
    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public event Action BeginPressed;
    public Transform TableTransform => table != null ? table.transform : null;

    private void Awake()
    {
        confirmButton.onClick.AddListener(PlaceTable);
        repositionButton.onClick.AddListener(RepositionTable);
        beginButton.onClick.AddListener(() => BeginPressed?.Invoke());
    }

    // Starts a fresh setup: no table, planes visible, scanning again.
    public void BeginSetup()
    {
        enabled = true;
        ClearTable();
        planeManager.enabled = true;
        planeManager.SetTrackablesActive(true);
        SetStep(Step.Scanning);
    }

    // Ends setup but keeps the table and its anchor in place.
    public void EndSetup()
    {
        planeManager.SetTrackablesActive(false);
        planeManager.enabled = false;
        enabled = false;
    }

    public void ClearTable()
    {
        if (tableAnchor != null)
        {
            Destroy(tableAnchor);
            tableAnchor = null;
        }
        if (table != null)
        {
            Destroy(table);
            table = null;
        }
    }

    private void Update()
    {
        if (step == Step.Scanning && planeManager.trackables.count > 0)
            statusText.text = "Surface found. Tap it to preview the table.";

        if (step == Step.Placed) return;

        if (TryGetPlacementPose(out Pose pose))
        {
            if (table == null)
            {
                float yaw = Camera.main.transform.eulerAngles.y;
                table = Instantiate(tablePrefab, pose.position, Quaternion.Euler(0f, yaw, 0f));
            }
            else
            {
                table.transform.position = pose.position;
            }
            SetStep(Step.Previewing);
        }
    }

    // Returns true when the user taps a detected plane (not a UI button).
    public bool TryGetPlacementPose(out Pose pose)
    {
        pose = default;
        Touchscreen touch = Touchscreen.current;
        if (touch == null || !touch.primaryTouch.press.wasPressedThisFrame) return false;

        int touchId = touch.primaryTouch.touchId.ReadValue();
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
            return false;

        Vector2 screenPos = touch.primaryTouch.position.ReadValue();
        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose;
            return true;
        }
        return false;
    }

    public void PlaceTable()
    {
        if (table == null) return;
        tableAnchor = table.AddComponent<ARAnchor>();
        SetStep(Step.Placed);
    }

    public void RepositionTable()
    {
        if (tableAnchor != null)
        {
            Destroy(tableAnchor);
            tableAnchor = null;
        }
        SetStep(Step.Previewing);
    }

    private void SetStep(Step newStep)
    {
        step = newStep;
        confirmButton.gameObject.SetActive(step == Step.Previewing);
        repositionButton.gameObject.SetActive(step == Step.Placed);
        beginButton.gameObject.SetActive(step == Step.Placed);

        if (step == Step.Scanning)
            statusText.text = "Move your phone slowly to find a surface.";
        else if (step == Step.Previewing)
            statusText.text = "Tap elsewhere to move the table, or press Confirm.";
        else
            statusText.text = "Table placed. Press Begin when you are ready.";
    }
}