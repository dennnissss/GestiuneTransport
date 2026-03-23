using GestiuneTransport.Models;
using System.Globalization;

namespace GestiuneTransport.StocareDate;

public class IntervalLucruFileRepository
{
    private const string FILE_PATH = "intervale.txt";
    private const char SEPARATOR = '|';
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm";

    public void SalveazaToti(List<IntervalLucru> intervale)
    {
        try
        {
            var linii = intervale.Select(i =>
                $"{i.SoferAlocat.Id}{SEPARATOR}" +
                $"{i.SoferAlocat.Nume}{SEPARATOR}" +
                $"{i.SoferAlocat.TotalKilometriParcursi}{SEPARATOR}" +
                $"{i.MasinaAlocata.NrInmatriculare}{SEPARATOR}" +
                $"{i.MasinaAlocata.Model}{SEPARATOR}" +
                $"{i.MasinaAlocata.Kilometraj}{SEPARATOR}" +
                $"{(int)i.MasinaAlocata.Culoare}{SEPARATOR}" +
                $"{(int)i.MasinaAlocata.Optiuni}{SEPARATOR}" +
                $"{i.DataStart.ToString(DATE_FORMAT)}{SEPARATOR}" +
                $"{i.DataSfarsit.ToString(DATE_FORMAT)}");

            File.WriteAllLines(FILE_PATH, linii);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ Eroare la salvarea intervalelor in fisier: {ex.Message}");
        }
    }

    public List<IntervalLucru> IncarcaToti(List<Sofer> soferi, List<Masina> masini)
    {
        if (!File.Exists(FILE_PATH))
            return new List<IntervalLucru>();

        var intervale = new List<IntervalLucru>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        for (int i = 0; i < linii.Length; i++)
        {
            try
            {
                string[] parts = linii[i].Split(SEPARATOR);
                if (parts.Length < 10)
                    throw new FormatException("Numar insuficient de campuri.");

                // Reconstituire Sofer — cautare in lista existenta prin LINQ
                int soferId = int.Parse(parts[0]);
                var sofer = soferi.FirstOrDefault(s => s.Id == soferId);
                if (sofer is null)
                {
                    Console.WriteLine($"⚠ Avertisment: Linia {i + 1} din {FILE_PATH} — soferul cu ID {soferId} nu exista in lista. Linia a fost ignorata.");
                    continue;
                }

                // Reconstituire Masina — cautare in lista existenta prin LINQ
                string nrInmatriculare = parts[3];
                var masina = masini.FirstOrDefault(m => m.NrInmatriculare == nrInmatriculare);
                if (masina is null)
                {
                    Console.WriteLine($"⚠ Avertisment: Linia {i + 1} din {FILE_PATH} — masina cu NR {nrInmatriculare} nu exista in lista. Linia a fost ignorata.");
                    continue;
                }

                // Reconstituire date
                DateTime dataStart = DateTime.ParseExact(parts[8], DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime dataSfarsit = DateTime.ParseExact(parts[9], DATE_FORMAT, CultureInfo.InvariantCulture);

                intervale.Add(new IntervalLucru(sofer, masina, dataStart, dataSfarsit));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Avertisment: Linia {i + 1} din {FILE_PATH} este malformata si a fost ignorata. ({ex.Message})");
            }
        }

        return intervale;
    }
}