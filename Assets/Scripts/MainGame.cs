using UnityEngine;
using System.Collections;
public class MainGame : SingletonPattern<MainGame>
{
    void Start()
    {
        StartCoroutine(LoadAsset());    
    }
    IEnumerator LoadAsset()
    {
        yield return LoadVFX();
        Debug.Log("Da xong qua trinh load assets");
    }
    IEnumerator LoadVFX()
    {
        string[] addresses ={
            Consts.VFXAddress.Weapon_Effect
        };
        int total = addresses.Length;
        int done = 0;

        foreach (var addr in addresses)
        {
            yield return VFXSpriteManager.Instance.LoadVFXFromAddressable(addr);
            done++;
            float percent = (float)done / total;
            Debug.Log($"Loading VFX: {(percent * 100f):0}%");
        }
    }
}
