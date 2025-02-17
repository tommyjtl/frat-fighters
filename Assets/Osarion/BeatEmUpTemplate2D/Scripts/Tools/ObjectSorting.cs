using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for sorting objects or units to appear in front or behind other sprites based on their y position
    public class ObjectSorting : MonoBehaviour {

        public static int SortingStep = -50;

        public virtual void Start(){
            Renderer rend = GetComponent<Renderer>();
            if(rend) ObjectSorting.Sort(rend, transform.position);
        }

        //sort an object based on it's position
        public static void Sort(Renderer rend, Vector2 position){
            if(rend) rend.sortingOrder = (int)(position.y * SortingStep);
        }

        private void OnValidate(){
            Sort(GetComponent<Renderer>(), transform.position);
        }
    }
}