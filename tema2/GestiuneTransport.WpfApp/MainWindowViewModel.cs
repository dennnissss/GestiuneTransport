using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GestiuneTransport.BusinessLogic;
using GestiuneTransport.Models;
using GestiuneTransport.StocareDate;

namespace GestiuneTransport.WpfApp;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MasinaRepository _masinaRepository = new();
    private readonly MasinaFileRepository _masinaFileRepository = new();
    private readonly SoferRepository _soferRepository = new();
    private readonly SoferFileRepository _soferFileRepository = new();
    private readonly List<Masina> _toateMasinile = new();
    private readonly List<Sofer> _totiSoferii = new();

    public ObservableCollection<Masina> Masini { get; } = new();
    public ObservableCollection<Sofer> Soferi { get; } = new();

    public string Rezumat => Masini.Count == 1
        ? "1 masina afisata in lista."
        : $"{Masini.Count} masini afisate in lista.";

    public string RezumatSoferi => Soferi.Count == 1
        ? "1 sofer afisat in lista."
        : $"{Soferi.Count} soferi afisati in lista.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindowViewModel()
    {
        Masini.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Rezumat));
        Soferi.CollectionChanged += (_, _) => OnPropertyChanged(nameof(RezumatSoferi));

        _toateMasinile.AddRange(_masinaRepository.GetAll());
        ActualizeazaLista(_toateMasinile);

        _totiSoferii.AddRange(_soferRepository.GetAll());
        ActualizeazaListaSoferi(_totiSoferii);
    }

    public void AdaugaMasina(Masina masina)
    {
        _masinaRepository.Adauga(masina);
        _toateMasinile.Add(masina);
        ActualizeazaLista(_toateMasinile);
    }

    public bool ActualizeazaMasina(string nrInmatriculare, Masina masinaActualizata)
    {
        Masina? masina = _toateMasinile.FirstOrDefault(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));

        if (masina == null)
        {
            return false;
        }

        masina.Model = masinaActualizata.Model;
        masina.Kilometraj = masinaActualizata.Kilometraj;
        masina.Culoare = masinaActualizata.Culoare;
        masina.Optiuni = masinaActualizata.Optiuni;

        _masinaFileRepository.SalveazaToti(_toateMasinile);
        ActualizeazaLista(_toateMasinile);
        return true;
    }

    public void CautaDupaNrInmatriculare(string termen)
    {
        if (string.IsNullOrWhiteSpace(termen))
        {
            ActualizeazaLista(_toateMasinile);
            return;
        }

        IEnumerable<Masina> rezultate = _toateMasinile
            .Where(m => m.NrInmatriculare.Contains(termen.Trim(), StringComparison.OrdinalIgnoreCase));

        ActualizeazaLista(rezultate);
    }

    public void ResetCautare()
    {
        ActualizeazaLista(_toateMasinile);
    }

    public bool ExistaNrInmatriculare(string nrInmatriculare)
    {
        return _toateMasinile.Any(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));
    }

    public void AdaugaSofer(Sofer sofer)
    {
        _soferRepository.Adauga(sofer);
        _totiSoferii.Add(sofer);
        ActualizeazaListaSoferi(_totiSoferii);
    }

    public bool ActualizeazaSofer(int id, string nume, double kilometri)
    {
        Sofer? sofer = _totiSoferii.FirstOrDefault(s => s.Id == id);

        if (sofer == null)
        {
            return false;
        }

        sofer.Nume = nume;
        sofer.TotalKilometriParcursi = kilometri;
        _soferFileRepository.SalveazaToti(_totiSoferii);
        ActualizeazaListaSoferi(_totiSoferii);
        return true;
    }

    public bool StergeSofer(int id)
    {
        Sofer? sofer = _totiSoferii.FirstOrDefault(s => s.Id == id);

        if (sofer == null)
        {
            return false;
        }

        _totiSoferii.Remove(sofer);
        _soferFileRepository.SalveazaToti(_totiSoferii);
        ActualizeazaListaSoferi(_totiSoferii);
        return true;
    }

    public void CautaSoferDupaNume(string nume)
    {
        if (string.IsNullOrWhiteSpace(nume))
        {
            ActualizeazaListaSoferi(_totiSoferii);
            return;
        }

        IEnumerable<Sofer> rezultate = _totiSoferii
            .Where(s => s.Nume.Contains(nume.Trim(), StringComparison.OrdinalIgnoreCase));

        ActualizeazaListaSoferi(rezultate);
    }

    public void ResetCautareSoferi()
    {
        ActualizeazaListaSoferi(_totiSoferii);
    }

    public bool ExistaSofer(int id)
    {
        return _totiSoferii.Any(s => s.Id == id);
    }

    private void ActualizeazaLista(IEnumerable<Masina> masini)
    {
        Masini.Clear();

        foreach (Masina masina in masini)
        {
            Masini.Add(masina);
        }
    }

    private void ActualizeazaListaSoferi(IEnumerable<Sofer> soferi)
    {
        Soferi.Clear();

        foreach (Sofer sofer in soferi)
        {
            Soferi.Add(sofer);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
