using System.Globalization;
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
                    : string.Empty;

                return string.Join(
                    SEPARATOR,
                    s.Id,
                    Curata(s.Nume),
                    Curata(s.Telefon),
                    s.TotalKilometriParcursi.ToString(CultureInfo.InvariantCulture),
                    (int)s.CategoriePermis,
                    (int)s.Status,
                    Curata(trasee));
            });

            File.WriteAllLines(FILE_PATH, linii);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Eroare la salvarea soferilor in fisier: {ex.Message}");
        }
    }

    public List<Sofer> IncarcaToti()
    {
        if (!File.Exists(FILE_PATH))
        {
            return new List<Sofer>();
        }

        var soferi = new List<Sofer>();
        string[] linii = File.ReadAllLines(FILE_PATH);

        for (int i = 0; i < linii.Length; i++)
        {
            try
            {
                string[] parts = linii[i].Split(SEPARATOR);

                if (parts.Length >= 7)
                {
                    soferi.Add(CitesteFormatNou(parts));
                    continue;
                }

                if (parts.Length >= 3)
                {
                    soferi.Add(CitesteFormatVechi(parts));
                    continue;
                }

                throw new FormatException("Numar insuficient de campuri.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Avertisment: Linia {i + 1} din {FILE_PATH} este malformata si a fost ignorata. ({ex.Message})");
            }
        }

        return soferi;
    }

    private static Sofer CitesteFormatNou(string[] parts)
    {
        var sofer = new Sofer(
            CitesteInt(parts[0], 0),
            parts[1],
            parts[2],
            CitesteEnum(parts[4], CategoriePermis.B),
            CitesteEnum(parts[5], StatusSofer.Disponibil))
        {
            TotalKilometriParcursi = CitesteDouble(parts[3], 0)
        };

        CitesteTrasee(sofer, parts[6]);
        return sofer;
    }

    private static Sofer CitesteFormatVechi(string[] parts)
    {
        var sofer = new Sofer(CitesteInt(parts[0], 0), parts[1])
        {
            TotalKilometriParcursi = CitesteDouble(parts[2], 0)
        };

        if (parts.Length > 3)
        {
            CitesteTrasee(sofer, parts[3]);
        }

        return sofer;
    }

    private static void CitesteTrasee(Sofer sofer, string trasee)
    {
        if (string.IsNullOrWhiteSpace(trasee))
        {
            return;
        }

        sofer.IstoricTrasee = trasee
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
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
