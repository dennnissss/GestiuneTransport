using GestiuneTransport.BusinessLogic;
using GestiuneTransport.Models;
using System;
using System.Globalization;
using System.Linq;

namespace GestiuneTransport.ConsoleApp;

class Program
{
    static readonly SoferRepository _soferRepository = new();
    static readonly MasinaRepository _masinaRepository = new();
    static readonly IntervalLucruRepository _intervalRepository =
        new IntervalLucruRepository(_soferRepository, _masinaRepository);

    static void Main(string[] args)
    {
        string optiune;
        do
        {
            Console.WriteLine("\n--- SISTEM GESTIUNE TRANSPORT (Enterprise v1.1) ---");
            Console.WriteLine("1.  Adauga sofer");
            Console.WriteLine("2.  Afiseaza toti soferii");
            Console.WriteLine("3.  Cauta sofer dupa nume (LINQ .Where)");
            Console.WriteLine("4.  Cauta soferi dupa km minimi (LINQ .Where + .OrderByDescending)");
            Console.WriteLine("5.  Adauga masina (Enumerari & Flags)");
            Console.WriteLine("6.  Afiseaza toate masinile");
            Console.WriteLine("7.  Cauta masini dupa culoare (LINQ .Where)");
            Console.WriteLine("8.  Cauta masini dupa optiune (LINQ .Where + HasFlag)");
            Console.WriteLine("9.  Modifica sofer (nume)");
            Console.WriteLine("10. Sterge sofer");
            Console.WriteLine("11. Modifica masina (model si kilometraj)");
            Console.WriteLine("12. Sterge masina");
            Console.WriteLine("13. Adauga interval lucru");
            Console.WriteLine("14. Afiseaza toate intervalele");
            Console.WriteLine("15. Cauta intervale dupa sofer");
            Console.WriteLine("16. Cauta intervale dupa masina");
            Console.WriteLine("17. Cauta intervale dupa data");
            Console.WriteLine("18. Sterge interval lucru");
            Console.WriteLine("X.  Iesire");
            Console.Write("Alege optiunea: ");
            optiune = Console.ReadLine()?.ToUpper() ?? "";

            switch (optiune)
            {
                case "1": AdaugaSofer(); break;
                case "2": AfiseazaSoferi(); break;
                case "3": CautaSofer(); break;
                case "4": CautaSoferiDupaKm(); break;
                case "5": AdaugaMasina(); break;
                case "6": AfiseazaMasini(); break;
                case "7": CautaMasiniDupaCuloare(); break;
                case "8": CautaMasiniDupaOptiune(); break;
                case "9": ActualizeazaSofer(); break;
                case "10": StergeSofer(); break;
                case "11": ActualizeazaMasina(); break;
                case "12": StergeMasina(); break;
                case "13": AdaugaInterval(); break;
                case "14": AfiseazaIntervale(); break;
                case "15": CautaIntervalDupaSofer(); break;
                case "16": CautaIntervalDupaMasina(); break;
                case "17": CautaIntervalDupaData(); break;
                case "18": StergeInterval(); break;
            }
        } while (optiune != "X");
    }

    // ── SOFERI ──────────────────────────────────────────────

    static void AdaugaSofer()
    {
        try
        {
            Console.Write("Introduceti ID-ul: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
                throw new FormatException("ID-ul trebuie sa fie un numar intreg!");

            if (_soferRepository.ExistaSofer(id))
                throw new InvalidOperationException($"Exista deja un sofer cu ID-ul {id}!");

            Console.Write("Introduceti numele soferului: ");
            string nume = Console.ReadLine() ?? "";

            Sofer s = new Sofer(id, nume);
            _soferRepository.Adauga(s);
            Console.WriteLine("✔ Sofer adaugat cu succes!");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"❌ Eroare logica: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare proces adaugare sofer.");
        }
    }

