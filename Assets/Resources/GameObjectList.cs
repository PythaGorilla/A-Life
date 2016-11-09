using Assets.Player;
using Assets.WorldObject.Building;
using Assets.WorldObject.Unit;
using UnityEngine;

namespace Assets.Resources
{
    public class GameObjectList : MonoBehaviour {
        public GameObject[] Buildings;
        public GameObject[] Units;
        public GameObject[] WorldObjects;
        public GameObject Player;
        private static bool _created = false;

        void Awake()
        {
            if (!_created)
            {
                DontDestroyOnLoad(transform.gameObject);
                ResourceManager.SetGameObjectList(this);
                _created = true;
            }
            else {
                Destroy(this.gameObject);
            }
        }


        // Use this for initialization
        void Start () {
    
        }
    
        // Update is called once per frame
        void Update () {
    
        }


        public GameObject GetBuilding(string name)
        {
            for (int i = 0; i < Buildings.Length; i++)
            {
                Building building = Buildings[i].GetComponent<Building>();
                if (building && building.name == name) return Buildings[i];
            }
            return null;
        }

        public GameObject GetUnit(string name)
        {
            for (int i = 0; i < Units.Length; i++)
            {
                Unit unit = Units[i].GetComponent<Unit>();
                if (unit && unit.name == name) return Units[i];
            }
            return null;
        }

        public GameObject GetWorldObject(string name)
        {
            foreach (GameObject worldObject in WorldObjects)
            {
                if (worldObject.name == name) return worldObject;
            }
            return null;
        }

        public GameObject GetPlayerObject()
        {
            return Player;
        }

        public Texture2D GetBuildImage(string name)
        {
            for (int i = 0; i < Buildings.Length; i++)
            {
                Building building = Buildings[i].GetComponent<Building>();
                if (building && building.name == name) return building.BuildImage;
            }
            for (int i = 0; i < Units.Length; i++)
            {
                Unit unit = Units[i].GetComponent<Unit>();
                if (unit && unit.name == name) return unit.BuildImage;
            }
            return null;
        }
    }
}
