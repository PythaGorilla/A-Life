using Assets.Resources;
using UnityEngine;

namespace Assets.Player
{
	public static class ResourceManager {
		public static float ScrollSpeed { get { return 100; } }
		public static float RotateSpeed { get { return 100; } }
		public static int ScrollWidth { get { return 15; } }
		public static float MinCameraHeight { get { return 10; } }
		public static float MaxCameraHeight { get { return 40; } }
		public static float RotateAmount { get { return 10; } }
		private static Vector3 _invalidPosition = new Vector3(-99999, -99999, -99999);
		public static Vector3 InvalidPosition { get { return _invalidPosition; } }

		private static GUISkin _selectBoxSkin;
		public static GUISkin SelectBoxSkin { get { return _selectBoxSkin; } }

		private static GameObjectList _gameObjectList;

		public static void StoreSelectBoxItems(GUISkin skin)
		{
			_selectBoxSkin = skin;
		}

		private static Bounds _invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
		public static Bounds InvalidBounds { get { return _invalidBounds; } }
		public static int BuildSpeed { get { return 2; } }

		public static void SetGameObjectList(GameObjectList objectList)
		{
			_gameObjectList = objectList;
		}

		public static GameObject GetBuilding(string name)
		{
			return _gameObjectList.GetBuilding(name);
		}

		public static GameObject GetUnit(string name)
		{
			return _gameObjectList.GetUnit(name);
		}

		public static GameObject GetWorldObject(string name)
		{
			return _gameObjectList.GetWorldObject(name);
		}

		public static GameObject GetPlayerObject()
		{
			return _gameObjectList.GetPlayerObject();
		}

		public static Texture2D GetBuildImage(string name)
		{
			//Debug.Log("name is");
			//Debug.Log(name);
			return _gameObjectList.GetBuildImage(name);
		}

	}
}