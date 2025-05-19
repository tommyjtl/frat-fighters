using UnityEngine;

public class BounceUIElement : MonoBehaviour
{
    public float amplitude = 10f; // Height of the bounce
    public float frequency = 1f;  // Speed of the bounce
    public float delay = 5f;

    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private float deltaTime;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.localPosition;
        deltaTime = Time.time;
    }

    void Update()
    {
        if (Time.time > deltaTime + delay)
        {
            float yOffset = Mathf.Abs(Mathf.Sin((Time.time-deltaTime-delay) * frequency) * amplitude);
            rectTransform.localPosition = initialPosition + new Vector3(0, yOffset, 0);
        }
    }
}
