using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace FallMonke.Hexagon;

public class FallableHexagon : MonoBehaviour
{
    private Renderer renderer;
    private Color originalColor;

    public bool IsFalling; // also for if it has fallen

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }

    public void Fall()
    {
        if (!IsFalling)
        {
            StartCoroutine(FallingCorountine());
        }
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        IsFalling = false;
        renderer.material.color = originalColor;
    }

    private IEnumerator FallingCorountine()
    {
        IsFalling = true;

        yield return new WaitForSeconds(0.35f);

        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            renderer.material.color = Color.Lerp(originalColor, Color.white, elapsed / 0.15f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);

        renderer.material.color = Color.white;
        gameObject.SetActive(false);
        IsFalling = false;
    }
}
