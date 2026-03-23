using GestiuneTransport.Models;

namespace GestiuneTransport.StocareDate;

public class SoferFileRepository
{
    private const string FILE_PATH = "soferi.txt";
    private const char SEPARATOR = '|';

    public void SalveazaToti(List<Sofer> soferi)
    {
        try
        {
            var linii = soferi.Select(s =>
            {
                string trasee = s.IstoricTrasee.Count > 0
                    ? string.Join(",", s.IstoricTrasee)
                    : "";
                return $"{s.Id}{SEPARATOR}{s.Nume}{SEPARATOR}{s.TotalKilometriParcursi}{SEPARATOR}{trasee}";
            });

            File.WriteAllLines(FILE_PATH, linii);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ Eroare la salvarea soferilor in fisier: {ex.Message}");
        }
    }

    public List<Sofer> IncarcaToti()
    {
        if (!File.Exists(FILE_PATH))
            return new List<Sofer>();

        var soferi = new List<Sofer>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        for (int i = 0; i < linii.Length; i++)
        {
            try
            {
                string[] parts = linii[i].Split(SEPARATOR);
                if (parts.Length < 3)
                    throw new FormatException("Numar insuficient de campuri.");

                int id = int.Parse(parts[0]);
                string nume = parts[1];
                double km = double.Parse(parts[2]);

                Sofer sofer = new Sofer(id, nume)
                {
                    TotalKilometriParcursi = km
                };

                // Reconstituire istoric trasee (parts[3] poate lipsi sau fi gol)
                if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                {
                    sofer.IstoricTrasee = parts[3]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                soferi.Add(sofer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Avertisment: Linia {i + 1} din {FILE_PATH} este malformata si a fost ignorata. ({ex.Message})");
            }
        }

        return soferi;
    }
}