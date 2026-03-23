namespace GestiuneTransport.Models;

public class IntervalLucru
{
    public Sofer SoferAlocat { get; set; }
    public Masina MasinaAlocata { get; set; }
    public DateTime DataStart { get; set; }
    public DateTime DataSfarsit { get; set; }

    public IntervalLucru(Sofer sofer, Masina masina, DateTime start, DateTime sfarsit)
    {
        SoferAlocat = sofer;
        MasinaAlocata = masina;
        DataStart = start;
        DataSfarsit = sfarsit;
    }
}