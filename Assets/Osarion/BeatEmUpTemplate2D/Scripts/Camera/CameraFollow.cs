using UnityEngine;
using UnityEngine.U2D;

namespace BeatEmUpTemplate2D {

    //class for following targets with the camera
    public class CameraFollow : MonoBehaviour {

	    [Header ("Player Targets")]
	    [SerializeField] private GameObject[] targets; //target(s) to follow
        [SerializeField] private bool restrictTargetsToCamView = true;
        [SerializeField] private float RTTCVVerticalAdjust = 0.0f;
        [SerializeField] private float borderMargin = 0.05f;

	    [Header ("Follow Settings")]
	    [SerializeField] private  float yOffset = 0; // the height offset of the camera relative to it's target
        [HideInInspector] public float additionalYOffset = 0; //used by cam shake

	    [Header ("Damp Settings")]
	    [SerializeField] private float DampX = 3f;
	    [SerializeField] private float DampY = 1f;

	    [Header ("View Area")]
	    [SerializeField] private float Left;
	    [SerializeField] private float Right;
        [SerializeField] private float Top;
	    [SerializeField] private float Bottom;
        [SerializeField] private bool showViewAreaRect; //draws the borders of the camera view and view area

        [Header ("BackTracking")]
        [SerializeField] private bool allowBacktracking; //enable or disable moving backwards in the level
        [SerializeField] private float BackTrackMargin; //a small margin to allow moving back slightly

        [Header ("Level Bound")]
        public LevelBound levelBound;

        private Vector3 centerPos; //center position of all current targets
        private Vector3 prevPos; //previous frame camera position
        private float camX;
        private float camY;
        private PixelPerfectCamera pixelPerfectCamera;

        [Header ("Lock Vertical")]
        [SerializeField] private bool lockVertical; //locks camera so it can't move vertically

        public static bool IsZooming = false;

        void Start(){
		    GetPlayerTargets();
            prevPos = transform.position;
            centerPos = GetCenterPosOfAllTargets();
            camX = centerPos.x;
            if (lockVertical) {
                camY = centerPos.y;
            } else {
                camY = centerPos.y - yOffset;
            }
	    }

	    void Update () {
            if (IsZooming) return;
            if (targets.Length == 0) return;
			
            //calculate x,y position
            centerPos = GetCenterPosOfAllTargets(); //get center position of all current targets
            camX = Mathf.Lerp(prevPos.x, centerPos.x, DampX * Time.deltaTime);
            if (!lockVertical) camY = Mathf.Lerp(prevPos.y, centerPos.y - yOffset, DampY * Time.deltaTime);

            //restrict camera position to View Area
            if(float.IsNaN(camX) || float.IsNaN(camY)) return;
            transform.position = new Vector3(Mathf.Clamp(camX, Left, Right), Mathf.Clamp(camY, Bottom, Top), transform.position.z);
            prevPos = transform.position;

            //restrict camera right side to current wave level bound
            if(levelBound != null){
                float halfSize = Camera.main.orthographicSize * Camera.main.aspect;
                float cameraRightEdge = Camera.main.transform.position.x + halfSize;
                float levelBoundPos = levelBound.transform.position.x;
                if(cameraRightEdge > levelBoundPos) Camera.main.transform.position = new Vector3(levelBoundPos - halfSize, Camera.main.transform.position.y, Camera.main.transform.position.z);
            }

            //keep targets in screen view
            if(restrictTargetsToCamView) RestrictTargetsToScreen();

            //camera backtracking
            if(!allowBacktracking){
                float cameraLeftEdge = Camera.main.transform.position.x;
                if(cameraLeftEdge - BackTrackMargin > Left) Left = cameraLeftEdge - BackTrackMargin; //move left border forward
            }
        
            //visualize Camera view and ViewArea for debugging
            if(showViewAreaRect){
                ShowCameraView();
                ShowViewArea();
            }
	    }

        private void LateUpdate() {
            //apply any additional offset
            transform.position += Vector3.up * additionalYOffset;
        }

