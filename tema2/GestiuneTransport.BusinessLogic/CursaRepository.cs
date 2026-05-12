using GestiuneTransport.Models;
using GestiuneTransport.StocareDate;

namespace GestiuneTransport.BusinessLogic;

public class CursaRepository
{
    private readonly CursaFileRepository _fileRepo = new();
    private readonly List<Cursa> _curse;

    public CursaRepository(List<Sofer> soferi, List<Masina> masini)
    {
        _curse = _fileRepo.IncarcaToti(soferi, masini);
    }

    public List<Cursa> GetAll()
    {
        return _curse.ToList();
    }

    public void Adauga(Cursa cursa)
    {
        _curse.Add(cursa);
        _fileRepo.SalveazaToti(_curse);
    }

    public bool Actualizeaza(int id, Cursa cursaActualizata)
    {
        Cursa? cursa = _curse.FirstOrDefault(c => c.Id == id);
        if (cursa == null)
        {
            return false;
        }

        cursa.LocPlecare = cursaActualizata.LocPlecare;
        cursa.Client = cursaActualizata.Client;
        cursa.Marfa = cursaActualizata.Marfa;
        cursa.Destinatie = cursaActualizata.Destinatie;
        cursa.DataPlecare = cursaActualizata.DataPlecare;
        cursa.DataSosire = cursaActualizata.DataSosire;
        cursa.MasinaAlocata = cursaActualizata.MasinaAlocata;
        cursa.SoferAlocat = cursaActualizata.SoferAlocat;
        cursa.Tip = cursaActualizata.Tip;
        cursa.Status = cursaActualizata.Status;
        cursa.Prioritate = cursaActualizata.Prioritate;
        cursa.DistantaKm = cursaActualizata.DistantaKm;
        cursa.PretPerKm = cursaActualizata.PretPerKm;
        cursa.CostEstimativ = cursaActualizata.CostEstimativ;
        cursa.Observatii = cursaActualizata.Observatii;

        _fileRepo.SalveazaToti(_curse);
        return true;
    }

    public bool Sterge(int id)
    {
        Cursa? cursa = _curse.FirstOrDefault(c => c.Id == id);
        if (cursa == null)
        {
            return false;
        }

        _curse.Remove(cursa);
        _fileRepo.SalveazaToti(_curse);
        return true;
    }

    public List<Cursa> CautaDupaRuta(string text)
    {
        return _curse
            .Where(c =>
                c.LocPlecare.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                c.Destinatie.Contains(text, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Cursa> CautaDupaStatus(StatusCursa status)
    {
        return _curse
            .Where(c => c.Status == status)
            .ToList();
    }
}
