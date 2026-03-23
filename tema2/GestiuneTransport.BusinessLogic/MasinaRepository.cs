using GestiuneTransport.Models;

namespace GestiuneTransport.BusinessLogic;

public class MasinaRepository
{
    private readonly List<Masina> _masini = new();

    public void Adauga(Masina masina)
    {
        _masini.Add(masina);
    }

    public List<Masina> GetAll()
    {
        return _masini;
    }

    // LINQ — .FirstOrDefault() pentru cautare dupa nr. inmatriculare
    public Masina? CautaDupaNrInmatriculare(string nrInmatriculare)
    {
        return _masini
            .FirstOrDefault(m => m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));
    }

    // LINQ — .Where() cu lambda pentru cautare dupa culoare (returneaza List<Masina>)
    public List<Masina> CautaDupaCuloare(Culoare culoare)
    {
        return _masini
            .Where(m => m.Culoare == culoare)
            .ToList();
    }

    // LINQ — .Where() cu Flags bitwise check pentru filtrare dupa optiune
    public List<Masina> CautaDupaOptiune(Optiuni optiune)
    {
        return _masini
            .Where(m => m.Optiuni.HasFlag(optiune))
            .ToList();
    }

    // LINQ — .Where() + .OrderBy() pentru filtrare dupa kilometraj maxim
    public List<Masina> CautaDupaKilometrajMaxim(double kmMaxim)
    {
        return _masini
            .Where(m => m.Kilometraj <= kmMaxim)
            .OrderBy(m => m.Kilometraj)
            .ToList();
    }
}