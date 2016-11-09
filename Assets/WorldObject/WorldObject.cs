using System.Collections.Generic;
using Assets.Player;
using UnityEngine;

namespace Assets.WorldObject
{
	public class WorldObject : MonoBehaviour {
		public string ObjectName;
		public Texture2D BuildImage;
		public int Cost, SellValue, HitPoints, MaxHitPoints;

		protected Player.Player player;
		protected string[] Operations = { };
		protected bool CurrentlySelected = false;
		protected bool InAction = false;
		protected Bounds SelectionBounds;
		protected Rect PlayingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
		private WorldObject _lastSelection;
		private float _sliderValue;
		private bool _attacking = false;
		private bool _movingIntoPosition = false;
		private bool _aiming = false;

		protected List<WorldObject> NearbyObjects;
		public int ObjectId { get; set; }
		public float DetectionRange = 20.0f;


		//we want to restrict how many decisions are made to help with game performance
		//the default time at the moment is a tenth of a second
		private float _timeSinceLastDecision = 0.0f, _timeBetweenDecisions = 0.1f;



		protected virtual void Awake()
		{
			SelectionBounds = ResourceManager.InvalidBounds;
			CalculateBounds();
		}

		protected virtual void Start()
		{
			player = transform.root.GetComponentInChildren<Player.Player>();
		}

		protected virtual void Update()
		{
			if (ShouldMakeDecision()) DecideWhatToDo();
		}

		/**
	 * A child class should only determine other conditions under which a decision should
	 * not be made. This could be 'harvesting' for a harvester, for example. Alternatively,
	 * an object that never has to make decisions could just return false.
	 */
		protected virtual bool ShouldMakeDecision()
		{
			if (_attacking || _movingIntoPosition || _aiming) return false;
			//we are not doing anything at the moment
			if (_timeSinceLastDecision > _timeBetweenDecisions)
			{
				_timeSinceLastDecision = 0.0f;
				return true;
			}
			_timeSinceLastDecision += Time.deltaTime;
			return false;
		}

		protected virtual void DecideWhatToDo()
		{
			//determine what should be done by the world object at the current point in time
			Vector3 currentPosition = transform.position;
			NearbyObjects = WorkManager.FindNearbyObjects(currentPosition, DetectionRange);
		}

		protected virtual void OnGui()
		{
			if (CurrentlySelected) DrawSelection();
		}

		public virtual void SetSelection(bool selected, Rect playingArea)
		{
			CurrentlySelected = selected;
			if (selected) this.PlayingArea = playingArea;

		}

		public string[] GetOperations()
		{
			//human player sent operations
			return Operations;
		}

		public virtual void PerformOperation(string operationToPerform)
		{
			//it is up to children with specific Operations to determine what to do with each of those Operations
		}

		public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player.Player controller)
		{
			//only handle input if currently selected
			if (CurrentlySelected && hitObject && hitObject.name != "Ground")
			{
				WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
				//clicked on another selectable object
				if (worldObject) ChangeSelection(worldObject, controller);
			}
		}

		private void ChangeSelection(WorldObject worldObject, Player.Player controller)
		{
			//this should be called by the following line, but there is an outside chance it will not
			SetSelection(false, PlayingArea);
			if (controller.SelectedObject) controller.SelectedObject.SetSelection(false, PlayingArea);
			controller.SelectedObject = worldObject;
			worldObject.SetSelection(true, controller.Hud.GetPlayingArea());
		}

		private void DrawSelection()
		{
			GUI.skin = ResourceManager.SelectBoxSkin;
			Rect selectBox = WorkManager.CalculateSelectionBox(SelectionBounds, PlayingArea);
			//Draw the selection box around the currently selected object, within the bounds of the playing area
			GUI.BeginGroup(PlayingArea);
			DrawSelectionBox(selectBox);
			GUI.EndGroup();
		}

		public void CalculateBounds()
		{
			SelectionBounds = new Bounds(transform.position, Vector3.zero);
			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				SelectionBounds.Encapsulate(r.bounds);
			}
		}

		protected virtual void DrawSelectionBox(Rect selectBox)
		{
			GUI.Box(selectBox, "");
		}

		public virtual void SetHoverState(GameObject hoverObject)
		{
			//only handle input if owned by a human player and currently selected
			if (player && player.Human && CurrentlySelected)
			{
				if (hoverObject.name != "Ground") player.Hud.SetCursorState(CursorState.Select);
			}
		}

		public bool IsOwnedBy(Player.Player owner)
		{
			if (player && player.Equals(owner))
			{
				return true;
			}
			else {
				return false;
			}
		}

		public Player.Player GetPlayer()
		{
			return player;
		}

	}
}
