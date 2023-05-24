using System.ComponentModel.DataAnnotations;

namespace BackendWebApi.Dataflow
{
    public class RegisterDataflow
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}