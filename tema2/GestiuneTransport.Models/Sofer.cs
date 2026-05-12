namespace GestiuneTransport.Models;

public enum CategoriePermis
{
    B,
    C,
    D,
    CE
}

public enum StatusSofer
{
    Disponibil,
    InCursa,
    Concediu,
    Inactiv
}

public class Sofer
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public string Telefon { get; set; }
    public CategoriePermis CategoriePermis { get; set; }
    public StatusSofer Status { get; set; }
    public double TotalKilometriParcursi { get; set; }
    public List<string> IstoricTrasee { get; set; } = new List<string>();
    public string DescriereScurta => $"{Nume} - permis {CategoriePermis} ({Status})";

    public Sofer(int id, string nume)
        : this(id, nume, string.Empty, CategoriePermis.B, StatusSofer.Disponibil)
    {
    }

    public Sofer(
        int id,
        string nume,
        string telefon,
        CategoriePermis categoriePermis,
        StatusSofer status)
    {
        Id = id;
        Nume = nume;
        Telefon = telefon;
        CategoriePermis = categoriePermis;
        Status = status;
        TotalKilometriParcursi = 0;
    }

    public void AdaugaTraseu(string destinatie, double km)
    {
        IstoricTrasee.Add(destinatie);
        TotalKilometriParcursi += km;
    }
}
