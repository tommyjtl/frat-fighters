using System.Collections;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    //behaviour for the bouncing ball in the training area
    public class BallBehaviour : MonoBehaviour {

        [SerializeField] private float bounceDuration;
        [SerializeField] private float bounceHeight;
        [SerializeField] private int bounces = 3;
        [SerializeField] private float bounceDistance = 2f;
        [SerializeField] private AnimationCurve bounceCurve;
        [SerializeField] private string bounceSFX = "";
        [SerializeField] private GameObject shadow;

        private Vector2 startPos; //initial position 
        private bool bounceInProgress;

        void OnEnable() {
		    UnitActions.onUnitDealDamage += OnHitObject; //subscribe to event
	    }

	    void OnDisable() {
		    UnitActions.onUnitDealDamage -= OnHitObject; //unsubscribe to event
	    }

        void Start(){
            startPos = transform.position;
            shadow = GameObject.Instantiate(shadow, transform);
            UpdateShadow();
        }

        //when this object was hit, start ball bounce animation
        void OnHitObject(GameObject obj, AttackData attackData){
            if(obj== this.gameObject){
                if(bounceInProgress) StopAllCoroutines(); //stop any bounce in progress
                StartCoroutine(BounceRoutine(attackData)); //start bouncing
            }
        }

        IEnumerator BounceRoutine(AttackData attackData){

            bounceInProgress = true;
            int attackDir = (int)attackData.inflictor.GetComponent<UnitActions>().dir; //check if the attack is coming from left or right

            //start position
            transform.position = new Vector2(transform.position.x, startPos.y);

            //the number of bounces
            for(int bounceCount=1; bounceCount<=bounces; bounceCount++){

                //bounce animation
                float t=0;
                while(t<1f){

                    //calculate new position
                    float xpos = transform.position.x + bounceDistance * Time.deltaTime * attackDir;
                    float ypos = startPos.y + bounceHeight / bounceCount * bounceCurve.Evaluate(t);

                    //don't move in x dir when we've hit a wall (Environment collider)
                    if(EnvironmentCollisionDetected(attackDir)) xpos = transform.position.x;

                    //move ball
                    transform.position = new Vector2(xpos, ypos);
                    UpdateShadow();  

                    //continue
                    t += Time.deltaTime / bounceDuration;
                    yield return 0;
                }

                //play bounce sfx
                if(bounceSFX.Length>0) BeatEmUpTemplate2D.AudioController.PlaySFX(bounceSFX, transform.position);

                //next bounce
                bounceCount ++;
            }
            bounceInProgress = false;
        }

        void UpdateShadow(){
            if(!shadow) return;
            shadow.transform.position = new Vector2(transform.position.x, startPos.y); //set shadow position
            shadow.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1; //put shadow behind ball
        }

        //returns true if we've hit the environment
        bool EnvironmentCollisionDetected(int attackDir){
            float spriteSizeX = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
            Vector3 From = new Vector2(transform.position.x, startPos.y);
            Vector3 To = From + Vector3.right * attackDir * spriteSizeX;
            RaycastHit2D hit = Physics2D.Linecast(From, To, 1 << LayerMask.NameToLayer("Environment")); //check if we've hit environment layer
            Debug.DrawLine(From, To, Color.yellow, Time.deltaTime); //show debug line in editor
            return hit.collider != null;
        }
    }
}
