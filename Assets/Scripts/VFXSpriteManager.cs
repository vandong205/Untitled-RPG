using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using System.Linq;

public class VFXSpriteManager : SingletonPattern<VFXSpriteManager>
{
    private Dictionary<string,Sprite> VFXSprites = new Dictionary<string,Sprite>();
    public List<Sprite> GetVFX(string baseName)
    {
        return VFXSprites
            .Where(pair => pair.Key.StartsWith(baseName))
            .OrderBy(pair => ExtractFrameIndex(pair.Key))
            .Select(pair => pair.Value)
            .ToList();
    }

    private int ExtractFrameIndex(string key)
{
    int i = key.Length - 1;
    while (i >= 0 && char.IsDigit(key[i]))
        i--;

    string numberPart = key.Substring(i + 1);
    return int.TryParse(numberPart, out int index) ? index : 0;
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
