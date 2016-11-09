namespace Assets.WorldObject.Building.Warfactory
{
    public class WarFactory : Building {
        protected override void Start () {
            base.Start();
            Operations = new string[] { "Tank" };
        }

        public override void PerformOperation(string actionToPerform)
        {
            base.PerformOperation(actionToPerform);
            CreateUnit(actionToPerform);
        }
    }
}   