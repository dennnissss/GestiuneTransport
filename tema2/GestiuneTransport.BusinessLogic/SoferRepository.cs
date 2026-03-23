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

    // LINQ — .FirstOrDefault() pentru actualizare + persistare in fisier
    public bool ActualizeazaSofer(int id, string numeNou)
    {
        var sofer = _soferi.FirstOrDefault(s => s.Id == id);
        if (sofer is null)
            return false;

        sofer.Nume = numeNou;
        _fileRepo.SalveazaToti(_soferi);
        return true;
    }

    // LINQ — .FirstOrDefault() pentru stergere + persistare in fisier
    public bool StergeSofer(int id)
    {
        var sofer = _soferi.FirstOrDefault(s => s.Id == id);
        if (sofer is null)
            return false;

        _soferi.Remove(sofer);
        _fileRepo.SalveazaToti(_soferi);
        return true;
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