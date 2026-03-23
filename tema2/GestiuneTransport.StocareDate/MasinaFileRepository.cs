using GestiuneTransport.Models;

namespace GestiuneTransport.StocareDate;

public class MasinaFileRepository
{
    private const string FILE_PATH = "masini.txt";
    private const char SEPARATOR = '|';

    public void SalveazaToti(List<Masina> masini)
    {
        try
        {
            var linii = masini.Select(m =>
                $"{m.NrInmatriculare}{SEPARATOR}{m.Model}{SEPARATOR}{m.Kilometraj}{SEPARATOR}{(int)m.Culoare}{SEPARATOR}{(int)m.Optiuni}");

            File.WriteAllLines(FILE_PATH, linii);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ Eroare la salvarea masinilor in fisier: {ex.Message}");
        }
    }

    public List<Masina> IncarcaToti()
    {
        if (!File.Exists(FILE_PATH))
            return new List<Masina>();

        var masini = new List<Masina>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        for (int i = 0; i < linii.Length; i++)
        {
            try
            {
                string[] parts = linii[i].Split(SEPARATOR);
                if (parts.Length < 5)
                    throw new FormatException("Numar insuficient de campuri.");

                string nrInmatriculare = parts[0];
                string model = parts[1];
                double kilometraj = double.Parse(parts[2]);
                Culoare culoare = (Culoare)int.Parse(parts[3]);
                Optiuni optiuni = (Optiuni)int.Parse(parts[4]);

                masini.Add(new Masina(nrInmatriculare, model, kilometraj, culoare, optiuni));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Avertisment: Linia {i + 1} din {FILE_PATH} este malformata si a fost ignorata. ({ex.Message})");
            }
        }

        return masini;
    }
}