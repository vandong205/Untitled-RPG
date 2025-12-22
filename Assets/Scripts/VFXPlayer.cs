using System.Collections.Generic;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    public SpriteRenderer m_renderer;
    private List<Sprite> sprites = new List<Sprite>();
    private int index = 0;
    private float timer = 0f;
    private float frameTime = 0f;
    private bool isPlaying = false;

    private void Awake()
    {
        if (m_renderer == null)
        {
            m_renderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    public void SetSpriteList(List<Sprite> sl)
    {
        this.sprites = sl;
    }

    public void Play(int fps)
    {
        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("VFXPlayer: sprite list is empty!");
            return;
        }

        frameTime = 1f / fps;
        index = 0;
        timer = 0f;
        isPlaying = true;

        m_renderer.sprite = sprites[0];
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isPlaying) return;
        if (sprites == null || sprites.Count == 0) return;

        timer += Time.deltaTime;
        if (timer < frameTime) return;

        timer -= frameTime;
        index++;

        if (index >= sprites.Count)
        {
            isPlaying = false;
            transform.SetParent(VFXPoolManager.Instance.transform);
            gameObject.SetActive(false);
            return;
        }

        m_renderer.sprite = sprites[index];
    }

    public void ResetState()
    {
        index = 0;
        timer = 0f;
        isPlaying = false;
        if (sprites != null && sprites.Count > 0)
            m_renderer.sprite = sprites[0];
    }
}
