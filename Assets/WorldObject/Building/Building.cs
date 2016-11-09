using System.Collections.Generic;
using Assets.Player;
using Assets.Player.RallyPoint;
using UnityEngine;

namespace Assets.WorldObject.Building
{
    public class Building : WorldObject
    {

        public float MaxBuildProgress;
        protected Queue<string> BuildQueue;
        private float _currentBuildProgress = 0.0f;
        private Vector3 _spawnPoint;
        protected Vector3 RallyPoint;
        public Texture2D RallyPointImage;
        public Texture2D SellImage;


        protected override void Awake()
        {

            BuildQueue = new Queue<string>();
            float spawnX = SelectionBounds.center.x + transform.forward.x*SelectionBounds.extents.x + transform.forward.x*10;
            float spawnZ = SelectionBounds.center.z + transform.forward.z + SelectionBounds.extents.z +
                           transform.forward.z*10;
            _spawnPoint = new Vector3(spawnX, 0.0f, spawnZ);
            RallyPoint = _spawnPoint;
            base.Awake();
        }

        protected override void Start()
        {
            Debug.Log("building initilized");
            base.Start();
        }

        protected override void Update()
        {
            ProcessBuildQueue();
            base.Update();
        }

        protected override void OnGui()
        {
            base.OnGui();
        }

        protected void CreateUnit(string unitName)
        {
            BuildQueue.Enqueue(unitName);
        }

        protected void ProcessBuildQueue()
        {
            if (BuildQueue.Count > 0)
            {
                _currentBuildProgress += Time.deltaTime*ResourceManager.BuildSpeed;
                if (_currentBuildProgress > MaxBuildProgress)
                {
                    if (player) player.AddUnit(BuildQueue.Dequeue(), _spawnPoint, RallyPoint, transform.rotation);
                    _currentBuildProgress = 0.0f;
                }
            }
        }

        public string[] GetBuildQueueValues()
        {
            string[] values = new string[BuildQueue.Count];
            int pos = 0;
            foreach (string unit in BuildQueue) values[pos++] = unit;
            return values;
        }

        public float GetBuildPercentage()
        {
            return _currentBuildProgress/MaxBuildProgress;
        }


        public override void SetSelection(bool selected, Rect playingArea)
        {
            base.SetSelection(selected, playingArea);
            if (player)
            {
                RallyPoint flag = player.GetComponentInChildren<RallyPoint>();
                if (selected)
                {
                    if (flag && player.Human && _spawnPoint != ResourceManager.InvalidPosition &&
                        RallyPoint != ResourceManager.InvalidPosition)
                    {
                        flag.transform.localPosition = RallyPoint;
                        flag.transform.forward = transform.forward;
                        flag.Enable();
                    }
                }
                else
                {
                    if (flag && player.Human) flag.Disable();
                }
            }
        }

        public bool HasSpawnPoint()
        {
            return _spawnPoint != ResourceManager.InvalidPosition && RallyPoint != ResourceManager.InvalidPosition;
        }

        public override void SetHoverState(GameObject hoverObject)
        {
            base.SetHoverState(hoverObject);
            //only handle input if owned by a human player and currently selected
            if (player && player.Human && CurrentlySelected)
            {
                if (hoverObject.name == "Ground")
                {
                    if (player.Hud.GetPreviousCursorState() == CursorState.RallyPoint) player.Hud.SetCursorState(CursorState.RallyPoint);
                }
            }
        }

        public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player.Player controller)
        {
            base.MouseClick(hitObject, hitPoint, controller);
            //only handle iput if owned by a human player and currently selected
            if (player && player.Human && CurrentlySelected)
            {
                if (hitObject.name == "Ground")
                {
                    if ((player.Hud.GetCursorState() == CursorState.RallyPoint || player.Hud.GetPreviousCursorState() == CursorState.RallyPoint) && hitPoint != ResourceManager.InvalidPosition)
                    {
                        SetRallyPoint(hitPoint);
                    }
                }
            }
        }

        public void SetRallyPoint(Vector3 position)
        {
            RallyPoint = position;
            if (player && player.Human && CurrentlySelected)
            {
                RallyPoint flag = player.GetComponentInChildren<RallyPoint>();
                if (flag) flag.transform.localPosition = RallyPoint;
            }
        }

        public void Sell()
        {
            if (player) player.AddResource(ResourceType.Money, SellValue);
            if (CurrentlySelected) SetSelection(false, PlayingArea);
            Destroy(this.gameObject);
            Debug.Log("destroyed");
        }
    }
}