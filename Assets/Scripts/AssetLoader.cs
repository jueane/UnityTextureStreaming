using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLoader : MonoBehaviour
{
    public string path;

    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetAsync<GameObject>(path).Completed += OnCompleted;
    }

    void OnCompleted(AsyncOperationHandle<GameObject> obj)
    {
        var asset = obj.Result;
        var newObject = GameObject.Instantiate(asset);
        newObject.transform.SetParent(transform);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
