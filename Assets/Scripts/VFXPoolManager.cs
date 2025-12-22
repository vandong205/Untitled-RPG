using UnityEngine;
using System.Collections.Generic;

public class VFXPoolManager : SingletonPattern<VFXPoolManager>
{
    public GameObject VFXPrefab;
    private List<GameObject> pool;
    private int poolSize = 10;

    protected override void Awake()
    {
        base.Awake();
        pool = new List<GameObject>();
        Init();
    }
    private void Init()
    {
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(VFXPrefab,transform);
            obj.SetActive(false);
            pool.Add(obj);

        }
    }
    public GameObject getVFXObj()
    {
        foreach (GameObject obj in pool)
        {
            if (obj.activeSelf == false)
            {
                ResetVFxObj(obj);
                return obj;
            }
        }
        GameObject newVFxobj = Instantiate(VFXPrefab,transform);
        pool.Add(newVFxobj);
        return newVFxobj;
    }
    private void ResetVFxObj(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;      
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        VFXPlayer vFXPlayer = obj.GetComponent<VFXPlayer>();
        if (vFXPlayer != null) {
            vFXPlayer.ResetState();
        }
    }
}
