using System.Collections.Generic;
using Assets.Player.HUD;
using Assets.WorldObject.Unit;
using UnityEngine;

namespace Assets.Player
{
    public class Player : MonoBehaviour {
        public Hud Hud;
        public string Username;
        public bool Human;
        public WorldObject.WorldObject SelectedObject { get; set; }
        public int StartMoney, StartMoneyLimit, StartPower, StartPowerLimit;
        private Dictionary<ResourceType, int> _resources, _resourceLimits;


        // Use this for initialization
        private void Start () {
            Debug.Log ("player initializing");
        

            Hud = GetComponentInChildren<Hud>();
        
            AddStartResourceLimits();
            AddStartResources();
        }
    
        // Update is called once per frame
        private void Update () {

            if (Human)
                Hud.SetResourceValues(_resources, _resourceLimits);
        }

        private void Awake()
        {
            _resources = InitResourceList();
            _resourceLimits = InitResourceList();
        }

        private Dictionary<ResourceType, int> InitResourceList()
        {
            var dict = new Dictionary<ResourceType, int>();
            dict.Add(ResourceType.Money, 0);
            dict.Add(ResourceType.Power, 0);
            return dict;
        }

        private void AddStartResourceLimits()
        {
            IncrementResourceLimit(ResourceType.Money, StartMoneyLimit);
            IncrementResourceLimit(ResourceType.Power, StartPowerLimit);
        }

        private void AddStartResources()
        {
            AddResource(ResourceType.Money, StartMoney);
            AddResource(ResourceType.Power, StartPower);
        }

        public void AddResource(ResourceType type, int amount)
        {
            _resources[type] += amount;
        }

        public void IncrementResourceLimit(ResourceType type, int amount)
        {
            _resourceLimits[type] += amount;
        }

        public void AddUnit(string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation)
        {
            Units units = GetComponentInChildren<Units>();
            GameObject newUnit = (GameObject)Instantiate(ResourceManager.GetUnit(unitName), spawnPoint, rotation);
            newUnit.transform.parent = units.transform;
            Unit unitObject = newUnit.GetComponent<Unit>();
            if (unitObject && (spawnPoint != rallyPoint)) unitObject.StartMove(rallyPoint);
        }
    }
}
