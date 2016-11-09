using UnityEngine;

/**
 * Singleton that handles loading level details. This includes making sure
 * that all world objects have an objectId set.
 */

namespace Assets.Resources
{
    public class LevelLoader : MonoBehaviour {

        private static int _nextObjectId = 0;
        private static bool _created = false;
        private bool _initialised = false;

        void Awake() {
            if(!_created) {
                DontDestroyOnLoad(transform.gameObject);
                _created = true;
                _initialised = true;
            } else {
                Destroy(this.gameObject);
            }
        }

        void OnLevelWasLoaded() {
            if(_initialised) {
                WorldObject.WorldObject[] worldObjects = GameObject.FindObjectsOfType(typeof(WorldObject.WorldObject)) as WorldObject.WorldObject[];
                foreach(WorldObject.WorldObject worldObject in worldObjects) {
                    worldObject.ObjectId = _nextObjectId++;
                    if(_nextObjectId >= int.MaxValue) _nextObjectId = 0;
                }
            }
        }
    }
}