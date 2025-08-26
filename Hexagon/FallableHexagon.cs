using UnityEngine;
using System.Collections;

namespace FallMonke.Hexagon;

public class FallableHexagon : MonoBehaviour
{
    private Renderer renderer;
    private Color originalColor;
    private AudioSource audioSource;

    public bool IsFalling; // also for if it has fallen

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        gameObject.AddComponent<GorillaSurfaceOverride>().overrideIndex = 93;
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
        StopAllCoroutines();
        gameObject.SetActive(true);
        TileAnimation(down: false);
        IsFalling = false;
        renderer.material.color = originalColor;

        StartCoroutine(ChangeColor(Color.white, originalColor));

        audioSource.spatialBlend = 0;
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void TileAnimation(bool down)
    {
        GetComponent<Animator>().SetBool("TileDown", down);
    }

    private IEnumerator FallingCorountine()
    {
        IsFalling = true;

        audioSource.spatialBlend = 1;
        audioSource.PlayOneShot(audioSource.clip);
        TileAnimation(down: true);

        yield return ChangeColor(originalColor, Color.white);
        yield return new WaitForSeconds(0.25f);

        renderer.material.color = Color.white;
        gameObject.SetActive(false);
    }

    private IEnumerator ChangeColor(Color from, Color to)
    {
        float elapsed = 0f;
        while (elapsed < 0.25f)
        {
            renderer.material.color = Color.Lerp(from, to, elapsed / 0.25f);
            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(0.005f);
        }
    }
}
