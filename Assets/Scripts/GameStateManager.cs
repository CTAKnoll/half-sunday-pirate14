using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Camera))]
    public class GameStateManager : MonoBehaviour, IService
    {
        public AnimationCurve TransitionCurve;
        public GameObject GardenFocus;
        public GameObject StonkFocus;
        public GameObject CreditsFocus;
        public GameObject TitleFocus;
        
        public float CameraZ = -10;

        private Camera Camera;
        
        public enum GameState
        {
            Title,
            Garden,
            Stonks,
            Credits
        }

        public Dictionary<GameState, GameObject> FocusPoints;

        protected void Start()
        {
            ServiceLocator.RegisterAsService(this);
            Camera = GetComponent<Camera>();
            
            FocusPoints = new()
            {
                [GameState.Garden] = GardenFocus,
                [GameState.Stonks] = StonkFocus,
                [GameState.Credits] = CreditsFocus,
                [GameState.Title] = TitleFocus,
            };
        }

        public void PanToState(GameState state, float seconds = 0f)
        {
            Debug.Log("ZEZEZEZE");
            Vector3 endStateLoc = FocusPoints[state].transform.position;
            Vector2 startPos = new Vector2(Camera.transform.position.x, Camera.transform.position.y);
            Vector2 endPos = new Vector2(endStateLoc.x, endStateLoc.y);
            StartCoroutine(LerpToState(startPos, endPos, seconds));
        }

        private IEnumerator LerpToState(Vector2 startPos, Vector2 endPos, float seconds)
        {
            float currentTime = 0;
            while (currentTime < seconds)
            {
                currentTime += Time.deltaTime;
                Vector2 currentPos = Vector2.LerpUnclamped(startPos, endPos, TransitionCurve.Evaluate(currentTime / seconds));
                Camera.transform.position = new Vector3(currentPos.x, currentPos.y, CameraZ);
                yield return null;
            }
            Camera.transform.position = new Vector3(endPos.x, endPos.y, CameraZ);
        }
    }
}