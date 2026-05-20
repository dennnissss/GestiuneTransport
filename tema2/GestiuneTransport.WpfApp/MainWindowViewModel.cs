using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GestiuneTransport.Models;
using GestiuneTransport.StocareDate;

namespace GestiuneTransport.WpfApp;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MasinaFileRepository _masinaFileRepository = new();
    private readonly SoferFileRepository _soferFileRepository = new();
    private readonly CursaFileRepository _cursaFileRepository = new();
    private readonly List<Masina> _toateMasinile = new();
    private readonly List<Sofer> _totiSoferii = new();
    private readonly List<Cursa> _toateCursele = new();

    public ObservableCollection<Masina> Masini { get; } = new();
    public ObservableCollection<Sofer> Soferi { get; } = new();
    public ObservableCollection<Cursa> Curse { get; } = new();
    public ObservableCollection<Masina> MasiniPentruSelectie { get; } = new();
    public ObservableCollection<Sofer> SoferiPentruSelectie { get; } = new();
    private Cursa? _cursaSelectata;

    public IReadOnlyList<MarcaMasina> Marci { get; } = Enum.GetValues<MarcaMasina>();
    public IReadOnlyList<Culoare> Culori { get; } = Enum.GetValues<Culoare>();
    public IReadOnlyList<CombustibilMasina> Combustibili { get; } = Enum.GetValues<CombustibilMasina>();
    public IReadOnlyList<StatusMasina> StatusuriMasina { get; } = Enum.GetValues<StatusMasina>();
    public IReadOnlyList<CategoriePermis> CategoriiPermis { get; } = Enum.GetValues<CategoriePermis>();
    public IReadOnlyList<StatusSofer> StatusuriSofer { get; } = Enum.GetValues<StatusSofer>();
    public IReadOnlyList<TipCursa> TipuriCursa { get; } = Enum.GetValues<TipCursa>();
    public IReadOnlyList<StatusCursa> StatusuriCursa { get; } = Enum.GetValues<StatusCursa>();
    public IReadOnlyList<PrioritateCursa> PrioritatiCursa { get; } = Enum.GetValues<PrioritateCursa>();
    public IReadOnlyList<int> AniFabricatie { get; } = Enumerable.Range(2000, DateTime.Now.Year - 1999).Reverse().ToList();

    public int TotalMasini => _toateMasinile.Count;
    public int MasiniDisponibile => _toateMasinile.Count(m => m.Status == StatusMasina.Disponibila);
    public int MasiniInService => _toateMasinile.Count(m => m.Status == StatusMasina.Service);
    public int TotalSoferi => _totiSoferii.Count;
    public int SoferiDisponibili => _totiSoferii.Count(s => s.Status == StatusSofer.Disponibil);
    public int SoferiInCursa => _totiSoferii.Count(s => s.Status == StatusSofer.InCursa);
    public int TotalCurse => _toateCursele.Count;
    public int CursePlanificate => _toateCursele.Count(c => c.Status == StatusCursa.Planificata);
    public int CurseActive => _toateCursele.Count(c => c.Status == StatusCursa.InDesfasurare);
    public int CurseAstazi => _toateCursele.Count(c => c.DataPlecare.Date == DateTime.Today);
    public double KilometriPlanificati => _toateCursele.Sum(c => c.DistantaKm);
    public decimal ValoareCurse => _toateCursele.Sum(c => c.CostEstimativ);
    public string RataDisponibilitate => TotalMasini == 0
        ? "0%"
        : $"{(MasiniDisponibile * 100.0 / TotalMasini):N0}%";

    public string UrmatoareaCursaTitlu
    {
        get
        {
            Cursa? cursa = _toateCursele
                .Where(c => c.Status is StatusCursa.Planificata or StatusCursa.InDesfasurare)
                .OrderBy(c => c.DataPlecare)
                .FirstOrDefault();

            return cursa == null ? "Nicio cursa planificata" : cursa.Ruta;
        }
    }

    public string UrmatoareaCursaDetalii
    {
        get
        {
            Cursa? cursa = _toateCursele
                .Where(c => c.Status is StatusCursa.Planificata or StatusCursa.InDesfasurare)
                .OrderBy(c => c.DataPlecare)
                .FirstOrDefault();

            return cursa == null
                ? "Adauga o cursa ca sa vezi rapid urmatoarea plecare."
                : $"{cursa.DataPlecare:dd.MM.yyyy HH:mm} / {cursa.SoferAlocat.Nume} / {cursa.MasinaAlocata.NrInmatriculare}";
        }
    }

    public string RezumatMasini => Masini.Count == 1
        ? "1 masina afisata"
        : $"{Masini.Count} masini afisate";

    public string RezumatSoferi => Soferi.Count == 1
        ? "1 sofer afisat"
        : $"{Soferi.Count} soferi afisati";

    public string RezumatCurse => Curse.Count == 1
        ? "1 cursa afisata"
        : $"{Curse.Count} curse afisate";

    public Cursa? CursaSelectata
    {
        get => _cursaSelectata;
        set
        {
            _cursaSelectata = value;
            OnPropertyChanged();
            NotificaCursaSelectata();
        }
    }

    public string CursaSelectataTitlu => CursaSelectata?.Ruta ?? "Selecteaza o cursa";
    public string CursaSelectataClient => CursaSelectata?.ClientAfisare ?? "Detaliile apar aici dupa selectie.";
    public string CursaSelectataResurse => CursaSelectata?.MasinaAfisare ?? "Masina nealocata";
    public string CursaSelectataSofer => CursaSelectata?.SoferAfisare ?? "Sofer nealocat";
    public string CursaSelectataCost => CursaSelectata == null ? "0 lei" : $"{CursaSelectata.CostEstimativ:N0} lei";
    public string CursaSelectataStatus => CursaSelectata?.Status.ToString() ?? "Neselectata";
    public string CursaSelectataProgram => CursaSelectata == null
        ? "Alege o cursa din tabel pentru program."
        : $"{CursaSelectata.DataPlecare:dd.MM.yyyy HH:mm} - {CursaSelectata.DataSosire:dd.MM.yyyy HH:mm}";

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindowViewModel()
    {
        Masini.CollectionChanged += (_, _) => OnPropertyChanged(nameof(RezumatMasini));
        Soferi.CollectionChanged += (_, _) => OnPropertyChanged(nameof(RezumatSoferi));
        Curse.CollectionChanged += (_, _) => OnPropertyChanged(nameof(RezumatCurse));

        _toateMasinile.AddRange(_masinaFileRepository.IncarcaToti());
        _totiSoferii.AddRange(_soferFileRepository.IncarcaToti());
        _toateCursele.AddRange(_cursaFileRepository.IncarcaToti(_totiSoferii, _toateMasinile));

        ActualizeazaListaMasini(_toateMasinile);
        ActualizeazaListaSoferi(_totiSoferii);
        ActualizeazaSelectiiCurse();
        ActualizeazaListaCurse(_toateCursele);
        NotificaDashboard();
    }

    public static IReadOnlyList<string> GetModelePentruMarca(MarcaMasina marca)
    {
        return marca switch
        {
            MarcaMasina.Dacia => new[] { "Dokker Van", "Logan Van", "Duster", "Jogger", "Logan MCV", "Pick-Up" },
            MarcaMasina.Volkswagen => new[] { "Crafter", "Transporter", "Caddy Cargo", "Caddy Maxi", "Multivan", "Amarok", "LT" },
            MarcaMasina.Ford => new[] { "Transit", "Transit Custom", "Transit Connect", "Transit Courier", "Ranger", "Tourneo Custom", "F-Max", "Cargo" },
            MarcaMasina.Mercedes => new[] { "Sprinter", "Vito", "Citan", "Vario", "Atego", "Actros", "Antos", "Arocs", "Econic" },
            MarcaMasina.BMW => new[] { "X5", "X7", "Seria 5 Touring", "Seria 3 Touring" },
            MarcaMasina.Renault => new[] { "Master", "Trafic", "Kangoo Van", "Express Van", "Mascott", "Midlum", "Premium", "T", "D", "C", "K" },
            MarcaMasina.Toyota => new[] { "Proace", "Proace City", "Proace Max", "Hilux", "Land Cruiser", "HiAce", "Dyna" },
            MarcaMasina.Hyundai => new[] { "H350", "Staria Cargo", "H-1 Cargo", "Porter", "Mighty", "Xcient" },
            MarcaMasina.Iveco => new[] { "Daily", "Eurocargo", "S-Way", "X-Way", "T-Way", "Stralis", "Trakker" },
            MarcaMasina.MAN => new[] { "TGE", "TGL", "TGM", "TGS", "TGX", "Lion's City", "CLA" },
            MarcaMasina.Scania => new[] { "P-Series", "G-Series", "R-Series", "S-Series", "L-Series", "XT", "Super" },
            MarcaMasina.Volvo => new[] { "FH", "FM", "FMX", "FE", "FL", "VNL", "VNR" },
            MarcaMasina.DAF => new[] { "LF", "CF", "XF", "XD", "XG", "XG+" },
            MarcaMasina.Fiat => new[] { "Ducato", "Doblo Cargo", "Talento", "Scudo", "Fiorino", "Fullback" },
            MarcaMasina.Citroen => new[] { "Berlingo Van", "Jumpy", "Jumper", "SpaceTourer", "Relay" },
            MarcaMasina.Peugeot => new[] { "Partner", "Expert", "Boxer", "Traveller", "Rifter Van" },
            MarcaMasina.Opel => new[] { "Combo Cargo", "Vivaro", "Movano", "Zafira Life", "Campo" },
            MarcaMasina.Nissan => new[] { "NV200", "NV300", "NV400", "Interstar", "Primastar", "Townstar", "Cabstar", "Atleon", "NT400" },
            MarcaMasina.Isuzu => new[] { "D-Max", "N-Series", "F-Series", "Forward", "Elf" },
            MarcaMasina.MitsubishiFuso => new[] { "Canter", "eCanter", "Fighter", "Super Great", "Rosa" },
            _ => Array.Empty<string>()
        };
    }

    public void AdaugaMasina(Masina masina)
    {
        _toateMasinile.Add(masina);
        SalveazaMasini();
        ActualizeazaListaMasini(_toateMasinile);
        ActualizeazaSelectiiCurse();
        NotificaDashboard();
    }

    public bool ActualizeazaMasina(string nrInmatriculare, Masina masinaActualizata)
    {
        Masina? masina = _toateMasinile.FirstOrDefault(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));

        if (masina == null)
        {
            return false;
        }

        masina.NrInmatriculare = masinaActualizata.NrInmatriculare;
        masina.Marca = masinaActualizata.Marca;
        masina.Model = masinaActualizata.Model;
        masina.AnFabricatie = masinaActualizata.AnFabricatie;
        masina.Kilometraj = masinaActualizata.Kilometraj;
        masina.Culoare = masinaActualizata.Culoare;
        masina.Combustibil = masinaActualizata.Combustibil;
        masina.Status = masinaActualizata.Status;
        masina.Optiuni = masinaActualizata.Optiuni;

        SalveazaMasini();
        ActualizeazaListaMasini(_toateMasinile);
        ActualizeazaSelectiiCurse();
        NotificaDashboard();
        return true;
    }

    public bool StergeMasina(string nrInmatriculare)
    {
        if (_toateCursele.Any(c =>
                c.MasinaAlocata.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Masina? masina = _toateMasinile.FirstOrDefault(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase));

        if (masina == null)
        {
            return false;
        }

        _toateMasinile.Remove(masina);
        SalveazaMasini();
        ActualizeazaListaMasini(_toateMasinile);
        ActualizeazaSelectiiCurse();
        NotificaDashboard();
        return true;
    }

    public void FiltreazaMasini(string text, MarcaMasina? marca, StatusMasina? status)
    {
        IEnumerable<Masina> rezultate = _toateMasinile;

        if (!string.IsNullOrWhiteSpace(text))
        {
            string termen = text.Trim();
            rezultate = rezultate.Where(m =>
                m.NrInmatriculare.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                m.Model.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                m.Marca.ToString().Contains(termen, StringComparison.OrdinalIgnoreCase));
        }

        if (marca.HasValue)
        {
            rezultate = rezultate.Where(m => m.Marca == marca.Value);
        }

        if (status.HasValue)
        {
            rezultate = rezultate.Where(m => m.Status == status.Value);
        }

        ActualizeazaListaMasini(rezultate);
    }

    public void ResetFiltreMasini()
    {
        ActualizeazaListaMasini(_toateMasinile);
    }

    public bool ExistaNrInmatriculare(string nrInmatriculare, string? nrIgnorat = null)
    {
        return _toateMasinile.Any(m =>
            m.NrInmatriculare.Equals(nrInmatriculare, StringComparison.OrdinalIgnoreCase) &&
            !m.NrInmatriculare.Equals(nrIgnorat ?? string.Empty, StringComparison.OrdinalIgnoreCase));
    }

    public void AdaugaSofer(Sofer sofer)
    {
        _totiSoferii.Add(sofer);
        SalveazaSoferi();
        ActualizeazaListaSoferi(_totiSoferii);
        ActualizeazaSelectiiCurse();
        NotificaDashboard();
    }

    public bool ActualizeazaSofer(int id, Sofer soferActualizat)
    {
        Sofer? sofer = _totiSoferii.FirstOrDefault(s => s.Id == id);

        if (sofer == null)
        {
            return false;
        }

        sofer.Nume = soferActualizat.Nume;
        sofer.Telefon = soferActualizat.Telefon;
        sofer.CategoriePermis = soferActualizat.CategoriePermis;
        sofer.Status = soferActualizat.Status;
        sofer.TotalKilometriParcursi = soferActualizat.TotalKilometriParcursi;

        SalveazaSoferi();
        ActualizeazaListaSoferi(_totiSoferii);
        ActualizeazaSelectiiCurse();
        NotificaDashboard();
        return true;
    }

    public bool StergeSofer(int id)
    {
        if (_toateCursele.Any(c => c.SoferAlocat.Id == id))
        {
            return false;
        }

        Sofer? sofer = _totiSoferii.FirstOrDefault(s => s.Id == id);

        if (sofer == null)
        {
            return false;
        }

        _totiSoferii.Remove(sofer);
        SalveazaSoferi();
        ActualizeazaListaSoferi(_totiSoferii);
        ActualizeazaSelectiiCurse();
        NotificaDashboard();
        return true;
    }

    public void FiltreazaSoferi(string text, StatusSofer? status)
    {
        IEnumerable<Sofer> rezultate = _totiSoferii;

        if (!string.IsNullOrWhiteSpace(text))
        {
            string termen = text.Trim();
            rezultate = rezultate.Where(s =>
                s.Nume.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                s.Telefon.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                s.Id.ToString().Contains(termen, StringComparison.OrdinalIgnoreCase));
        }

        if (status.HasValue)
        {
            rezultate = rezultate.Where(s => s.Status == status.Value);
        }

        ActualizeazaListaSoferi(rezultate);
    }

    public void ResetFiltreSoferi()
    {
        ActualizeazaListaSoferi(_totiSoferii);
    }

    public bool ExistaSofer(int id)
    {
        return _totiSoferii.Any(s => s.Id == id);
    }

    public int GenereazaIdCursa()
    {
        return _toateCursele.Count == 0 ? 1 : _toateCursele.Max(c => c.Id) + 1;
    }

    public bool ExistaCursa(int id, int? idIgnorat = null)
    {
        return _toateCursele.Any(c => c.Id == id && c.Id != idIgnorat);
    }

    public void AdaugaCursa(Cursa cursa)
    {
        _toateCursele.Add(cursa);
        SalveazaCurse();
        ActualizeazaListaCurse(_toateCursele);
        NotificaDashboard();
    }

    public bool ActualizeazaCursa(int id, Cursa cursaActualizata)
    {
        Cursa? cursa = _toateCursele.FirstOrDefault(c => c.Id == id);

        if (cursa == null)
        {
            return false;
        }

        cursa.Id = cursaActualizata.Id;
        cursa.Client = cursaActualizata.Client;
        cursa.Marfa = cursaActualizata.Marfa;
        cursa.LocPlecare = cursaActualizata.LocPlecare;
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

        SalveazaCurse();
        ActualizeazaListaCurse(_toateCursele);
        CursaSelectata = cursa;
        NotificaDashboard();
        return true;
    }

    public bool StergeCursa(int id)
    {
        Cursa? cursa = _toateCursele.FirstOrDefault(c => c.Id == id);

        if (cursa == null)
        {
            return false;
        }

        _toateCursele.Remove(cursa);
        SalveazaCurse();
        ActualizeazaListaCurse(_toateCursele);
        NotificaDashboard();
        return true;
    }

    public void FiltreazaCurse(string text, StatusCursa? status, TipCursa? tip)
    {
        IEnumerable<Cursa> rezultate = _toateCursele;

        if (!string.IsNullOrWhiteSpace(text))
        {
            string termen = text.Trim();
            rezultate = rezultate.Where(c =>
                c.Id.ToString().Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                c.Client.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                c.Marfa.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                c.LocPlecare.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                c.Destinatie.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                c.MasinaAlocata.NrInmatriculare.Contains(termen, StringComparison.OrdinalIgnoreCase) ||
                c.SoferAlocat.Nume.Contains(termen, StringComparison.OrdinalIgnoreCase));
        }

        if (status.HasValue)
        {
            rezultate = rezultate.Where(c => c.Status == status.Value);
        }

        if (tip.HasValue)
        {
            rezultate = rezultate.Where(c => c.Tip == tip.Value);
        }

        ActualizeazaListaCurse(rezultate);
    }

    public void ResetFiltreCurse()
    {
        ActualizeazaListaCurse(_toateCursele);
    }

    public bool EsteMasinaDisponibila(Masina masina, DateTime plecare, DateTime sosire, int? idIgnorat = null)
    {
        return !_toateCursele.Any(c =>
            c.Id != idIgnorat &&
            c.Status != StatusCursa.Anulata &&
            c.MasinaAlocata.NrInmatriculare.Equals(masina.NrInmatriculare, StringComparison.OrdinalIgnoreCase) &&
            IntervaleleSeSuprapun(c.DataPlecare, c.DataSosire, plecare, sosire));
    }

    public bool EsteSoferDisponibil(Sofer sofer, DateTime plecare, DateTime sosire, int? idIgnorat = null)
    {
        return !_toateCursele.Any(c =>
            c.Id != idIgnorat &&
            c.Status != StatusCursa.Anulata &&
            c.SoferAlocat.Id == sofer.Id &&
            IntervaleleSeSuprapun(c.DataPlecare, c.DataSosire, plecare, sosire));
    }

    public void ActualizeazaSelectiiCurse(
        DateTime? plecare = null,
        DateTime? sosire = null,
        int? idIgnorat = null,
        Masina? masinaCurenta = null,
        Sofer? soferCurent = null)
    {
        IEnumerable<Masina> masini = _toateMasinile
            .Where(m => m.Status is not StatusMasina.Service and not StatusMasina.Inactiva);
        IEnumerable<Sofer> soferi = _totiSoferii
            .Where(s => s.Status is not StatusSofer.Concediu and not StatusSofer.Inactiv);

        if (plecare.HasValue && sosire.HasValue && sosire > plecare)
        {
            masini = masini.Where(m =>
                EsteMasinaDisponibila(m, plecare.Value, sosire.Value, idIgnorat) ||
                (masinaCurenta != null && m.NrInmatriculare.Equals(masinaCurenta.NrInmatriculare, StringComparison.OrdinalIgnoreCase)));
            soferi = soferi.Where(s =>
                EsteSoferDisponibil(s, plecare.Value, sosire.Value, idIgnorat) ||
                (soferCurent != null && s.Id == soferCurent.Id));
        }

        ActualizeazaMasiniPentruSelectie(masini);
        ActualizeazaSoferiPentruSelectie(soferi);
    }

    private void SalveazaMasini()
    {
        _masinaFileRepository.SalveazaToti(_toateMasinile);
    }

    private void SalveazaSoferi()
    {
        _soferFileRepository.SalveazaToti(_totiSoferii);
    }

    private void SalveazaCurse()
    {
        _cursaFileRepository.SalveazaToti(_toateCursele);
    }

    private void ActualizeazaListaMasini(IEnumerable<Masina> masini)
    {
        Masini.Clear();

        foreach (Masina masina in masini.OrderBy(m => m.Marca).ThenBy(m => m.Model))
        {
            Masini.Add(masina);
        }
    }

    private void ActualizeazaListaSoferi(IEnumerable<Sofer> soferi)
    {
        Soferi.Clear();

        foreach (Sofer sofer in soferi.OrderBy(s => s.Nume))
        {
            Soferi.Add(sofer);
        }
    }

    private void ActualizeazaListaCurse(IEnumerable<Cursa> curse)
    {
        Curse.Clear();

        foreach (Cursa cursa in curse.OrderBy(c => c.DataPlecare))
        {
            Curse.Add(cursa);
        }
    }

    private void ReincarcaSelectiiCurse()
    {
        ActualizeazaSelectiiCurse();
    }

    private void ActualizeazaMasiniPentruSelectie(IEnumerable<Masina> masini)
    {
        MasiniPentruSelectie.Clear();
        foreach (Masina masina in masini.OrderBy(m => m.NrInmatriculare))
        {
            MasiniPentruSelectie.Add(masina);
        }
    }

    private void ActualizeazaSoferiPentruSelectie(IEnumerable<Sofer> soferi)
    {
        SoferiPentruSelectie.Clear();
        foreach (Sofer sofer in soferi.OrderBy(s => s.Nume))
        {
            SoferiPentruSelectie.Add(sofer);
        }
    }

    private void NotificaDashboard()
    {
        OnPropertyChanged(nameof(TotalMasini));
        OnPropertyChanged(nameof(MasiniDisponibile));
        OnPropertyChanged(nameof(MasiniInService));
        OnPropertyChanged(nameof(TotalSoferi));
        OnPropertyChanged(nameof(SoferiDisponibili));
        OnPropertyChanged(nameof(SoferiInCursa));
        OnPropertyChanged(nameof(TotalCurse));
        OnPropertyChanged(nameof(CursePlanificate));
        OnPropertyChanged(nameof(CurseActive));
        OnPropertyChanged(nameof(CurseAstazi));
        OnPropertyChanged(nameof(KilometriPlanificati));
        OnPropertyChanged(nameof(ValoareCurse));
        OnPropertyChanged(nameof(RataDisponibilitate));
        OnPropertyChanged(nameof(UrmatoareaCursaTitlu));
        OnPropertyChanged(nameof(UrmatoareaCursaDetalii));
    }

    private void NotificaCursaSelectata()
    {
        OnPropertyChanged(nameof(CursaSelectataTitlu));
        OnPropertyChanged(nameof(CursaSelectataClient));
        OnPropertyChanged(nameof(CursaSelectataResurse));
        OnPropertyChanged(nameof(CursaSelectataSofer));
        OnPropertyChanged(nameof(CursaSelectataCost));
        OnPropertyChanged(nameof(CursaSelectataStatus));
        OnPropertyChanged(nameof(CursaSelectataProgram));
    }

    private static bool IntervaleleSeSuprapun(DateTime startA, DateTime sfarsitA, DateTime startB, DateTime sfarsitB)
    {
        return startA < sfarsitB && startB < sfarsitA;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
