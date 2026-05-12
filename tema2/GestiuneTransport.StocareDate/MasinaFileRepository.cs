using System.Globalization;
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
                string.Join(
                    SEPARATOR,
                    Curata(m.NrInmatriculare),
                    (int)m.Marca,
                    Curata(m.Model),
                    m.AnFabricatie,
                    m.Kilometraj.ToString(CultureInfo.InvariantCulture),
                    (int)m.Culoare,
                    (int)m.Combustibil,
                    (int)m.Status,
                    (int)m.Optiuni));

            File.WriteAllLines(FILE_PATH, linii);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Eroare la salvarea masinilor in fisier: {ex.Message}");
        }
    }

    public List<Masina> IncarcaToti()
    {
        if (!File.Exists(FILE_PATH))
        {
            return new List<Masina>();
        }

        var masini = new List<Masina>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        for (int i = 0; i < linii.Length; i++)
        {
            try
            {
                string[] parts = linii[i].Split(SEPARATOR);

                if (parts.Length >= 9)
                {
                    masini.Add(CitesteFormatNou(parts));
                    continue;
                }

                if (parts.Length >= 5)
                {
                    masini.Add(CitesteFormatVechi(parts));
                    continue;
                }

                throw new FormatException("Numar insuficient de campuri.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Avertisment: Linia {i + 1} din {FILE_PATH} este malformata si a fost ignorata. ({ex.Message})");
            }
        }

        return masini;
    }

    private static Masina CitesteFormatNou(string[] parts)
    {
        return new Masina(
            parts[0],
            CitesteEnum(parts[1], MarcaMasina.Dacia),
            parts[2],
            CitesteInt(parts[3], DateTime.Now.Year),
            CitesteDouble(parts[4], 0),
            CitesteEnum(parts[5], Culoare.Alb),
            CitesteEnum(parts[6], CombustibilMasina.Diesel),
            CitesteEnum(parts[7], StatusMasina.Disponibila),
            CitesteEnum(parts[8], Optiuni.Niciuna));
    }

    private static Masina CitesteFormatVechi(string[] parts)
    {
        return new Masina(
            parts[0],
            MarcaMasina.Dacia,
            parts[1],
            DateTime.Now.Year,
            CitesteDouble(parts[2], 0),
            CitesteEnum(parts[3], Culoare.Alb),
            CombustibilMasina.Diesel,
            StatusMasina.Disponibila,
            CitesteEnum(parts[4], Optiuni.Niciuna));
    }

    private static string Curata(string text)
    {
        return text.Replace(SEPARATOR, ' ').Trim();
    }

    private static int CitesteInt(string text, int valoareImplicita)
    {
        return int.TryParse(text, out int valoare) ? valoare : valoareImplicita;
    }

    private static double CitesteDouble(string text, double valoareImplicita)
    {
        return double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out double valoare) ||
               double.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out valoare)
            ? valoare
            : valoareImplicita;
    }

    private static TEnum CitesteEnum<TEnum>(string text, TEnum valoareImplicita)
        where TEnum : struct, Enum
    {
        if (!int.TryParse(text, out int valoare))
        {
            return valoareImplicita;
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), valoare);
    }
}
