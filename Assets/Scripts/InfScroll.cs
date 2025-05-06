using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    public RectTransform content; // The RectTransform of the content to scroll
    public float scrollSpeed = 50f; // Speed at which the content scrolls upward
    public float resetPositionY = -500f; // Y position to reset the content
    public float startPositionY = 500f; // Initial Y position of the content
    public bool up = true;

    private void Update()
    {
        if (up) {
            // Move the content upward
            content.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Check if the content has moved past the reset position
            if (content.anchoredPosition.y > startPositionY)
            {
                // Reset the content's position to create a seamless loop
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, resetPositionY);
            }
        } else {
            // Move the content downward
            content.anchoredPosition += Vector2.down * scrollSpeed * Time.deltaTime;

            // Check if the content has moved past the reset position
            if (content.anchoredPosition.y < startPositionY)
            {
                // Reset the content's position to create a seamless loop
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, resetPositionY);
            }

        }
    }
}
