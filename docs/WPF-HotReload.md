# Demonstratie WPF si XAML Hot Reload

## Scop

Proiectul `GestiuneTransport.WpfApp` afiseaza informatii despre entitatea `Masina` din aplicatia GestiuneTransport. Fereastra foloseste binding catre `MainWindowViewModel`, iar datele afisate provin din modelul existent `GestiuneTransport.Models.Masina`.

## Utilizarea XAML Hot Reload

1. Deschideti solutia `tema2.sln` in Visual Studio.
2. Setati `GestiuneTransport.WpfApp` ca proiect de pornire.
3. Porniti aplicatia cu `F5` sau `Debug > Start Debugging`.
4. Lasati fereastra WPF deschisa si editati `MainWindow.xaml`.
5. Salvati fisierul sau asteptati aplicarea automata. Visual Studio aplica modificarile XAML direct in fereastra rulata.

## Exemple de modificari vizibile in timp real

- In `Window.Resources`, schimbati `PrimaryBrush` din `#176B5B` in alta culoare.
- Modificati `FontSize` pentru titlul paginii.
- Schimbati textul sau `Padding` pentru cardul de status.
- Adaugati sau eliminati un `TextBlock` din zona de observatii.

## Observatii

XAML Hot Reload ajuta la ajustarea rapida a interfetei fara oprirea aplicatiei. Este util pentru testarea culorilor, spatierilor, fonturilor si layout-ului. Unele schimbari structurale majore sau modificari in codul C# pot necesita rebuild si repornirea aplicatiei.
