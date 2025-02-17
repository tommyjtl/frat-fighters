using UnityEngine;

namespace BeatEmUpTemplate2D {

    //a general collection of math functions that are useful throughout the project
    public static class MathUtilities {
	
        //ease-in curve calculation
	    public static float Sinerp(float value)	{
		    return Mathf.Lerp(0,1, Mathf.Sin(value * Mathf.PI * 0.5f));
	    }

	    //ease-out curve calculation
	    public static float Coserp(float value)	{
		    return Mathf.Lerp(0, 1, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	    }

	    //curve calculation for Ease-in and Ease-out
	    public static float CoSinLerp(float value) {
            return Mathf.Lerp(0, 1, value * value * (3.0f - 2.0f * value));
        }
		
	    //draw a rectangle Gizmo
        public static void DrawRectGizmo(Vector2 pos, Vector2 size, Color color, float duration) {
            Vector3 topLeft = (Vector3)pos + Vector3.left * size.x/2f + Vector3.up * size.y/2f;
            Vector3 topRight = topLeft + Vector3.right * size.x;
            Vector3 BottomLeft = topLeft + Vector3.down * size.y;
            Vector3 BottomRight = topRight + Vector3.down * size.y;

            Debug.DrawLine(topLeft, topRight, color, duration);
            Debug.DrawLine(BottomLeft, BottomRight, color, duration);
            Debug.DrawLine(BottomLeft, topLeft, color, duration);
            Debug.DrawLine(BottomRight, topRight, color, duration);
        }
    }
}