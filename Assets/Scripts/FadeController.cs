using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    private float alpha;
    private Texture2D texture;
    public bool isFading {get; private set;}
    public bool isFadingIn {get; private set;}
    private float timer;
    private float duration;
    private const float DEF_DURATION = 1f;

    // Start is called before the first frame update
    void Start()
    {
        alpha = 0;
        texture = new Texture2D(1, 1);
        isFading = false;
        isFadingIn = false;
        timer = 0f;
        duration = 1f;
    }

    public void ToggleFade()
    {
        ToggleFade(DEF_DURATION);
    }

    public void ToggleFade(float duration)
    {
        timer = 0f;
        this.duration = duration;
        alpha = isFadingIn ? 1f : 0f;
        isFading = true;
    }

    public void FadeIn()
    {
        FadeIn(DEF_DURATION);
    }

    public void FadeIn(float duration)
    {
        isFadingIn = true;
        ToggleFade(duration);
    }

    public void FadeOut()
    {
        FadeOut(DEF_DURATION);
    }

    public void FadeOut(float duration)
    {
        isFadingIn = false;
        ToggleFade(duration);
    }

    private void OnGUI()
    {
        if (!isFading)
        {
            if (isFadingIn)
            {
                texture.SetPixel(0, 0, new Color(Color.black.r, Color.black.g, Color.black.b, 1f));
                texture.Apply();
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
            }
            else return;
        }
        if (texture == null) texture = new Texture2D(1, 1);
        timer += Time.deltaTime;
        alpha = isFadingIn ? timer / duration : 1 - (timer / duration);
        texture.SetPixel(0, 0, new Color(Color.black.r, Color.black.g, Color.black.b, alpha));
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        if (timer >= duration)
        {
            alpha = Mathf.Round(alpha);
            isFadingIn = alpha >= 1f ? true : false;
            isFading = false;
        }
    }
}