    static void AfiseazaSoferi()
    {
        Console.WriteLine("\n--- LISTA SOFERI ---");
        var soferi = _soferRepository.GetAll();

        if (!soferi.Any())
        {
            Console.WriteLine("Nu exista soferi inregistrati.");
            return;
        }

        foreach (var s in soferi)
            Console.WriteLine($"ID: {s.Id} | Nume: {s.Nume} | KM: {s.TotalKilometriParcursi}");
    }

    static void CautaSofer()
    {
        try
        {
            Console.Write("Introduceti numele sau o parte din nume: ");
            string numeCautat = Console.ReadLine() ?? "";

            // LINQ .Where() — apelat in repository
            var rezultat = _soferRepository.CautaDupaNume(numeCautat);

            if (rezultat.Any())
            {
                Console.WriteLine($"S-au gasit {rezultat.Count} rezultate:");
                rezultat.ForEach(s => Console.WriteLine($"  [GASIT] ID: {s.Id} | Nume: {s.Nume}"));
            }
            else
            {
                Console.WriteLine("Nu a fost gasit niciun sofer care sa corespunda cautarii.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare: {ex.Message}");
        }
    }

    static void CautaSoferiDupaKm()
    {
        try
        {
            Console.Write("Introduceti numarul minim de kilometri: ");
            if (!double.TryParse(Console.ReadLine(), out double kmMinim))
                throw new FormatException("Kilometrii trebuie sa fie un numar valid!");

            // LINQ .Where() + .OrderByDescending() — apelat in repository
            var rezultat = _soferRepository.CautaDupaKilometriMinimi(kmMinim);

            if (rezultat.Any())
            {
                Console.WriteLine($"Soferi cu >= {kmMinim} km (ordonati descrescator):");
                rezultat.ForEach(s =>
                    Console.WriteLine($"  ID: {s.Id} | Nume: {s.Nume} | KM: {s.TotalKilometriParcursi}"));
            }
            else
            {
                Console.WriteLine("Niciun sofer nu indeplineste criteriul.");
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare cautare soferi dupa km.");
        }
    }

    static void ActualizeazaSofer()
    {
        try
        {
            Console.Write("Introduceti ID-ul soferului de modificat: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
                throw new FormatException("ID-ul trebuie sa fie un numar intreg!");

            Console.Write("Introduceti noul nume: ");
            string numeNou = Console.ReadLine() ?? "";

            bool rezultat = _soferRepository.ActualizeazaSofer(id, numeNou);

            if (rezultat)
                Console.WriteLine("✔ Sofer actualizat si salvat in fisier!");
            else
                Console.WriteLine("❌ Soferul nu a fost gasit.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare actualizare sofer.");
        }
    }

    static void StergeSofer()
    {
        try
        {
            Console.Write("Introduceti ID-ul soferului de sters: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
                throw new FormatException("ID-ul trebuie sa fie un numar intreg!");

            bool rezultat = _soferRepository.StergeSofer(id);

            if (rezultat)
                Console.WriteLine("✔ Sofer sters si fisier actualizat!");
            else
                Console.WriteLine("❌ Soferul nu a fost gasit.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare stergere sofer.");
        }
    }

    // ── MASINI ──────────────────────────────────────────────

    static void AdaugaMasina()
    {
        try
        {
            Console.Write("Introduceti numarul de inmatriculare: ");
            string nrInmatriculare = Console.ReadLine() ?? "";

            Console.Write("Introduceti modelul: ");
            string model = Console.ReadLine() ?? "";

            Console.Write("Introduceti kilometrajul initial: ");
            if (!double.TryParse(Console.ReadLine(), out double km))
                throw new FormatException("Kilometrajul trebuie sa fie un numar valid!");

            // Citire Culoare (enum simplu)
            Console.WriteLine("Alegeti culoarea:");
            var culori = Enum.GetValues<Culoare>();
            for (int i = 0; i < culori.Length; i++)
                Console.WriteLine($"  {i}. {culori[i]}");

            Console.Write("Culoare (numar): ");
            if (!int.TryParse(Console.ReadLine(), out int culoareIndex) ||
                !Enum.IsDefined(typeof(Culoare), culoareIndex))
                throw new FormatException("Culoare invalida!");

            Culoare culoare = (Culoare)culoareIndex;

            // Citire Optiuni ([Flags] enum — selectie multipla)
            Console.WriteLine("Alegeti optiunile (introduceti numerele separate prin virgula, sau 0 pentru niciuna):");
            var toateOptiunile = Enum.GetValues<Optiuni>();
            foreach (var opt in toateOptiunile)
                Console.WriteLine($"  {(int)opt}. {opt}");

            Console.Write("Optiuni: ");
            string optiuniInput = Console.ReadLine() ?? "0";

            Optiuni optiuniSelectate = Optiuni.Niciuna;
            foreach (string parte in optiuniInput.Split(',', StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(parte, out int val) && Enum.IsDefined(typeof(Optiuni), val))
                    optiuniSelectate |= (Optiuni)val;
            }

            Masina masina = new Masina(nrInmatriculare, model, km, culoare, optiuniSelectate);
            _masinaRepository.Adauga(masina);

            Console.WriteLine("✔ Masina adaugata cu succes!");
            Console.WriteLine($"  → {masina}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare proces adaugare masina.");
        }
    }

    static void AfiseazaMasini()
    {
        Console.WriteLine("\n--- LISTA MASINI ---");
        var masini = _masinaRepository.GetAll();

        if (!masini.Any())
        {
            Console.WriteLine("Nu exista masini inregistrate.");
            return;
        }

        foreach (var m in masini)
            Console.WriteLine(m);
    }

    static void CautaMasiniDupaCuloare()
    {
        try
        {
            Console.WriteLine("Alegeti culoarea cautata:");
            var culori = Enum.GetValues<Culoare>();
            for (int i = 0; i < culori.Length; i++)
                Console.WriteLine($"  {i}. {culori[i]}");

            Console.Write("Culoare (numar): ");
            if (!int.TryParse(Console.ReadLine(), out int culoareIndex) ||
                !Enum.IsDefined(typeof(Culoare), culoareIndex))
                throw new FormatException("Culoare invalida!");

            // LINQ .Where() — apelat in repository
            var rezultat = _masinaRepository.CautaDupaCuloare((Culoare)culoareIndex);

            if (rezultat.Any())
            {
                Console.WriteLine($"Masini gasite cu culoarea {(Culoare)culoareIndex}:");
                rezultat.ForEach(m => Console.WriteLine($"  {m}"));
            }
            else
            {
                Console.WriteLine("Nu exista masini cu aceasta culoare.");
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare cautare masini dupa culoare.");
        }
    }

    static void CautaMasiniDupaOptiune()
    {
        try
        {
            Console.WriteLine("Alegeti optiunea cautata:");
            var optiuni = Enum.GetValues<Optiuni>().Where(o => o != Optiuni.Niciuna).ToArray();
            foreach (var opt in optiuni)
                Console.WriteLine($"  {(int)opt}. {opt}");

            Console.Write("Optiune (numar): ");
            if (!int.TryParse(Console.ReadLine(), out int optVal) ||
                !Enum.IsDefined(typeof(Optiuni), optVal) ||
                optVal == 0)
                throw new FormatException("Optiune invalida!");

            // LINQ .Where() + .HasFlag() — apelat in repository
            var rezultat = _masinaRepository.CautaDupaOptiune((Optiuni)optVal);

            if (rezultat.Any())
            {
                Console.WriteLine($"Masini care au optiunea {(Optiuni)optVal}:");
                rezultat.ForEach(m => Console.WriteLine($"  {m}"));
            }
            else
            {
                Console.WriteLine("Nu exista masini cu aceasta optiune.");
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare cautare masini dupa optiune.");
        }
    }

    static void ActualizeazaMasina()
    {
        try
        {
            Console.Write("Introduceti numarul de inmatriculare al masinii de modificat: ");
            string nrInmatriculare = Console.ReadLine() ?? "";

            Console.Write("Introduceti noul model: ");
            string modelNou = Console.ReadLine() ?? "";

            Console.Write("Introduceti noul kilometraj: ");
            if (!double.TryParse(Console.ReadLine(), out double kmNou))
                throw new FormatException("Kilometrajul trebuie sa fie un numar valid!");

            bool rezultat = _masinaRepository.ActualizeazaMasina(nrInmatriculare, modelNou, kmNou);

            if (rezultat)
                Console.WriteLine("✔ Masina actualizata si salvata in fisier!");
            else
                Console.WriteLine("❌ Masina nu a fost gasita.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare actualizare masina.");
        }
    }

    static void StergeMasina()
    {
        try
        {
            Console.Write("Introduceti numarul de inmatriculare al masinii de sters: ");
            string nrInmatriculare = Console.ReadLine() ?? "";

            bool rezultat = _masinaRepository.StergeMasina(nrInmatriculare);

            if (rezultat)
                Console.WriteLine("✔ Masina stearsa si fisier actualizat!");
            else
                Console.WriteLine("❌ Masina nu a fost gasita.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare stergere masina.");
        }
    }

    // ── INTERVALE LUCRU ─────────────────────────────────────

    static void AdaugaInterval()
    {
        try
        {
            // Cautare sofer prin LINQ .FirstOrDefault()
            Console.Write("Introduceti ID-ul soferului: ");
            if (!int.TryParse(Console.ReadLine(), out int soferId))
                throw new FormatException("ID-ul trebuie sa fie un numar intreg!");

            var sofer = _soferRepository.GetAll()
                .FirstOrDefault(s => s.Id == soferId);
            if (sofer is null)
            {
                Console.WriteLine($"❌ Nu exista un sofer cu ID-ul {soferId}.");
                return;
            }

            // Cautare masina prin LINQ .FirstOrDefault()
            Console.Write("Introduceti numarul de inmatriculare al masinii: ");
            string nrInmatriculare = Console.ReadLine() ?? "";

            var masina = _masinaRepository.GetAll()
                .FirstOrDefault(m => m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));
            if (masina is null)
            {
                Console.WriteLine($"❌ Nu exista o masina cu NR {nrInmatriculare}.");
                return;
            }

            Console.Write("Introduceti data de inceput (format: yyyy-MM-dd HH:mm): ");
            string startInput = Console.ReadLine() ?? "";
            DateTime dataStart = DateTime.ParseExact(startInput, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            Console.Write("Introduceti data de sfarsit (format: yyyy-MM-dd HH:mm): ");
            string sfarsitInput = Console.ReadLine() ?? "";
            DateTime dataSfarsit = DateTime.ParseExact(sfarsitInput, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            if (dataSfarsit <= dataStart)
                throw new InvalidOperationException("Data de sfarsit trebuie sa fie dupa data de inceput!");

            IntervalLucru interval = new IntervalLucru(sofer, masina, dataStart, dataSfarsit);
            _intervalRepository.Adauga(interval);

            Console.WriteLine("✔ Interval adaugat si salvat in fisier!");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"❌ Eroare logica: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare adaugare interval.");
        }
    }

    static void AfiseazaIntervale()
    {
        Console.WriteLine("\n--- LISTA INTERVALE LUCRU ---");
        var intervale = _intervalRepository.GetAll();

        if (!intervale.Any())
        {
            Console.WriteLine("Nu exista intervale inregistrate.");
            return;
        }

        for (int i = 0; i < intervale.Count; i++)
        {
            var il = intervale[i];
            Console.WriteLine(
                $"[{i + 1}]. Sofer: {il.SoferAlocat.Nume} | " +
                $"Masina: {il.MasinaAlocata.Model} [{il.MasinaAlocata.NrInmatriculare}] | " +
                $"De la: {il.DataStart:yyyy-MM-dd HH:mm} Pana la: {il.DataSfarsit:yyyy-MM-dd HH:mm}");
        }
    }

    static void CautaIntervalDupaSofer()
    {
        try
        {
            Console.Write("Introduceti numele soferului (sau o parte): ");
            string nume = Console.ReadLine() ?? "";

            // LINQ .Where() — apelat in repository
            var rezultat = _intervalRepository.CautaDupaSofer(nume);

            if (rezultat.Any())
            {
                Console.WriteLine($"S-au gasit {rezultat.Count} intervale:");
                rezultat.ForEach(il => Console.WriteLine(
                    $"  Sofer: {il.SoferAlocat.Nume} | Masina: {il.MasinaAlocata.Model} [{il.MasinaAlocata.NrInmatriculare}] | " +
                    $"De la: {il.DataStart:yyyy-MM-dd HH:mm} Pana la: {il.DataSfarsit:yyyy-MM-dd HH:mm}"));
            }
            else
            {
                Console.WriteLine("Nu au fost gasite intervale.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare cautare dupa sofer.");
        }
    }

    static void CautaIntervalDupaMasina()
    {
        try
        {
            Console.Write("Introduceti numarul de inmatriculare (sau o parte): ");
            string nrInmatriculare = Console.ReadLine() ?? "";

            // LINQ .Where() — apelat in repository
            var rezultat = _intervalRepository.CautaDupaMasina(nrInmatriculare);

            if (rezultat.Any())
            {
                Console.WriteLine($"S-au gasit {rezultat.Count} intervale:");
                rezultat.ForEach(il => Console.WriteLine(
                    $"  Sofer: {il.SoferAlocat.Nume} | Masina: {il.MasinaAlocata.Model} [{il.MasinaAlocata.NrInmatriculare}] | " +
                    $"De la: {il.DataStart:yyyy-MM-dd HH:mm} Pana la: {il.DataSfarsit:yyyy-MM-dd HH:mm}"));
            }
            else
            {
                Console.WriteLine("Nu au fost gasite intervale.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare cautare dupa masina.");
        }
    }

    static void CautaIntervalDupaData()
    {
        try
        {
            Console.Write("Introduceti data cautata (format: yyyy-MM-dd): ");
            string dataInput = Console.ReadLine() ?? "";
            DateTime data = DateTime.ParseExact(dataInput, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // LINQ .Where() — apelat in repository
            var rezultat = _intervalRepository.CautaDupaData(data);

            if (rezultat.Any())
            {
                Console.WriteLine($"Intervale active la data {data:yyyy-MM-dd}:");
                rezultat.ForEach(il => Console.WriteLine(
                    $"  Sofer: {il.SoferAlocat.Nume} | Masina: {il.MasinaAlocata.Model} [{il.MasinaAlocata.NrInmatriculare}] | " +
                    $"De la: {il.DataStart:yyyy-MM-dd HH:mm} Pana la: {il.DataSfarsit:yyyy-MM-dd HH:mm}"));
            }
            else
            {
                Console.WriteLine("Nu au fost gasite intervale pentru aceasta data.");
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare cautare dupa data.");
        }
    }

    static void StergeInterval()
    {
        try
        {
            AfiseazaIntervale();

            var intervale = _intervalRepository.GetAll();
            if (!intervale.Any())
                return;

            Console.Write("Introduceti numarul intervalului de sters (1-based): ");
            if (!int.TryParse(Console.ReadLine(), out int index))
                throw new FormatException("Indexul trebuie sa fie un numar intreg!");

            bool rezultat = _intervalRepository.Sterge(index - 1);

            if (rezultat)
                Console.WriteLine("✔ Interval sters si fisier actualizat!");
            else
                Console.WriteLine("❌ Index invalid.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Eroare de format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Eroare neasteptata: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Finalizare stergere interval.");
        }
    }
}