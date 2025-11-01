using Pooling;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.Editor;
using UnityEngine;
using UnityEngine.Profiling;

public class ParticlePoolObject : PoolObject
{
    [SerializeField]
    private List<ParticleSystem> ps;

    public override void Commission()
    {
        base.Commission();
        PlayPs();
    }

    public override void Decommission()
    {
        StopPs();
        this.ReturnToPool();
    }

    private void PlayPs()
    {
        for (int i = 0; i < ps.Count; ++i)
        {
            ps[i].Play();
        }
    }

    private void StopPs()
    {
        for(int i = 0; i < ps.Count; ++i)
        {
            ps[i].Stop();
            ps[i].Clear();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Profiler.BeginSample("Particle destroy");
        Debug.Log($"Particle {name} destroyed");
        Profiler.EndSample();
    }

    private void OnDisable()
    {
        Decommission();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (ps == null || ps.Count == 0)
            GetComponentsInChildren<ParticleSystem>(true, ps);
    }
#endif

}
