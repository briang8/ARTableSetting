using System.Collections.Generic;
using UnityEngine;

public class TablewareFactory : MonoBehaviour
{
    [SerializeField] private AssetLoader assetLoader;

    [Header("Tray grid (table space, metres)")]
    [SerializeField] private int columns = 3;
    [SerializeField] private float columnSpacing = 0.3f;
    [SerializeField] private float firstRowZ = -0.33f;
    [SerializeField] private float rowSpacing = 0.25f;

    private readonly Dictionary<string, ObjectPool<TablewareItem>> pools = new Dictionary<string, ObjectPool<TablewareItem>>();
    private readonly Dictionary<TablewareItem, ObjectPool<TablewareItem>> owners = new Dictionary<TablewareItem, ObjectPool<TablewareItem>>();
    private readonly List<TablewareItem> activeItems = new List<TablewareItem>();

    public List<TablewareItem> CreateItems(TableSettingLevel level, Transform table)
    {
        ReleaseAllItems();
        PlacementSlot[] slots = table.GetComponentsInChildren<PlacementSlot>();

        // Hide every guide first. Each item that is created shows the guide for its own slot.
        foreach (PlacementSlot slot in slots) slot.SetMarkerVisible(false);

        for (int i = 0; i < level.items.Count; i++)
        {
            ItemDefinition def = level.items[i];
            PlacementSlot slot = FindSlot(slots, def.slotId);
            GameObject prefab = assetLoader.GetPrefab(def.addressKey);

            if (slot == null || prefab == null)
            {
                Debug.LogError("Missing slot or prefab for item: " + def.itemType);
                continue;
            }

            ObjectPool<TablewareItem> pool = GetPool(def.addressKey, prefab);
            if (pool == null) continue;

            TablewareItem item = pool.Get();
            item.transform.SetParent(table, true);

            // Items are laid out in a grid on the front part of the table, centred left to right.
            int column = i % columns;
            int row = i / columns;
            float x = (column - (columns - 1) / 2f) * columnSpacing;
            float z = firstRowZ - row * rowSpacing;
            Vector3 local = new Vector3(x, slot.transform.localPosition.y, z);
            Vector3 trayPos = table.TransformPoint(local);

            // Cutlery starts turned 90 degrees so the trainee has to rotate it once.
            float turn = item is Cutlery ? -90f : 0f;
            Quaternion trayRot = table.rotation * Quaternion.Euler(0f, turn, 0f);

            item.Setup(slot, trayPos, trayRot);
            owners[item] = pool;
            activeItems.Add(item);
        }

        return new List<TablewareItem>(activeItems);
    }

    // Sends every item back to its pool (used on restart and before a new level).
    public void ReleaseAllItems()
    {
        foreach (TablewareItem item in activeItems)
        {
            if (item == null) continue;
            item.transform.SetParent(transform, true);
            owners[item].Release(item);
        }
        activeItems.Clear();
        owners.Clear();
    }

    // Destroys all pooled items. Call this before AssetLoader releases the assets.
    public void DestroyAll()
    {
        ReleaseAllItems();
        foreach (ObjectPool<TablewareItem> pool in pools.Values) pool.Clear();
        pools.Clear();
    }

    private ObjectPool<TablewareItem> GetPool(string key, GameObject prefab)
    {
        if (pools.TryGetValue(key, out ObjectPool<TablewareItem> existing)) return existing;

        TablewareItem itemPrefab = prefab.GetComponent<TablewareItem>();
        if (itemPrefab == null)
        {
            Debug.LogError("Prefab has no TablewareItem component: " + key);
            return null;
        }

        ObjectPool<TablewareItem> pool = new ObjectPool<TablewareItem>(itemPrefab, transform);
        pools[key] = pool;
        return pool;
    }

    private PlacementSlot FindSlot(PlacementSlot[] slots, string slotId)
    {
        foreach (PlacementSlot slot in slots)
        {
            if (slot.SlotId == slotId) return slot;
        }
        return null;
    }
}