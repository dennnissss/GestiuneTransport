namespace GestiuneTransport.Models
{
    public class Masina
    {
        public string NrInmatriculare { get; set; }
        public string Model { get; set; }
        public double Kilometraj { get; set; }

        public Masina(string nrInmatriculare, string model, double kilometrajInitial)
        {
            NrInmatriculare = nrInmatriculare;
            Model = model;
            Kilometraj = kilometrajInitial;
        }

        public override string ToString()
        {
            return $"{Model} [{NrInmatriculare}] - Odometru: {Kilometraj} km";
        }
    }
}