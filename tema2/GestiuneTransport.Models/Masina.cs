namespace GestiuneTransport.Models;

public enum Culoare
{
    Rosu,
    Alb,
    Negru
}

[Flags]
public enum Optiuni
{
    Niciuna        = 0,
    AerConditionat = 1,
    Navigatie      = 2,
    SenzoriParcare = 4,
    ScauneIncalzite = 8
}

public class Masina
{
    public string NrInmatriculare { get; set; }
    public string Model { get; set; }
    public double Kilometraj { get; set; }
    public Culoare Culoare { get; set; }
    public Optiuni Optiuni { get; set; }

    public Masina(string nrInmatriculare, string model, double kilometrajInitial,
                  Culoare culoare, Optiuni optiuni)
    {
        NrInmatriculare = nrInmatriculare;
        Model = model;
        Kilometraj = kilometrajInitial;
        Culoare = culoare;
        Optiuni = optiuni;
    }

    public override string ToString()
    {
        string optiuniText = Optiuni == Optiuni.Niciuna
            ? "Niciuna"
            : Optiuni.ToString();

        return $"{Model} [{NrInmatriculare}] - Odometru: {Kilometraj} km | Culoare: {Culoare} | Optiuni: {optiuniText}";
    }
}