using GestiuneTransport.Models;
using GestiuneTransport.StocareDate;

namespace GestiuneTransport.BusinessLogic;

public class IntervalLucruRepository
{
    private readonly IntervalLucruFileRepository _fileRepo = new();
    private readonly SoferRepository _soferRepo;
    private readonly MasinaRepository _masinaRepo;
    private List<IntervalLucru> _intervale;

    public IntervalLucruRepository(SoferRepository soferRepo, MasinaRepository masinaRepo)
    {
        _soferRepo = soferRepo;
        _masinaRepo = masinaRepo;
        // Incarcare automata din fisier la initializare, cu referinte la entitatile existente
        _intervale = _fileRepo.IncarcaToti(_soferRepo.GetAll(), _masinaRepo.GetAll());
    }

    public List<IntervalLucru> GetAll()
    {
        return _intervale;
    }

    public void Adauga(IntervalLucru interval)
    {
        _intervale.Add(interval);
        _fileRepo.SalveazaToti(_intervale);
    }

    // Stergere dupa index (0-based) + persistare in fisier
    public bool Sterge(int index)
    {
        if (index < 0 || index >= _intervale.Count)
            return false;

        _intervale.RemoveAt(index);
        _fileRepo.SalveazaToti(_intervale);
        return true;
    }

    // LINQ — .Where() cu lambda pentru cautare intervale dupa numele soferului
    public List<IntervalLucru> CautaDupaSofer(string nume)
    {
        return _intervale
            .Where(i => i.SoferAlocat.Nume.Contains(nume, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // LINQ — .Where() cu lambda pentru cautare intervale dupa nr. inmatriculare masina
    public List<IntervalLucru> CautaDupaMasina(string nrInmatriculare)
    {
        return _intervale
            .Where(i => i.MasinaAlocata.NrInmatriculare.Contains(nrInmatriculare, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // LINQ — .Where() cu lambda pentru cautare intervale active la o anumita data
    public List<IntervalLucru> CautaDupaData(DateTime data)
    {
        return _intervale
            .Where(i => i.DataStart.Date <= data.Date && i.DataSfarsit.Date >= data.Date)
            .ToList();
    }
}