        //updates the targets to follow
        public void GetPlayerTargets(){
		    targets = GameObject.FindGameObjectsWithTag ("Player");
	    }

        //returns the center position of all current targets
        public Vector3 GetCenterPosOfAllTargets(){
                if(targets.Length == 0) return Vector3.zero;
                centerPos = Vector3.zero;
			    int count = 0;
			    for(int i=0; i<targets.Length; i++){
				    if(targets[i]){
					    centerPos += targets[i].transform.position;
					    count ++;
				    }
			    }
			    return centerPos/count;
        }

        //keep targets within screen view
        void RestrictTargetsToScreen(){
            foreach(GameObject target in targets){

                //calculate target screen bounds
                Vector3 viewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);
                viewportPosition.x = Mathf.Clamp(viewportPosition.x, borderMargin, 1f - borderMargin);
                viewportPosition.y = Mathf.Clamp(viewportPosition.y, borderMargin - RTTCVVerticalAdjust, 1f - borderMargin);

                //convert viewport coordinates to world coordinates
                Vector3 clampedWorldPosition = Camera.main.ViewportToWorldPoint(viewportPosition);

                //set position
                target.transform.position = new Vector2(clampedWorldPosition.x, clampedWorldPosition.y);
            }
        }

        //checks to avoid invalid (overlapping) numbers
        void OnValidate() {

            //right side can never subceed left border
            if(Right < Left) Right = Left;

            //left side can never exceed right side
            if(Left > Right) Left = Right;

            //top can never subceed bottom
            if(Top < Bottom) Top = Bottom;

            //bottom can never exceed top
            if(Bottom > Top) Top = Bottom;
        }

        //draw the Camera view Area in the Unity Editor
        void ShowCameraView(){
            if(pixelPerfectCamera == null){ pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>(); return; }

            float aspectRatio = (float)pixelPerfectCamera.refResolutionX / pixelPerfectCamera.refResolutionY;
            float cameraHalfHeight = pixelPerfectCamera.refResolutionY / (2f * pixelPerfectCamera.assetsPPU);
            float cameraHalfWidth = cameraHalfHeight * aspectRatio;

            Vector2 topLeft = new Vector2(transform.position.x - cameraHalfWidth, transform.position.y + cameraHalfHeight);
            Vector2 bottomLeft = new Vector2(transform.position.x - cameraHalfWidth, transform.position.y - cameraHalfHeight);
            Vector2 topRight = new Vector2(transform.position.x + cameraHalfWidth, transform.position.y + cameraHalfHeight);
            Vector2 bottomRight = new Vector2(transform.position.x + cameraHalfWidth, transform.position.y - cameraHalfHeight);
             
            Debug.DrawLine(bottomLeft, topLeft);
            Debug.DrawLine(topLeft, topRight);
            Debug.DrawLine(topRight, bottomRight);
            Debug.DrawLine(bottomRight, bottomLeft);
        }

        //draw the View Area Border in the Unity Editor
        void ShowViewArea(){
            if(pixelPerfectCamera == null){ pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>(); return; }

            float aspectRatio = (float)pixelPerfectCamera.refResolutionX / pixelPerfectCamera.refResolutionY;
            float cameraHalfHeight = pixelPerfectCamera.refResolutionY / (2f * pixelPerfectCamera.assetsPPU);
            float cameraHalfWidth = cameraHalfHeight * aspectRatio;
        
            Vector2 topLeft = new Vector2(Left - cameraHalfWidth, Top + cameraHalfHeight);
            Vector2 topRight = new Vector2(Right + cameraHalfWidth, Top + cameraHalfHeight);
            Vector2 bottomLeft = new Vector2(Left - cameraHalfWidth, Bottom - cameraHalfHeight);
            Vector2 bottomRight = new Vector2(Right + cameraHalfWidth, Bottom - cameraHalfHeight);

            Debug.DrawLine(bottomLeft, topLeft, Color.red);
            Debug.DrawLine(topLeft, topRight, Color.red);
            Debug.DrawLine(topRight, bottomRight, Color.red);
            Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        }
    }
}