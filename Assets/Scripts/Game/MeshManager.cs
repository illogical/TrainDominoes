// TODO: MeshManager eventually needs to track the existing meshes along with the ability to make a mesh for each domino
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.TMP_InputField;

namespace Assets.Scripts.Game
{
    public class MeshManager : MonoBehaviour
    {
        public GameObject DominoMesh;
        public Camera Camera;
        public Transform Player;
        public SelectionEvent DominoSelected;
        public SelectionEvent TrackSelected;
        // works well for camera at 75 degree FOV
        [Space]
        public Vector3 PlayerDistanceFromCamera = new Vector3(0, -0.105f, 0.08f);
        public float StationDistanceFromCamera = 0.05f;
        public float DistanceWhenSelected = 0.01f;
        public float SelectSpeed = 0.3f;
        public float MoveToTrackSpeed = 0.25f;

        public GameObject TrackPositions; // used to parent all other tracks to

        private Dictionary<int, GameObject> playerDominoes;
        private Dictionary<int, GameObject> tableDominoes = new Dictionary<int, GameObject>();  // TODO: need a wrapper for tracking each domino's direction on the table, and its parent/child domino relationship. Part of TableManager class?


        private GameObject selectedPlayerDomino;
        private bool allowClick = true;
        
        // TODO: might want a state machine for when the Domino is selected then the next state would be a TableDomino if one side or the other matches

        void Start()
        {

        }

        void Update()
        {
            HandleDominoSelection();
        }


        private void HandleDominoSelection()
        {
            if (allowClick && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {                    
                    if (hit.transform.tag == "Domino")
                    {                       
                        Debug.Log($"Domino {hit.transform.name} was clicked. Position = ({hit.transform.position.x}, {hit.transform.position.y}, {hit.transform.position.z})");

                        DominoSelected?.Invoke(hit.transform.name);
                    }
                }
            }
        }

        public void SelectDomino(GameObject selected)
        {
            if(selected == selectedPlayerDomino)
            {
                return;
            }

            allowClick = false; // TODO: prevent clicking while these animations are running. This is fragile. Might be a hint I need a state machine for while a selection is happening

            if (selectedPlayerDomino != null)
            {
                // animation
                ShowDeselectedDomino(selectedPlayerDomino);
                    //.ContinueWith(ShowSelectedDomino(selected));
            }
            else
            {
                selectedPlayerDomino = selected;

                // animation
                ShowSelectedDomino(selected);
            }     
        }



        public void DeselectDomino()
        {
            ShowDeselectedDomino(selectedPlayerDomino);
            selectedPlayerDomino = null;
        }

        public void ClearSelectedDomino()
        {
            selectedPlayerDomino = null;
        }

        private void ShowSelectedDomino(GameObject domino)
        {
            var currentX = domino.transform.position.x;
            domino.transform.position = PlayerDistanceFromCamera + new Vector3(currentX, DistanceWhenSelected, 0);
        }

        private void ShowDeselectedDomino(GameObject domino)
        {
            var currentX = domino.transform.position.x;
            var down = PlayerDistanceFromCamera + new Vector3(currentX, 0, 0);
            domino.transform.position = down;
        }

        //private Vector3Tween ShowSelectedDomino(GameObject domino)
        //{
        //    var currentX = domino.transform.position.x;
        //    var up = PlayerDistanceFromCamera + new Vector3(currentX, DistanceWhenSelected, 0);

        //    System.Action<ITween<Vector3>> updateDominoPos = (t) => domino.gameObject.transform.position = t.CurrentValue;

        //    System.Action<ITween<Vector3>> circleMoveCompleted = (t) =>
        //    {
        //        selectedPlayerDomino = domino;
        //        allowClick = true;
        //    };

        //    return domino.transform.gameObject.Tween("SelectMoveUp", domino.transform.position, up, SelectSpeed, TweenScaleFunctions.CubicEaseOut, updateDominoPos, circleMoveCompleted);
        //}

        //private Vector3Tween ShowDeselectedDomino(GameObject domino)
        //{
        //    var currentX = domino.transform.position.x;
        //    var down = PlayerDistanceFromCamera + new Vector3(currentX, 0, 0);
        //    System.Action<ITween<Vector3>> updateDominoPos = (t) => domino.transform.position = t.CurrentValue;


        //    return domino.Tween("DeselectMoveDown", domino.transform.position, down, SelectSpeed, TweenScaleFunctions.CubicEaseIn, updateDominoPos);
        //}

        //private Vector3Tween MovePlayerDominoToTrack(GameObject domino, Vector3 destinationPosition)
        //{           
        //    System.Action<ITween<Vector3>> updateDominoPos = (t) => domino.gameObject.transform.position = t.CurrentValue;

        //    return domino.Tween("MovePlayerDominoToTrack", domino.transform.position, destinationPosition, MoveToTrackSpeed, TweenScaleFunctions.CubicEaseInOut, updateDominoPos);
        //}

        //private void RotateDominoAddedToTrack(Transform dominoTransform, Vector3 destinationRotation)
        //{
        //    Tween.Rotation(dominoTransform, destinationRotation, 0.3f, 0.05f, Tween.EaseInOutStrong);
        //}

        public void CreateMesh(DominoEntity domino)
        {
            domino.UpdateDominoLabels();
            var newMesh = Instantiate(DominoMesh, DominoMesh.transform.position, DominoMesh.transform.rotation, Player);
            // TODO: check if this one already exists? Change this method to GetOrCreateMesh()
            newMesh.name = domino.ID.ToString();
            playerDominoes.Add(domino.ID, newMesh);
        }

