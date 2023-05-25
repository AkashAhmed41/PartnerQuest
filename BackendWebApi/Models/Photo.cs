using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWebApi.Models
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsProfilePhoto { get; set; }
        public string PublicId { get; set; }

        public int UserId { get; set; }     //Required foreign key property
        public User User { get; set; }      //Required reference navigation to parent
    }
}