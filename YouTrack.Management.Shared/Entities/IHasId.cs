namespace YouTrack.Management.Shared.Entities
{
    public abstract class HasId
    {
        public string Id { get; set; }

        protected HasId(string id)
        {
            Id = id;
        }
    }

    public abstract class HasId<T>
    {
        public T Id { get; set; }

        public HasId(T id)
        {
            Id = id;
        }

        protected HasId()
        {
            
        }
    }
}