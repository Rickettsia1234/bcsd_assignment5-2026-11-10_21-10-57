using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenRed : MonoBehaviour
{
    private Image imageScreenRed;

    private void Awake()
    {
        imageScreenRed = GetComponent<Image>();
    }

    public void PlayHitAnimation()
    {
        StopCoroutine("FadeOut");
        StartCoroutine("FadeOut");
    }

    private IEnumerator FadeOut()
    {
        Color color = imageScreenRed.color;
        color.a = 0.5f;
        imageScreenRed.color = color;

        while (color.a > 0.0f)
        {
            color.a -= Time.deltaTime * 2.0f;
            imageScreenRed.color = color;
            yield return null;
        }
    }
}