        public void CreateMeshes(List<DominoEntity> dominoes)
        {
            foreach (var domino in dominoes)
            {
                CreateMesh(domino);
            }
        }        


        // TODO: no need to delete any meshes. Will store meshes in an array. May need to manage the Active flags on all of them
        //private void removeMesh(DominoEntity domino)
        //{
        //    Destroy(domino.Mesh);
        //    domino.Mesh = null;
        //}
        

        public void AlignPlayerDominoes(List<DominoEntity> dominoes)
        {
            // Vector3 relativePosition = objectA.transform.InverseTransformPoint(objectB.transform.position); // example of finding relative distance between objects
            var dominoCount = dominoes.Count;
            var renderer = dominoes[0].GetComponent<Renderer>(); // use any domino?
            float width = renderer.bounds.size.x;
            float margin = width / 4;
            float totalWidth = dominoCount * margin + dominoCount * width;
            float xOffset = totalWidth / 2;

            for (int i = 0; i < dominoCount; i++)
            {
                float evenOffset = dominoCount % 2 == 0 ? (width / 2) + (margin / 2) : 0;
                float xPos = (i * (width + margin)) - xOffset + evenOffset;
                // TODO: Need to fix the position updating in AlignPlayerDominoes()
                throw new NotImplementedException("Need to fix the position updating in AlignPlayerDominoes()");
                //dominoes[i].Mesh.transform.position = new Vector3(xPos, 0, 0) + PlayerDistanceFromCamera; // DistanceFromCamera provides the y and z
            }
        }

        //public void AlignDominoTracks(List<DominoEntity> dominoes) // TODO: pass in track as Dictionary<int, DominoEntity> when ready to add more tracks
        //{            
        //    // TODO: repurpose this to handle multiple domino tracks?
        //    var renderer = dominoes[0].Mesh.GetComponent<Renderer>();            

        //    for (int i = 0; i < dominoes.Count; i++)
        //    {

        //        //AlignTrackEmpty(trackStartPosition);    // TODO: maybe parent domino to the empty?

        //        RotateDominoAddedToTrack(dominoes[i].Mesh.transform, new Vector3(0, 90, 90));

        //        var trackStartPosition = GetTrackPostion(i + 1, 1, renderer.bounds.size);
        //        MovePlayerDominoToTrack(dominoes[i].Mesh, trackStartPosition);
        //    }
        //}

        // TODO: Fix AlignDominoToTrack() (need to manage a single list of all the domino meshes)
        public void AlignDominoToTrack(DominoEntity domino, int dominoCount, int trackIndex)
        {            
            // var renderer = domino.Mesh.GetComponent<Renderer>();
        
            //RotateDominoAddedToTrack(domino.Mesh.transform, new Vector3(0, 90, 90));
            //MovePlayerDominoToTrack(domino.Mesh, GetTrackPostion(dominoCount, trackIndex, renderer.bounds.size));
        }

        // TODO: Fix AlignEngine() (need to manage a single list of all the domino meshes)
        /*
        public void AlignEngine(DominoEntity domino) // TODO: pass in track as Dictionary<int, DominoEntity> when ready to add more tracks
        {            
            var renderer = domino.Mesh.GetComponent<Renderer>();
            float width = renderer.bounds.size.y; // will flip horizontally so use y            
            float xOffset = width / 2;

            var leftSide = GetScreenCenterLeft();

            // get position of empty to provide y & z
            Vector3 linePos = new Vector3(leftSide.x, leftSide.y, StationDistanceFromCamera);

            domino.Mesh.transform.Rotate(new Vector3(0, -90, 0));
            domino.Mesh.transform.position = new Vector3(xOffset, 0, 0) + linePos;
        }
        */

        public void AddDominoToTrack(DominoEntity domino)
        {
            // TODO: need a new list of dominoes on the track to know which one was last
            // TODO: need a way to flip a domino (could just swap Top/Bottom numbers for now but would be cooler with an animation)
        }

        private void AlignTrackEmpty(Vector3 trackPosition)
        {
            // TODO: create an empty and begin managing 0-8 tracks
            TrackPositions.transform.position = trackPosition;
        }

        private Vector3 GetTrackPostion(int dominoCount, int trackNumber, Vector3 dominoSize)
        {
            float width = dominoSize.y;
            float height = dominoSize.z;

            float xOffset = width / 2;
            float yOffset = height / 2 * trackNumber;

            var leftSide = GetScreenCenterLeft();
            Vector3 linePos = new Vector3(leftSide.x, leftSide.y, StationDistanceFromCamera);

            return new Vector3(dominoCount * width + xOffset, yOffset, 0) + linePos;
        }

        private Vector3 GetScreenCenterLeft()
        {
            return Camera.ViewportToWorldPoint(new Vector3(-1, 0.5f, StationDistanceFromCamera));
        }

        //public Vector3 GetScreenPosition(Transform transform, Canvas canvas, Camera cam)
        //{
        //    Vector3 pos;
        //    float width = canvas.GetComponent<RectTransform>().sizeDelta.x;
        //    float height = canvas.GetComponent<RectTransform>().sizeDelta.y;
        //    float x = Camera.main.WorldToScreenPoint(transform.position).x / Screen.width;
        //    float y = Camera.main.WorldToScreenPoint(transform.position).y / Screen.height;
        //    pos = new Vector3(width * x - width / 2, y * height - height / 2);
        //    return pos;
        //}
    }
}