using System;
using GestiuneTransport.Models;

class Program
{
    static void Main()
    {
        // Creare entități
        Sofer sofer1 = new Sofer(1, "Ion Popescu");
        Masina masina1 = new Masina("SV-01-ABC", "Volvo FH", 120500);

        // Simulare activitate
        Console.WriteLine($"Inregistram traseu pentru {sofer1.Nume}...");
        sofer1.AdaugaTraseu("Suceava - Bucuresti", 450);
        masina1.Kilometraj += 450;

        // Afisare rezultate
        Console.WriteLine("\n--- Raport Activitate ---");
        Console.WriteLine($"Sofer: {sofer1.Nume} | Total KM: {sofer1.TotalKilometriParcursi}");
        Console.WriteLine($"Trasee efectuate: {string.Join(", ", sofer1.IstoricTrasee)}");
        Console.WriteLine($"Stare masina: {masina1.ToString()}");
    }
}