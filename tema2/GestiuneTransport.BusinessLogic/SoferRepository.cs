using GestiuneTransport.Models;

namespace GestiuneTransport.BusinessLogic;

public class SoferRepository
{
    private readonly List<Sofer> _soferi = new();

    public void Adauga(Sofer sofer)
    {
        _soferi.Add(sofer);
    }

    public List<Sofer> GetAll()
    {
        return _soferi;
    }

    // LINQ — .Where() cu lambda pentru cautare partiala dupa nume
    public List<Sofer> CautaDupaNume(string nume)
    {
        return _soferi
            .Where(s => s.Nume.Contains(nume, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // LINQ — .FirstOrDefault() cu lambda pentru cautare exacta dupa ID
    public Sofer? CautaDupaId(int id)
    {
        return _soferi
            .FirstOrDefault(s => s.Id == id);
    }

    // LINQ — .Where() + .OrderByDescending() pentru filtrare dupa km minim
    public List<Sofer> CautaDupaKilometriMinimi(double kmMinim)
    {
        return _soferi
            .Where(s => s.TotalKilometriParcursi >= kmMinim)
            .OrderByDescending(s => s.TotalKilometriParcursi)
            .ToList();
    }

    // LINQ — .Any() pentru verificare existenta
    public bool ExistaSofer(int id)
    {
        return _soferi.Any(s => s.Id == id);
    }
}