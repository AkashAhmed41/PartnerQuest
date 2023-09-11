using System.ComponentModel.DataAnnotations;

namespace BackendWebApi.Models;

public class SignalRGroup
{
    public SignalRGroup()
    {

    }

    public SignalRGroup(string name)
    {
        Name = name;
    }

    [Key]
    public string Name { get; set; }
    public ICollection<SignalRConnection> Connections { get; set; } = new List<SignalRConnection>();
}