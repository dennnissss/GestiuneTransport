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

public class Cursa
{
    public int Id { get; set; }
    public string LocPlecare { get; set; }
    public string Destinatie { get; set; }
    public DateTime DataPlecare { get; set; }
    public DateTime DataSosire { get; set; }
    public Masina MasinaAlocata { get; set; }
    public Sofer SoferAlocat { get; set; }
    public TipCursa Tip { get; set; }
    public StatusCursa Status { get; set; }
    public double DistantaKm { get; set; }
    public decimal CostEstimativ { get; set; }

    public string Ruta => $"{LocPlecare} - {Destinatie}";
    public string MasinaAfisare => $"{MasinaAlocata.NrInmatriculare} / {MasinaAlocata.ModelComplet}";
    public string SoferAfisare => $"{SoferAlocat.Nume} ({SoferAlocat.CategoriePermis})";

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
    {
        Id = id;
        LocPlecare = locPlecare;
        Destinatie = destinatie;
        DataPlecare = dataPlecare;
        DataSosire = dataSosire;
        MasinaAlocata = masinaAlocata;
        SoferAlocat = soferAlocat;
        Tip = tip;
        Status = status;
        DistantaKm = distantaKm;
        CostEstimativ = costEstimativ;
    }
}
