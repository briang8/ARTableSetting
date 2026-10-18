using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLoader : MonoBehaviour
{
    private readonly Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    private readonly List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

    public bool IsLoaded { get; private set; }

    public async Task LoadLevelAssets(TableSettingLevel level)
    {
        IsLoaded = false;

        foreach (ItemDefinition def in level.items)
        {
            // Already loaded (for example after a restart), so reuse it.
            if (prefabs.ContainsKey(def.addressKey)) continue;

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(def.addressKey);
            handles.Add(handle);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                prefabs[def.addressKey] = handle.Result;
            else
                Debug.LogError("Could not load Addressable: " + def.addressKey);
        }

        IsLoaded = true;
    }

    public GameObject GetPrefab(string key)
    {
        return prefabs.TryGetValue(key, out GameObject prefab) ? prefab : null;
    }

    public void ReleaseLevelAssets()
    {
        foreach (AsyncOperationHandle<GameObject> handle in handles)
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
        handles.Clear();
        prefabs.Clear();
        IsLoaded = false;
    }
}