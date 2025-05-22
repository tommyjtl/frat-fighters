using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteSwapController : MonoBehaviour
{
    [Header("UI Image Settings")]
    public Image uiImage;
    public Sprite[] sprites;

    [Header("Timing Settings")]
    public float enableDelay = 2f; // Time before enabling the image
    public float duration = 5f; // Duration to change colors before disabling
    public float colorChangeInterval = 0.1f; // Interval to change colors

    private bool isColor1 = true; // Toggle between colors
    private float timer = 0f; // Timer to track elapsed time

    void Start()
    {
        // Ensure the UI Image starts disabled
        if (uiImage != null)
        {
            uiImage.enabled = false;
            StartCoroutine(HandleImageAppearance());
        }
        else
        {
            Debug.LogError("UI Image component is not assigned.");
        }
    }

    IEnumerator HandleImageAppearance()
    {
        // Wait for the specified delay before enabling the image
        yield return new WaitForSecondsRealtime(enableDelay);

        // Enable the UI Image
        uiImage.enabled = true;

        // Start changing colors
        yield return StartCoroutine(ChangeColor());

        // Disable the UI Image
        uiImage.enabled = false;
    }

    IEnumerator ChangeColor()
    {
        while (timer < duration)
        {
            // Alternate between the two colors
            uiImage.sprite = isColor1 ? sprites[1] : sprites[0];
            isColor1 = !isColor1;

            // Wait for the specified interval before changing color again
            yield return new WaitForSecondsRealtime(colorChangeInterval);

            // Update the timer
            timer += colorChangeInterval;
        }
    }

}