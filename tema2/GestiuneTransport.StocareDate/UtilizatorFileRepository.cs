using System.Security.Cryptography;
using System.Text;
using GestiuneTransport.Models;

namespace GestiuneTransport.StocareDate;

public class UtilizatorFileRepository
{
    private const string FILE_PATH = "utilizatori.txt";
    private const char SEPARATOR = '|';
    private const string DefaultUsername = "admin";
    private const string DefaultPassword = "admin123";

    public UtilizatorFileRepository()
    {
        InitializeazaFisier();
    }

    public bool ValideazaAutentificare(string username, string parola)
    {
        string parolaHash = CalculeazaHash(parola);

        return IncarcaToti().Any(u =>
            u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) &&
            u.ParolaHash.Equals(parolaHash, StringComparison.Ordinal));
    }

    public List<Utilizator> IncarcaToti()
    {
        InitializeazaFisier();

        var utilizatori = new List<Utilizator>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        foreach (string linie in linii.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            string[] parts = linie.Split(SEPARATOR);
            if (parts.Length < 3)
            {
                continue;
            }

            RolUtilizator rol = Enum.TryParse(parts[2], out RolUtilizator rolCitit)
                ? rolCitit
                : RolUtilizator.Operator;

            utilizatori.Add(new Utilizator(parts[0], parts[1], rol));
        }

        return utilizatori;
    }

    private static void InitializeazaFisier()
    {
        if (File.Exists(FILE_PATH) && new FileInfo(FILE_PATH).Length > 0)
        {
            return;
        }

        string linieAdmin = string.Join(
            SEPARATOR,
            DefaultUsername,
            CalculeazaHash(DefaultPassword),
            RolUtilizator.Administrator);

        File.WriteAllText(FILE_PATH, linieAdmin + Environment.NewLine);
    }

    private static string CalculeazaHash(string parola)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(parola));
        return Convert.ToHexString(bytes);
    }
}
