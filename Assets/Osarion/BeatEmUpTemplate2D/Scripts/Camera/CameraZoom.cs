using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    [RequireComponent(typeof(CameraFollow))] // optional if you want to ensure it's used with follow
    public class CameraZoom : MonoBehaviour
    {
        private Coroutine zoomCoroutine;

        public void ZoomTo(float targetSize, float duration)
        {
            Debug.Log($"Zooming to {targetSize} over {duration} seconds.");
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(ZoomRoutine(targetSize, duration));
        }

        private IEnumerator ZoomRoutine(float targetSize, float duration)
        {
            float startSize = Camera.main.orthographicSize;
            float t = 0f;
            while (t < 1f)
            {
                Camera.main.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                t += Time.unscaledDeltaTime / duration;
                yield return null;
            }
            Camera.main.orthographicSize = targetSize;
        }
    }
}
