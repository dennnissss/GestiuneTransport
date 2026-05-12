namespace GestiuneTransport.Models;

public enum RolUtilizator
{
    Administrator,
    Operator
}

public class Utilizator
{
    public string Username { get; set; }
    public string ParolaHash { get; set; }
    public RolUtilizator Rol { get; set; }

    public Utilizator(string username, string parolaHash, RolUtilizator rol)
    {
        Username = username;
        ParolaHash = parolaHash;
        Rol = rol;
    }
}
