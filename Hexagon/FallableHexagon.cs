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
        try { StopCoroutine(FallingCorountine()); } catch (System.Exception ex) { Main.Log($"Failed to stop tile from falling during reset: {ex}", BepInEx.Logging.LogLevel.Warning); }
        gameObject.SetActive(true);
        TileAnimation(down: false);
        IsFalling = false;
        renderer.material.color = originalColor;
    }

    private void TileAnimation(bool down) =>
        GetComponent<Animator>().SetBool("TileDown", down);

    private IEnumerator FallingCorountine()
    {
        IsFalling = true;

        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        TileAnimation(down: true);

        yield return new WaitForSeconds(0.35f);

        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            renderer.material.color = Color.Lerp(originalColor, Color.white, elapsed / 0.15f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        renderer.material.color = Color.white;
        gameObject.SetActive(false);
        IsFalling = false;
    }

}
