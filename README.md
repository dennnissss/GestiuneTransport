# GestiuneTransport

Aplicatie C# pentru gestionarea unei firme de transport. Proiectul include modele de domeniu, stocare in fisiere text, logica de business, o aplicatie consola si o interfata WPF.

## Structura

- `GestiuneTransport.sln` - solutia principala care trebuie deschisa in Visual Studio.
- `tema2/GestiuneTransport.Models` - entitatile aplicatiei: `Sofer`, `Masina`, `IntervalLucru`.
- `tema2/GestiuneTransport.StocareDate` - salvare si incarcare din fisiere text.
- `tema2/GestiuneTransport.BusinessLogic` - operatii de cautare, adaugare, modificare si stergere.
- `tema2/GestiuneTransport.ConsoleApp` - varianta de consola.
- `tema2/GestiuneTransport.WpfApp` - interfata grafica WPF.
- `docs/WPF-HotReload.md` - demonstratie pentru XAML Hot Reload.

## Functionalitati WPF

- CRUD pentru `Masina`.
- CRUD pentru `Sofer`, a doua entitate din aplicatia proprie.
- Cautare masini dupa numar de inmatriculare.
- Cautare soferi dupa nume.
- Validare date introduse si marcarea campurilor invalide.
- Controale WPF folosite: `Menu`, `TabControl`, `Grid`, `StackPanel`, `TextBox`, `ComboBox`, `ListBox`, `CheckBox`, `RadioButton`, `DataGrid`.
- Binding pentru colectii si texte de stare, de exemplu `ItemsSource="{Binding Soferi}"`, `ItemsSource="{Binding Masini}"`, `Text="{Binding RezumatSoferi}"`.

## Rulare

Din linia de comanda:

```powershell
dotnet build GestiuneTransport.sln
dotnet run --project .\tema2\GestiuneTransport.WpfApp\GestiuneTransport.WpfApp.csproj
```

In Visual Studio:

1. Deschide `GestiuneTransport.sln`.
2. Seteaza `GestiuneTransport.WpfApp` ca startup project.
3. Porneste aplicatia cu `F5`.

## Persistenta

Datele sunt salvate in fisiere text prin proiectul `GestiuneTransport.StocareDate`.

- `masini.txt` pentru masini.
- `soferi.txt` pentru soferi.
- `intervale.txt` pentru intervale de lucru.
