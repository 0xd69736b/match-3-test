using Pooling;
using UnityEngine;

public class PoolObject : MonoBehaviour, IGenericPoolElement
{
    [HideInInspector]
    [SerializeField]
    protected Transform tr;
    [HideInInspector]
    [SerializeField]
    protected GameObject go;
    [SerializeField]
    protected Vector3 resetPos = new Vector3(100, 0, 100);

    public GameObject GO => go;
    public Transform Tr => tr;

    public int PoolRef { get; set; }
    public bool IsAvailable { get; }
    public bool IsCommissioned { get; set; }
    public bool UsesAutoPool { get; set; }

    public void SetPosition(Vector3 pos)
    {
        tr.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        tr.rotation = rot;
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        tr.position = pos;
        tr.rotation = rot;
    }

    public void SetParent(Transform parent)
    {
        tr.SetParent(parent);
    }

    public virtual void Commission()
    {
        go.SetActive(true);
    }

    public virtual void Decommission()
    {
        Tr.parent = null;
        Tr.position = resetPos;
        go.SetActive(false);
        this.ReturnToPool();
    }

    public virtual void OnDestroy()
    {
        this.RemoveFromPool();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Caching gameObject, because extra optimization points ;)
    /// </summary>
    protected virtual void OnValidate()
    {
        go = gameObject;
        tr = transform;
    }
#endif
}
