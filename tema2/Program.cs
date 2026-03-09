using System;
using System.Collections.Generic;
using System.Linq;
using GestiuneTransport.Models;

namespace GestiuneTransport
{
    class Program
    {
        // Lista care ține locul "vectorului de obiecte" (List este varianta modernă și recomandată în C#)
        static List<Sofer> listaSoferi = new List<Sofer>();

        static void Main(string[] args)
        {
            string optiune;
            do
            {
                Console.WriteLine("\n--- SISTEM GESTIUNE TRANSPORT ---");
                Console.WriteLine("1. Adauga sofer (Citire tastatura)");
                Console.WriteLine("2. Afiseaza toti soferii (Afisare vector)");
                Console.WriteLine("3. Cauta sofer dupa nume (Cautare)");
                Console.WriteLine("X. Iesire");
                Console.Write("Alege optiunea: ");
                optiune = Console.ReadLine().ToUpper();

                switch (optiune)
                {
                    case "1": 
                        AdaugaSofer();
                        break;
                    case "2":
                        AfiseazaSoferi();
                        break;
                    case "3":
                        CautaSofer();
                        break;
                }
            } while (optiune != "X");
        }

        // 1. Citirea datelor de la tastatura
        static void AdaugaSofer()
        {
            Console.Write("Introduceti ID-ul: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Introduceti numele soferului: ");
            string nume = Console.ReadLine();

            Sofer s = new Sofer(id, nume);

            // 2. Salvarea datelor intr-un vector (lista) de obiecte
            listaSoferi.Add(s);
            Console.WriteLine("Sofer adaugat cu succes!");
        }

        // 3. Afisarea datelor dintr-un vector de obiecte
        static void AfiseazaSoferi()
        {
            Console.WriteLine("\n--- LISTA SOFERI ---");
            if (listaSoferi.Count == 0) Console.WriteLine("Nu exista soferi inregistrati.");

            foreach (var s in listaSoferi)
            {
                Console.WriteLine($"ID: {s.Id} | Nume: {s.Nume} | KM: {s.TotalKilometriParcursi}");
            }
        }

        // 4. Cautarea dupa anumite criterii (Nume)
        static void CautaSofer()
        {
            Console.Write("Introduceti numele cautat: ");
            string numeCautat = Console.ReadLine();

            // Cautare in lista
            var rezultat = listaSoferi.Where(s => s.Nume.Contains(numeCautat, StringComparison.OrdinalIgnoreCase)).ToList();

            if (rezultat.Count > 0)
            {
                Console.WriteLine("Soferi gasiti:");
                foreach (var s in rezultat)
                {
                    Console.WriteLine($"ID: {s.Id} | Nume: {s.Nume}");
                }
            }
            else
            {
                Console.WriteLine("Nu a fost gasit niciun sofer cu acest nume.");
            }
        }
    }
}