using System.Globalization;
using GestiuneTransport.Models;

namespace GestiuneTransport.StocareDate;

public class CursaFileRepository
{
    private const string FILE_PATH = "curse.txt";
    private const char SEPARATOR = '|';
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm";

    public void SalveazaToti(IEnumerable<Cursa> curse)
    {
        try
        {
            var linii = curse.Select(c =>
                $"{c.Id}{SEPARATOR}" +
                $"{CurataText(c.Client)}{SEPARATOR}" +
                $"{CurataText(c.Marfa)}{SEPARATOR}" +
                $"{CurataText(c.LocPlecare)}{SEPARATOR}" +
                $"{CurataText(c.Destinatie)}{SEPARATOR}" +
                $"{c.DataPlecare.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)}{SEPARATOR}" +
                $"{c.DataSosire.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)}{SEPARATOR}" +
                $"{c.SoferAlocat.Id}{SEPARATOR}" +
                $"{c.MasinaAlocata.NrInmatriculare}{SEPARATOR}" +
                $"{(int)c.Tip}{SEPARATOR}" +
                $"{(int)c.Status}{SEPARATOR}" +
                $"{(int)c.Prioritate}{SEPARATOR}" +
                $"{c.DistantaKm.ToString(CultureInfo.InvariantCulture)}{SEPARATOR}" +
                $"{c.PretPerKm.ToString(CultureInfo.InvariantCulture)}{SEPARATOR}" +
                $"{c.CostEstimativ.ToString(CultureInfo.InvariantCulture)}{SEPARATOR}" +
                $"{CurataText(c.Observatii)}");

            File.WriteAllLines(FILE_PATH, linii);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Eroare la salvarea curselor in fisier: {ex.Message}");
        }
    }

    public List<Cursa> IncarcaToti(List<Sofer> soferi, List<Masina> masini)
    {
        if (!File.Exists(FILE_PATH))
        {
            return new List<Cursa>();
        }

        var curse = new List<Cursa>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        for (int i = 0; i < linii.Length; i++)
        {
            try
            {
                string[] parts = linii[i].Split(SEPARATOR);
                Cursa cursa = parts.Length >= 16
                    ? CitesteFormatNou(parts, soferi, masini)
                    : CitesteFormatVechi(parts, soferi, masini);

                curse.Add(cursa);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Linia {i + 1} din {FILE_PATH} a fost ignorata: {ex.Message}");
            }
        }

        return curse;
    }

    private static Cursa CitesteFormatNou(string[] parts, List<Sofer> soferi, List<Masina> masini)
    {
        int soferId = int.Parse(parts[7], CultureInfo.InvariantCulture);
        string nrInmatriculare = parts[8];
        Sofer sofer = soferi.First(s => s.Id == soferId);
        Masina masina = masini.First(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));

        return new Cursa(
            int.Parse(parts[0], CultureInfo.InvariantCulture),
            parts[1],
            parts[2],
            parts[3],
            parts[4],
            DateTime.ParseExact(parts[5], DATE_FORMAT, CultureInfo.InvariantCulture),
            DateTime.ParseExact(parts[6], DATE_FORMAT, CultureInfo.InvariantCulture),
            masina,
            sofer,
            (TipCursa)int.Parse(parts[9], CultureInfo.InvariantCulture),
            (StatusCursa)int.Parse(parts[10], CultureInfo.InvariantCulture),
            (PrioritateCursa)int.Parse(parts[11], CultureInfo.InvariantCulture),
            double.Parse(parts[12], CultureInfo.InvariantCulture),
            decimal.Parse(parts[13], CultureInfo.InvariantCulture),
            decimal.Parse(parts[14], CultureInfo.InvariantCulture),
            parts[15]);
    }

    private static Cursa CitesteFormatVechi(string[] parts, List<Sofer> soferi, List<Masina> masini)
    {
        if (parts.Length < 11)
        {
            throw new FormatException("Numar insuficient de campuri.");
        }

        int soferId = int.Parse(parts[5], CultureInfo.InvariantCulture);
        string nrInmatriculare = parts[6];
        Sofer sofer = soferi.First(s => s.Id == soferId);
        Masina masina = masini.First(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));

        double distanta = double.Parse(parts[9], CultureInfo.InvariantCulture);
        decimal cost = decimal.Parse(parts[10], CultureInfo.InvariantCulture);

        return new Cursa(
            int.Parse(parts[0], CultureInfo.InvariantCulture),
            parts[1],
            parts[2],
            DateTime.ParseExact(parts[3], DATE_FORMAT, CultureInfo.InvariantCulture),
            DateTime.ParseExact(parts[4], DATE_FORMAT, CultureInfo.InvariantCulture),
            masina,
            sofer,
            (TipCursa)int.Parse(parts[7], CultureInfo.InvariantCulture),
            (StatusCursa)int.Parse(parts[8], CultureInfo.InvariantCulture),
            distanta,
            cost);
    }

    private static string CurataText(string text)
    {
        return text.Replace(SEPARATOR, '/').Trim();
    }
}
