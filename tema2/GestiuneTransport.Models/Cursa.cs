namespace GestiuneTransport.Models;

public enum TipCursa
{
    Interna,
    Internationala
}

public enum StatusCursa
{
    Planificata,
    InDesfasurare,
    Finalizata,
    Anulata
}

public enum PrioritateCursa
{
    Normala,
    Rapida,
    Urgenta
}

public class Cursa
{
    public int Id { get; set; }
    public string Client { get; set; }
    public string Marfa { get; set; }
    public string LocPlecare { get; set; }
    public string Destinatie { get; set; }
    public DateTime DataPlecare { get; set; }
    public DateTime DataSosire { get; set; }
    public Masina MasinaAlocata { get; set; }
    public Sofer SoferAlocat { get; set; }
    public TipCursa Tip { get; set; }
    public StatusCursa Status { get; set; }
    public PrioritateCursa Prioritate { get; set; }
    public double DistantaKm { get; set; }
    public decimal PretPerKm { get; set; }
    public decimal CostEstimativ { get; set; }
    public string Observatii { get; set; }

    public string Ruta => $"{LocPlecare} - {Destinatie}";
    public string MasinaAfisare => $"{MasinaAlocata.NrInmatriculare} / {MasinaAlocata.ModelComplet}";
    public string SoferAfisare => $"{SoferAlocat.Nume} ({SoferAlocat.CategoriePermis})";
    public string ClientAfisare => string.IsNullOrWhiteSpace(Client) ? "Client nespecificat" : Client;

    public Cursa(
        int id,
        string locPlecare,
        string destinatie,
        DateTime dataPlecare,
        DateTime dataSosire,
        Masina masinaAlocata,
        Sofer soferAlocat,
        TipCursa tip,
        StatusCursa status,
        double distantaKm,
        decimal costEstimativ)
        : this(
            id,
            string.Empty,
            string.Empty,
            locPlecare,
            destinatie,
            dataPlecare,
            dataSosire,
            masinaAlocata,
            soferAlocat,
            tip,
            status,
            PrioritateCursa.Normala,
            distantaKm,
            distantaKm > 0 ? costEstimativ / (decimal)distantaKm : 0,
            costEstimativ,
            string.Empty)
    {
    }

    public Cursa(
        int id,
        string client,
        string marfa,
        string locPlecare,
        string destinatie,
        DateTime dataPlecare,
        DateTime dataSosire,
        Masina masinaAlocata,
        Sofer soferAlocat,
        TipCursa tip,
        StatusCursa status,
        PrioritateCursa prioritate,
        double distantaKm,
        decimal pretPerKm,
        decimal costEstimativ,
        string observatii)
    {
        Id = id;
        Client = client;
        Marfa = marfa;
        LocPlecare = locPlecare;
        Destinatie = destinatie;
        DataPlecare = dataPlecare;
        DataSosire = dataSosire;
        MasinaAlocata = masinaAlocata;
        SoferAlocat = soferAlocat;
        Tip = tip;
        Status = status;
        Prioritate = prioritate;
        DistantaKm = distantaKm;
        PretPerKm = pretPerKm;
        CostEstimativ = costEstimativ;
        Observatii = observatii;
    }
}
