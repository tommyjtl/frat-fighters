using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    [RequireComponent(typeof(CameraFollow))]
    public class CameraZoom : MonoBehaviour
    {
        private Coroutine zoomCoroutine;

        public void ZoomTo(float targetSize, float duration, Vector3? focusPosition = null)
        {
            Debug.Log($"Zooming to {targetSize} over {duration} seconds.");
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(ZoomRoutine(targetSize, duration, focusPosition));
        }

        private IEnumerator ZoomRoutine(float targetSize, float duration, Vector3? focusPosition)
        {
            Camera cam = Camera.main;

            float startSize = cam.orthographicSize;
            float startTime = Time.unscaledTime;
            float endTime = startTime + duration;

            Vector3 startPos = cam.transform.position;
            Vector3 targetPos = focusPosition ?? startPos;
            targetPos.z = startPos.z; // preserve camera's Z

            while (Time.unscaledTime < endTime)
            {
                float t = (Time.unscaledTime - startTime) / duration;
                cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            cam.orthographicSize = targetSize;
            cam.transform.position = targetPos;
        }

        public void ZoomInOut(float zoomInSize, float duration, float holdTime, Vector3? focusPosition = null)
        {
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(ZoomInOutRoutine(zoomInSize, duration, holdTime, focusPosition));
        }

        private IEnumerator ZoomInOutRoutine(float zoomInSize, float duration, float holdTime, Vector3? focusOverride = null)
        {
            CameraFollow.IsZooming = true;

            Camera cam = Camera.main;
            float originalSize = cam.orthographicSize;
            Vector3 originalPos = cam.transform.position;

            // Get initial player center as target (like CameraFollow)
            Vector3 GetCenterOfPlayers()
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length == 0) return cam.transform.position;

                Vector3 sum = Vector3.zero;
                foreach (GameObject p in players)
                    sum += p.transform.position;

                Vector3 center = sum / players.Length;
                center.z = originalPos.z; // lock z
                return center;
            }

            Vector3 targetPos = focusOverride ?? GetCenterOfPlayers();

            // --- Zoom In ---
            float t = 0f;
            while (t < 1f)
            {
                cam.orthographicSize = Mathf.Lerp(originalSize, zoomInSize, t);
                cam.transform.position = Vector3.Lerp(originalPos, targetPos, t);
                t += Time.unscaledDeltaTime / duration;
                yield return null;
            }
            cam.orthographicSize = zoomInSize;
            cam.transform.position = targetPos;

            // --- Hold Zoom & Track Center ---
            float holdElapsed = 0f;
            while (holdElapsed < holdTime)
            {
                holdElapsed += Time.unscaledDeltaTime;

                Vector3 dynamicCenter = GetCenterOfPlayers();
                float damp = 3f;
                cam.transform.position = Vector3.Lerp(
                    cam.transform.position,
                    dynamicCenter,
                    damp * Time.unscaledDeltaTime
                );

                yield return null;
            }

            // --- Zoom Out ---
            Vector3 latestCenter = GetCenterOfPlayers();

            t = 0f;
            while (t < 1f)
            {
                cam.orthographicSize = Mathf.Lerp(zoomInSize, originalSize, t);
                cam.transform.position = Vector3.Lerp(latestCenter, originalPos, t);
                t += Time.unscaledDeltaTime / duration;
                yield return null;
            }

            cam.orthographicSize = originalSize;
            cam.transform.position = originalPos;

            CameraFollow.IsZooming = false;
        }

    }
}
