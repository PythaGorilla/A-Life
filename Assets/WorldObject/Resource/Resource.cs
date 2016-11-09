namespace Assets.WorldObject.Resource
{
    public class Resource : WorldObject
    {

        //Public variables
        public float Capacity;

        //Variables accessible by subclass
        protected float AmountLeft;
        protected ResourceType ResourceType;

        /*** Game Engine methods, all can be overridden by subclass ***/

        protected override void Start()
        {
            base.Start();
            AmountLeft = Capacity;
            ResourceType = ResourceType.Unknown;
        }

        /*** Public methods ***/

        public void Remove(float amount)
        {
            AmountLeft -= amount;
            if (AmountLeft < 0) AmountLeft = 0;
        }

        public bool IsEmpty()
        {
            return AmountLeft <= 0;
        }

        public ResourceType GetResourceType()
        {
            return ResourceType;
        }
    }
}