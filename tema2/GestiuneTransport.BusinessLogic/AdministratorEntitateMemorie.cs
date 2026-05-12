namespace GestiuneTransport.BusinessLogic;

public class AdministratorEntitateMemorie<T>
{
    private readonly List<T> _entitati = new();

    public void Adauga(T entitate)
    {
        _entitati.Add(entitate);
    }

    public List<T> GetAll()
    {
        return _entitati.ToList();
    }

    public List<T> Cauta(Func<T, bool> conditie)
    {
        return _entitati
            .Where(conditie)
            .ToList();
    }

    public T? CautaPrima(Func<T, bool> conditie)
    {
        return _entitati.FirstOrDefault(conditie);
    }

    public bool Sterge(Func<T, bool> conditie)
    {
        T? entitate = _entitati.FirstOrDefault(conditie);

        if (entitate == null)
        {
            return false;
        }

        return _entitati.Remove(entitate);
    }
}
