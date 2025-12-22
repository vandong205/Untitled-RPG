using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class VFXSpriteManager : SingletonPattern<VFXSpriteManager>
{
    private Dictionary<string,Sprite> VFXSprites = new Dictionary<string,Sprite>();

    public List<Sprite> GetVFX(string baseName)
    {
        List<Sprite> result = new List<Sprite>();
        foreach (var pair in VFXSprites)
        {
            if (pair.Key.StartsWith(baseName))
            {
                result.Add(pair.Value);
            }
        }
        return result;
    }
    public IEnumerator LoadVFXFromAddressable(string address)
    {
        var handle = Addressables.LoadAssetAsync<SpriteAtlas>(address);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Load SpriteAtlas failed: {address}");
            yield break;
        }

        SpriteAtlas atlas = handle.Result;

        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        foreach (Sprite sprite in sprites)
        {
            VFXSprites[sprite.name] = sprite;
        }

        Debug.Log($"Loaded {sprites.Length} VFX sprites from {address}");
    }



}
