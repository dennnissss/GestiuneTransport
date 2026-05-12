namespace GestiuneTransport.Models;

public enum MarcaMasina
{
    Dacia,
    Volkswagen,
    Ford,
    Mercedes,
    BMW,
    Renault,
    Toyota,
    Hyundai
}

public enum Culoare
{
    Rosu,
    Alb,
    Negru,
    Gri,
    Albastru,
    Verde
}

public enum CombustibilMasina
{
    Benzina,
    Diesel,
    Hybrid,
    Electric
}

public enum StatusMasina
{
    Disponibila,
    InCursa,
    Service,
    Inactiva
}

[Flags]
public enum Optiuni
{
    Niciuna = 0,
    AerConditionat = 1,
    Navigatie = 2,
    SenzoriParcare = 4,
    ScauneIncalzite = 8,
    CutieAutomata = 16,
    CameraMarsarier = 32
}

public class Masina
{
    public string NrInmatriculare { get; set; }
    public MarcaMasina Marca { get; set; }
    public string Model { get; set; }
    public int AnFabricatie { get; set; }
    public double Kilometraj { get; set; }
    public Culoare Culoare { get; set; }
    public CombustibilMasina Combustibil { get; set; }
    public StatusMasina Status { get; set; }
    public Optiuni Optiuni { get; set; }

    public string ModelComplet => $"{Marca} {Model}";
    public string DescriereScurta => $"{NrInmatriculare} - {ModelComplet} ({Status})";

    public Masina(
        string nrInmatriculare,
        string model,
        double kilometrajInitial,
        Culoare culoare,
        Optiuni optiuni)
        : this(
            nrInmatriculare,
            MarcaMasina.Dacia,
            model,
            DateTime.Now.Year,
            kilometrajInitial,
            culoare,
            CombustibilMasina.Diesel,
            StatusMasina.Disponibila,
            optiuni)
    {
    }

    public Masina(
        string nrInmatriculare,
        MarcaMasina marca,
        string model,
        int anFabricatie,
        double kilometrajInitial,
        Culoare culoare,
        CombustibilMasina combustibil,
        StatusMasina status,
        Optiuni optiuni)
    {
        NrInmatriculare = nrInmatriculare;
        Marca = marca;
        Model = model;
        AnFabricatie = anFabricatie;
        Kilometraj = kilometrajInitial;
        Culoare = culoare;
        Combustibil = combustibil;
        Status = status;
        Optiuni = optiuni;
    }

    public override string ToString()
    {
        string optiuniText = Optiuni == Optiuni.Niciuna
            ? "Niciuna"
            : Optiuni.ToString();

        return $"{ModelComplet} [{NrInmatriculare}] - {AnFabricatie}, {Kilometraj:N0} km, {Combustibil}, {Status}, Optiuni: {optiuniText}";
    }
}
