using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    [SerializeField] bool _haveEffect;
    [SerializeField] float _effectSize = 1;
    public Vector3 getPos()
    {
        return transform.localPosition;
    }
    public bool HaveEffect()
    {
        return _haveEffect;
    }
    public void PlayVFX()
    {
        if (!_haveEffect) return;
        GameObject vfxobj = VFXPoolManager.Instance.getVFXObj();
        VFXPlayer vfxplayer = vfxobj?.GetComponent<VFXPlayer>();

        List<Sprite> sprites = VFXSpriteManager.Instance.GetVFX(Consts.VFXName.Blood_Effect);
        if (vfxobj != null)
        {
            vfxobj.transform.SetParent(transform.parent);
            vfxobj.transform.localPosition = transform.localPosition;
            vfxobj.transform.localScale = new Vector3(_effectSize, _effectSize, _effectSize);   
            if (vfxplayer != null)
            {
                if (sprites.Count == 0)
                {
                    Debug.LogWarning("Khong the chay VFXplayer do khong ton tai VFX " + Consts.VFXName.Blood_Effect);
                }
                else
                {
                    Debug.LogWarning("Dang choi VFX,sprites = " + sprites.Count);
                    vfxplayer.SetSpriteList(sprites);
                    vfxplayer.Play(60);
                }
            }
        }
    }
}
