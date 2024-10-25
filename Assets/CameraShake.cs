using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.5f; // Magnitude of shake during normal movement
    public float boostShakeMagnitude = 0.8f; // Magnitude of shake while boosting

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition; // Store original camera position
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude; // Randomize x shake
            float y = Random.Range(-1f, 1f) * magnitude; // Randomize y shake

            transform.localPosition = originalPosition + new Vector3(x, y, 0); // Apply shake to camera position

            elapsed += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        transform.localPosition = originalPosition; // Reset camera position after shaking
    }
}
