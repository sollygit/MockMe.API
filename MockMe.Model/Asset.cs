namespace MockMe.Model
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Asset()
        {
        }

        public Asset(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
