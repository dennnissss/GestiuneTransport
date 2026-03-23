using GestiuneTransport.Models;
using GestiuneTransport.StocareDate;

namespace GestiuneTransport.BusinessLogic;

public class SoferRepository
{
    private readonly SoferFileRepository _fileRepo = new();
    private List<Sofer> _soferi;

    public SoferRepository()
    {
        // Incarcare automata din fisier la initializare
        _soferi = _fileRepo.IncarcaToti();
    }

    public void Adauga(Sofer sofer)
    {
        _soferi.Add(sofer);
        _fileRepo.SalveazaToti(_soferi);
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