using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Collider2D swordCollider;  
    public LayerMask enemyMask;
    public int damage = 20;
    public Transform m_hitpoint;
    public void Hit()
    {
        Debug.Log("Dang kiem tra Hit");

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyMask);
        filter.useTriggers = true; 

        Collider2D[] results = new Collider2D[10];
        int count = swordCollider.Overlap(filter, results);

        Debug.Log("Da chem trung " + count);        
        for (int i = 0; i < count; i++)
        {
            IEnemy enemy = results[i].GetComponentInParent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                //VFX
                GameObject vfxobj = VFXPoolManager.Instance.getVFXObj();
                VFXPlayer vfxplayer = vfxobj?.GetComponent<VFXPlayer>();
                List<Sprite> sprites = VFXSpriteManager.Instance.GetVFX(Consts.VFXName.Hit_Effect);
                if (vfxobj != null)
                {
                    vfxobj.transform.SetParent(transform);
                    vfxobj.transform.localPosition = m_hitpoint.localPosition;
                    if (vfxplayer != null)
                    {
                        if (sprites.Count == 0)
                        {
                            Debug.LogWarning("Khong the chay VFXplayer do khong ton tai VFX " + Consts.VFXName.Hit_Effect);
                        }
                        else
                        {
                            vfxplayer.SetSpriteList(sprites);
                            vfxplayer.Play(27);
                        }
                    }
                }
            }
        }
    }
}
