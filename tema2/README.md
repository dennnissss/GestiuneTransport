# Proiecte aplicatie

Folderul contine proiectele sursa ale aplicatiei `GestiuneTransport`.

- `GestiuneTransport.Models` - clasele domeniului: masini, soferi si curse.
- `GestiuneTransport.StocareDate` - persistenta in fisiere text.
- `GestiuneTransport.BusinessLogic` - operatii asupra entitatilor.
- `GestiuneTransport.ConsoleApp` - interfata consola.
- `GestiuneTransport.WpfApp` - interfata grafica WPF.

Interfata WPF include un dashboard, design premium cu fundal texturat discret si formulare CRUD pentru `Masina`, `Sofer` si `Cursa`.
La curse se pot seta clientul, marfa, prioritatea, pretul pe kilometru si costul estimativ calculat automat.

Pentru build si rulare folositi solutia de la radacina repository-ului:

```powershell
dotnet build ..\GestiuneTransport.sln
```
