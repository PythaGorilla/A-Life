using Assets.Player;
using UnityEngine;
using System.Collections.Generic;


namespace Assets.WorldObject.Unit
{
    public class Unit : WorldObject
    {


        protected bool Moving, Rotating;

        private Vector3 _destination;
        private Quaternion _targetRotation;
        public float MoveSpeed, RotateSpeed;
        protected List<WorldObject> nearbyObjects;

        /*** Game Engine methods, all can be overridden by subclass ***/

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            if (Rotating) TurnToTarget();
            else if (Moving) MakeMove();
        }

        protected override void OnGui()
        {
            base.OnGui();
        }

        protected override void DecideWhatToDo()
        {
            base.DecideWhatToDo();
            if (CanAttack())
            {
                List<WorldObject> enemyObjects = new List<WorldObject>();
                foreach (WorldObject nearbyObject in nearbyObjects)
                {
                    Resource.Resource resource = nearbyObject.GetComponent<Resource.Resource>();
                    if (resource) continue;
                    if (nearbyObject.GetPlayer() != player) enemyObjects.Add(nearbyObject);
                }
                WorldObject closestObject = WorkManager.FindNearestWorldObjectInListToPosition(enemyObjects, currentPosition);
                if (closestObject) BeginAttack(closestObject);
            }
        }

        public override void SetHoverState(GameObject hoverObject)
        {
            base.SetHoverState(hoverObject);
            Debug.Log ("unit sethoverstate called");
            //only handle input if owned by a human player and currently selected
            Debug.Log(player);
            Debug.Log (player.Human);
            Debug.Log (CurrentlySelected);
            if (player && player.Human && CurrentlySelected)
            {
                if (hoverObject.name == "Ground") player.Hud.SetCursorState(CursorState.Move);
                Debug.Log ("move cursor set");
            }
        }

        public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player.Player controller)
        {
            base.MouseClick(hitObject, hitPoint, controller);
            //only handle input if owned by a human player and currently selected
            if (player && player.Human && CurrentlySelected)
            {
                if (hitObject.name == "Ground" && hitPoint != ResourceManager.InvalidPosition)
                {
                    float x = hitPoint.x;
                    //makes sure that the unit stays on top of the surface it is on
                    float y = hitPoint.y + player.SelectedObject.transform.position.y;
                    float z = hitPoint.z;
                    Vector3 destination = new Vector3(x, y, z);
                    //Debug.Log ("start moving");
                    StartMove(destination);
                }
            }
        }

        public void StartMove(Vector3 destination)

        {
            this._destination = destination;
            _targetRotation = Quaternion.LookRotation(destination - transform.position);
            Rotating = true;
            Moving = false;
            InAction = true;
        }

        private void TurnToTarget()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, RotateSpeed);
            //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
            Quaternion inverseTargetRotation = new Quaternion(-_targetRotation.x, -_targetRotation.y, -_targetRotation.z, -_targetRotation.w);
            if (transform.rotation == _targetRotation || transform.rotation == inverseTargetRotation)
            {
                Rotating = false;
                Moving = true;
                InAction = true;

            }
            CalculateBounds();
        }

        private void MakeMove()
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * MoveSpeed);
            if (transform.position == _destination) Moving = false;
            CalculateBounds();
        }
    }
}