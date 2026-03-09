using System;
using System.Collections.Generic;

namespace GestiuneTransport.Models
{
    public class Sofer
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public double TotalKilometriParcursi { get; set; }
        public List<string> IstoricTrasee { get; set; } = new List<string>();

        public Sofer(int id, string nume)
        {
            Id = id;
            Nume = nume;
            TotalKilometriParcursi = 0;
        }

        public void AdaugaTraseu(string destinatie, double km)
        {
            IstoricTrasee.Add(destinatie);
            TotalKilometriParcursi += km;
        }
    }
}