using UnityEngine;

namespace Assets.WorldObject.Resource.OreDeposit
{
    public class OreDeposit : Resource {

        private int _numBlocks;

        protected override void Start () {
            base.Start();
            _numBlocks = GetComponentsInChildren< Ore >().Length;
            ResourceType = ResourceType.Ore;
        }

        protected override void Update () {
            base.Update();
            float percentLeft = (float)AmountLeft / (float)Capacity;
            if(percentLeft < 0) percentLeft = 0;
            int numBlocksToShow = (int)(percentLeft * _numBlocks);
            Ore[] blocks = GetComponentsInChildren< Ore >();
            if(numBlocksToShow >= 0 && numBlocksToShow < blocks.Length) {
                Ore[] sortedBlocks = new Ore[blocks.Length];
                //sort the list from highest to lowest
                foreach(Ore ore in blocks) {
                    sortedBlocks[blocks.Length - int.Parse(ore.name)] = ore;
                }
                for(int i = numBlocksToShow; i < sortedBlocks.Length; i++) {
                    sortedBlocks[i].GetComponent<Renderer>().enabled = false;
                }
                CalculateBounds();
            }
        }
    }
}