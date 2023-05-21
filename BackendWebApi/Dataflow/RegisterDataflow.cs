using System.ComponentModel.DataAnnotations;

namespace BackendWebApi.Dataflow
{
    public class RegisterDataflow
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}