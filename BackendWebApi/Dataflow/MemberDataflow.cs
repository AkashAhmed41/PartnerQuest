namespace BackendWebApi.Dataflow
{
    public class MemberDataflow
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public int Age { get; set; }
        public string Nickname { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string About { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<PhotoDataflow> Photos { get; set; }
    }
}