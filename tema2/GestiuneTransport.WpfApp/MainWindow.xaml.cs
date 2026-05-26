using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GestiuneTransport.Models;

namespace GestiuneTransport.WpfApp;

public partial class MainWindow : Window
{
    private const int NrInmatriculareMinLength = 5;
    private const int NrInmatriculareMaxLength = 12;
    private const double KilometrajMinim = 0;
    private const double KilometrajMaxim = 2_000_000;
    private const int SoferIdMinim = 1;
    private const int SoferNumeMinLength = 2;
    private const int SoferNumeMaxLength = 50;
    private const int SoferTelefonMinLength = 7;
    private const int SoferTelefonMaxLength = 18;
    private const double SoferKmMinim = 0;
    private const double SoferKmMaxim = 5_000_000;
    private const int CursaIdMinim = 1;
    private const int CursaTextMinLength = 2;
    private const int CursaTextMaxLength = 60;
    private const double DistantaMinima = 1;
    private const double DistantaMaxima = 100_000;
    private const decimal PretKmMinim = 0.1m;
    private const decimal PretKmMaxim = 1_000m;
    private const decimal CostMinim = 0;
    private const decimal CostMaxim = 1_000_000;

    private readonly Brush _labelNormalBrush = new SolidColorBrush(Color.FromRgb(94, 107, 104));
    private readonly Brush _labelInvalidBrush = new SolidColorBrush(Color.FromRgb(180, 45, 45));
    private readonly MainWindowViewModel _viewModel = new();
    private bool _initializareCompleta;
    private bool _actualizareProgramatica;
    private DispatcherTimer? _toastTimer;
    private string? _nrInmatriculareEditare;
    private int? _soferIdEditare;
    private int? _cursaIdEditare;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;

        InitializeazaListeMasini();
        InitializeazaListeSoferi();
        InitializeazaListeCurse();
        CurataFormularMasina();
        CurataFormularSofer();
        CurataFormularCursa();

        _initializareCompleta = true;
        SelecteazaPrimaCursaDisponibila();
    }

    private void InitializeazaListeMasini()
    {
        MarcaComboBox.ItemsSource = _viewModel.Marci;
        CuloareComboBox.ItemsSource = _viewModel.Culori;
        CombustibilComboBox.ItemsSource = _viewModel.Combustibili;
        StatusMasinaComboBox.ItemsSource = _viewModel.StatusuriMasina;
        AnComboBox.ItemsSource = _viewModel.AniFabricatie;

        MasinaMarcaFilterComboBox.ItemsSource = _viewModel.Marci;
        MasinaStatusFilterComboBox.ItemsSource = _viewModel.StatusuriMasina;
    }

    private void InitializeazaListeSoferi()
    {
        SoferPermisComboBox.ItemsSource = _viewModel.CategoriiPermis;
        SoferStatusComboBox.ItemsSource = _viewModel.StatusuriSofer;
        SoferStatusFilterComboBox.ItemsSource = _viewModel.StatusuriSofer;
    }

    private void InitializeazaListeCurse()
    {
        CursaStatusComboBox.ItemsSource = _viewModel.StatusuriCursa;
        CursaStatusFilterComboBox.ItemsSource = _viewModel.StatusuriCursa;
        CursaTipFilterComboBox.ItemsSource = _viewModel.TipuriCursa;
        PrioritateCursaComboBox.ItemsSource = _viewModel.PrioritatiCursa;
        CursaMasinaComboBox.ItemsSource = _viewModel.MasiniPentruSelectie;
        CursaSoferComboBox.ItemsSource = _viewModel.SoferiPentruSelectie;
    }

    private void MarcaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MarcaComboBox.SelectedItem is not MarcaMasina marca)
        {
            ModelComboBox.ItemsSource = null;
            return;
        }

        string? modelCurent = ModelComboBox.SelectedItem as string;
        var modele = MainWindowViewModel.GetModelePentruMarca(marca);
        ModelComboBox.ItemsSource = modele;

        if (modelCurent != null && modele.Contains(modelCurent))
        {
            ModelComboBox.SelectedItem = modelCurent;
        }
        else
        {
            ModelComboBox.SelectedIndex = modele.Count > 0 ? 0 : -1;
        }

        ResetValidareMasina();
    }

    private void SalveazaMasina_Click(object sender, RoutedEventArgs e)
    {
        ResetValidareMasina();

        if (!ValideazaMasina(out Masina masina))
        {
            return;
        }

        if (_nrInmatriculareEditare == null)
        {
            _viewModel.AdaugaMasina(masina);
            CurataFormularMasina();
            AfiseazaMesajMasina("Masina a fost adaugata.", esteEroare: false);
            AfiseazaToast("Masina a fost adaugata.");
            return;
        }

        bool actualizat = _viewModel.ActualizeazaMasina(_nrInmatriculareEditare, masina);
        _nrInmatriculareEditare = actualizat ? masina.NrInmatriculare : _nrInmatriculareEditare;
        AfiseazaMesajMasina(
            actualizat ? "Modificarile au fost salvate." : "Masina selectata nu a fost gasita.",
            esteEroare: !actualizat);
        if (actualizat)
        {
            AfiseazaToast("Masina a fost actualizata.");
        }
    }

    private void StergeMasina_Click(object sender, RoutedEventArgs e)
    {
        if (_nrInmatriculareEditare == null)
        {
            AfiseazaMesajMasina("Selecteaza o masina din tabel inainte de stergere.", esteEroare: true);
            return;
        }

        if (!ConfirmaStergere("Stergi masina selectata?"))
        {
            return;
        }

        bool sters = _viewModel.StergeMasina(_nrInmatriculareEditare);
        if (sters)
        {
            CurataFormularMasina();
            AfiseazaToast("Masina a fost stearsa.");
        }

        AfiseazaMesajMasina(
            sters ? "Masina a fost stearsa." : "Masina nu poate fi stearsa: nu exista sau este folosita intr-o cursa.",
            esteEroare: !sters);
    }

    private void MasinaNoua_Click(object sender, RoutedEventArgs e)
    {
        CurataFormularMasina();
        ResetValidareMasina();
    }

    private void MasiniDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MasiniDataGrid.SelectedItem is Masina masina)
        {
            IncarcaMasina(masina);
        }
    }

    private void EditeazaMasinaContext_Click(object sender, RoutedEventArgs e)
    {
        if (MasiniDataGrid.SelectedItem is not Masina masina)
        {
            AfiseazaToast("Selecteaza o masina pentru editare.");
            return;
        }

        IncarcaMasina(masina);
        MainTabControl.SelectedItem = MasiniTab;
        NrInmatriculareTextBox.Focus();
        NrInmatriculareTextBox.SelectAll();
        AfiseazaToast("Masina este pregatita pentru editare.");
    }

    private void StergeMasinaContext_Click(object sender, RoutedEventArgs e)
    {
        StergeMasina_Click(sender, e);
    }

    private void MasinaFiltru_Changed(object sender, EventArgs e)
    {
        if (!_initializareCompleta)
        {
            return;
        }

        MarcaMasina? marca = MasinaMarcaFilterComboBox.SelectedItem is MarcaMasina marcaSelectata ? marcaSelectata : null;
        StatusMasina? status = MasinaStatusFilterComboBox.SelectedItem is StatusMasina statusSelectat ? statusSelectat : null;
        _viewModel.FiltreazaMasini(MasinaSearchTextBox.Text, marca, status);
    }

    private void ResetFiltreMasini_Click(object sender, RoutedEventArgs e)
    {
        MasinaSearchTextBox.Clear();
        MasinaMarcaFilterComboBox.SelectedIndex = -1;
        MasinaStatusFilterComboBox.SelectedIndex = -1;
        _viewModel.ResetFiltreMasini();
    }

    private void MasinaCamp_Changed(object sender, EventArgs e)
    {
        ResetValidareMasina();
    }

    private bool ValideazaMasina(out Masina masina)
    {
        masina = new Masina(string.Empty, string.Empty, 0, Culoare.Alb, Optiuni.Niciuna);
        var mesaje = new StringBuilder();
        bool valid = true;
        string nr = NrInmatriculareTextBox.Text.Trim();

        if (nr.Length < NrInmatriculareMinLength || nr.Length > NrInmatriculareMaxLength)
        {
            MarcheazaInvalid(NrInmatriculareLabel, mesaje, $"Numarul de inmatriculare trebuie sa aiba intre {NrInmatriculareMinLength} si {NrInmatriculareMaxLength} caractere.");
            valid = false;
        }
        else if (_viewModel.ExistaNrInmatriculare(nr, _nrInmatriculareEditare))
        {
            MarcheazaInvalid(NrInmatriculareLabel, mesaje, "Exista deja o masina cu acest numar.");
            valid = false;
        }

        if (MarcaComboBox.SelectedItem is not MarcaMasina marca)
        {
            MarcheazaInvalid(MarcaLabel, mesaje, "Alege marca masinii.");
            valid = false;
            marca = MarcaMasina.Dacia;
        }

        if (ModelComboBox.SelectedItem is not string model || string.IsNullOrWhiteSpace(model))
        {
            MarcheazaInvalid(ModelLabel, mesaje, "Alege modelul masinii.");
            valid = false;
            model = string.Empty;
        }

        if (AnComboBox.SelectedItem is not int an)
        {
            MarcheazaInvalid(AnLabel, mesaje, "Alege anul fabricatiei.");
            valid = false;
            an = DateTime.Now.Year;
        }

        if (!CitesteDouble(KilometrajTextBox.Text, out double kilometraj) ||
            kilometraj < KilometrajMinim ||
            kilometraj > KilometrajMaxim)
        {
            MarcheazaInvalid(KilometrajLabel, mesaje, $"Kilometrajul trebuie sa fie intre {KilometrajMinim:N0} si {KilometrajMaxim:N0}.");
            valid = false;
        }

        if (CuloareComboBox.SelectedItem is not Culoare culoare)
        {
            MarcheazaInvalid(CuloareLabel, mesaje, "Alege culoarea.");
            valid = false;
            culoare = Culoare.Alb;
        }

        if (CombustibilComboBox.SelectedItem is not CombustibilMasina combustibil)
        {
            MarcheazaInvalid(CombustibilLabel, mesaje, "Alege combustibilul.");
            valid = false;
            combustibil = CombustibilMasina.Diesel;
        }

        if (StatusMasinaComboBox.SelectedItem is not StatusMasina status)
        {
            MarcheazaInvalid(StatusMasinaLabel, mesaje, "Alege statusul.");
            valid = false;
            status = StatusMasina.Disponibila;
        }

        masina = new Masina(nr, marca, model, an, kilometraj, culoare, combustibil, status, CitesteOptiuni());

        if (valid)
        {
            return true;
        }

        AfiseazaMesajMasina(mesaje.ToString().TrimEnd(), esteEroare: true);
        return false;
    }

    private Optiuni CitesteOptiuni()
    {
        Optiuni optiuni = Optiuni.Niciuna;

        if (AerConditionatCheckBox.IsChecked == true) optiuni |= Optiuni.AerConditionat;
        if (NavigatieCheckBox.IsChecked == true) optiuni |= Optiuni.Navigatie;
        if (SenzoriParcareCheckBox.IsChecked == true) optiuni |= Optiuni.SenzoriParcare;
        if (ScauneIncalziteCheckBox.IsChecked == true) optiuni |= Optiuni.ScauneIncalzite;
        if (CutieAutomataCheckBox.IsChecked == true) optiuni |= Optiuni.CutieAutomata;
        if (CameraMarsarierCheckBox.IsChecked == true) optiuni |= Optiuni.CameraMarsarier;

        return optiuni;
    }

    private void IncarcaMasina(Masina masina)
    {
        _nrInmatriculareEditare = masina.NrInmatriculare;
        NrInmatriculareTextBox.Text = masina.NrInmatriculare;
        MarcaComboBox.SelectedItem = masina.Marca;
        ModelComboBox.ItemsSource = MainWindowViewModel.GetModelePentruMarca(masina.Marca);
        ModelComboBox.SelectedItem = masina.Model;
        AnComboBox.SelectedItem = masina.AnFabricatie;
        KilometrajTextBox.Text = masina.Kilometraj.ToString(CultureInfo.CurrentCulture);
        CuloareComboBox.SelectedItem = masina.Culoare;
        CombustibilComboBox.SelectedItem = masina.Combustibil;
        StatusMasinaComboBox.SelectedItem = masina.Status;
        AerConditionatCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.AerConditionat);
        NavigatieCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.Navigatie);
        SenzoriParcareCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.SenzoriParcare);
        ScauneIncalziteCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.ScauneIncalzite);
        CutieAutomataCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.CutieAutomata);
        CameraMarsarierCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.CameraMarsarier);
        ResetValidareMasina();
    }

    private void CurataFormularMasina()
    {
        _nrInmatriculareEditare = null;
        MasiniDataGrid.SelectedIndex = -1;
        NrInmatriculareTextBox.Clear();
        MarcaComboBox.SelectedIndex = 0;
        ModelComboBox.ItemsSource = MainWindowViewModel.GetModelePentruMarca(MarcaMasina.Dacia);
        ModelComboBox.SelectedIndex = 0;
        AnComboBox.SelectedItem = DateTime.Now.Year;
        KilometrajTextBox.Text = "0";
        CuloareComboBox.SelectedItem = Culoare.Alb;
        CombustibilComboBox.SelectedItem = CombustibilMasina.Diesel;
        StatusMasinaComboBox.SelectedItem = StatusMasina.Disponibila;
        AerConditionatCheckBox.IsChecked = true;
        NavigatieCheckBox.IsChecked = false;
        SenzoriParcareCheckBox.IsChecked = false;
        ScauneIncalziteCheckBox.IsChecked = false;
        CutieAutomataCheckBox.IsChecked = false;
        CameraMarsarierCheckBox.IsChecked = false;
    }

    private void SalveazaSofer_Click(object sender, RoutedEventArgs e)
    {
        ResetValidareSofer();

        if (!ValideazaSofer(out Sofer sofer))
        {
            return;
        }

        if (_soferIdEditare == null)
        {
            _viewModel.AdaugaSofer(sofer);
            CurataFormularSofer();
            AfiseazaMesajSofer("Soferul a fost adaugat.", esteEroare: false);
            AfiseazaToast("Soferul a fost adaugat.");
            return;
        }

        bool actualizat = _viewModel.ActualizeazaSofer(_soferIdEditare.Value, sofer);
        AfiseazaMesajSofer(actualizat ? "Modificarile au fost salvate." : "Soferul nu a fost gasit.", esteEroare: !actualizat);
        if (actualizat)
        {
            AfiseazaToast("Soferul a fost actualizat.");
        }
    }

    private void StergeSofer_Click(object sender, RoutedEventArgs e)
    {
        if (_soferIdEditare == null)
        {
            AfiseazaMesajSofer("Selecteaza un sofer din tabel inainte de stergere.", esteEroare: true);
            return;
        }

        if (!ConfirmaStergere("Stergi soferul selectat?"))
        {
            return;
        }

        bool sters = _viewModel.StergeSofer(_soferIdEditare.Value);
        if (sters)
        {
            CurataFormularSofer();
            AfiseazaToast("Soferul a fost sters.");
        }

        AfiseazaMesajSofer(
            sters ? "Soferul a fost sters." : "Soferul nu poate fi sters: nu exista sau este folosit intr-o cursa.",
            esteEroare: !sters);
    }

    private void SoferNou_Click(object sender, RoutedEventArgs e)
    {
        CurataFormularSofer();
        ResetValidareSofer();
    }

    private void SoferDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SoferDataGrid.SelectedItem is Sofer sofer)
        {
            IncarcaSofer(sofer);
        }
    }

    private void EditeazaSoferContext_Click(object sender, RoutedEventArgs e)
    {
        if (SoferDataGrid.SelectedItem is not Sofer sofer)
        {
            AfiseazaToast("Selecteaza un sofer pentru editare.");
            return;
        }

        IncarcaSofer(sofer);
        MainTabControl.SelectedItem = SoferiTab;
        SoferNumeTextBox.Focus();
        SoferNumeTextBox.SelectAll();
        AfiseazaToast("Soferul este pregatit pentru editare.");
    }

    private void StergeSoferContext_Click(object sender, RoutedEventArgs e)
    {
        StergeSofer_Click(sender, e);
    }

    private void SoferFiltru_Changed(object sender, EventArgs e)
    {
        if (!_initializareCompleta)
        {
            return;
        }

        StatusSofer? status = SoferStatusFilterComboBox.SelectedItem is StatusSofer statusSelectat ? statusSelectat : null;
        _viewModel.FiltreazaSoferi(SoferSearchTextBox.Text, status);
    }

    private void ResetFiltreSoferi_Click(object sender, RoutedEventArgs e)
    {
        SoferSearchTextBox.Clear();
        SoferStatusFilterComboBox.SelectedIndex = -1;
        _viewModel.ResetFiltreSoferi();
    }

    private void SoferCamp_Changed(object sender, EventArgs e)
    {
        ResetValidareSofer();
    }

    private bool ValideazaSofer(out Sofer sofer)
    {
        sofer = new Sofer(0, string.Empty);
        var mesaje = new StringBuilder();
        bool valid = true;

        if (!int.TryParse(SoferIdTextBox.Text, out int id) || id < SoferIdMinim)
        {
            MarcheazaInvalid(SoferIdLabel, mesaje, $"ID-ul trebuie sa fie >= {SoferIdMinim}.");
            valid = false;
        }
        else if (_soferIdEditare == null && _viewModel.ExistaSofer(id))
        {
            MarcheazaInvalid(SoferIdLabel, mesaje, "Exista deja un sofer cu acest ID.");
            valid = false;
        }

        string nume = SoferNumeTextBox.Text.Trim();
        if (nume.Length < SoferNumeMinLength || nume.Length > SoferNumeMaxLength)
        {
            MarcheazaInvalid(SoferNumeLabel, mesaje, $"Numele trebuie sa aiba intre {SoferNumeMinLength} si {SoferNumeMaxLength} caractere.");
            valid = false;
        }

        string telefon = SoferTelefonTextBox.Text.Trim();
        if (telefon.Length < SoferTelefonMinLength || telefon.Length > SoferTelefonMaxLength)
        {
            MarcheazaInvalid(SoferTelefonLabel, mesaje, $"Telefonul trebuie sa aiba intre {SoferTelefonMinLength} si {SoferTelefonMaxLength} caractere.");
            valid = false;
        }

        if (SoferPermisComboBox.SelectedItem is not CategoriePermis permis)
        {
            MarcheazaInvalid(SoferPermisLabel, mesaje, "Alege categoria permisului.");
            valid = false;
            permis = CategoriePermis.B;
        }

        if (SoferStatusComboBox.SelectedItem is not StatusSofer status)
        {
            MarcheazaInvalid(SoferStatusLabel, mesaje, "Alege statusul soferului.");
            valid = false;
            status = StatusSofer.Disponibil;
        }

        if (!CitesteDouble(SoferKmTextBox.Text, out double kilometri) ||
            kilometri < SoferKmMinim ||
            kilometri > SoferKmMaxim)
        {
            MarcheazaInvalid(SoferKmLabel, mesaje, $"Kilometrii trebuie sa fie intre {SoferKmMinim:N0} si {SoferKmMaxim:N0}.");
            valid = false;
        }

        sofer = new Sofer(id, nume, telefon, permis, status)
        {
            TotalKilometriParcursi = kilometri
        };

        if (valid)
        {
            return true;
        }

        AfiseazaMesajSofer(mesaje.ToString().TrimEnd(), esteEroare: true);
        return false;
    }

    private void IncarcaSofer(Sofer sofer)
    {
        _soferIdEditare = sofer.Id;
        SoferIdTextBox.Text = sofer.Id.ToString(CultureInfo.CurrentCulture);
        SoferNumeTextBox.Text = sofer.Nume;
        SoferTelefonTextBox.Text = sofer.Telefon;
        SoferPermisComboBox.SelectedItem = sofer.CategoriePermis;
        SoferStatusComboBox.SelectedItem = sofer.Status;
        SoferKmTextBox.Text = sofer.TotalKilometriParcursi.ToString(CultureInfo.CurrentCulture);
        ResetValidareSofer();
    }

    private void CurataFormularSofer()
    {
        _soferIdEditare = null;
        SoferDataGrid.SelectedIndex = -1;
        SoferIdTextBox.Clear();
        SoferNumeTextBox.Clear();
        SoferTelefonTextBox.Clear();
        SoferPermisComboBox.SelectedItem = CategoriePermis.B;
        SoferStatusComboBox.SelectedItem = StatusSofer.Disponibil;
        SoferKmTextBox.Text = "0";
    }

    private void SalveazaCursa_Click(object sender, RoutedEventArgs e)
    {
        ResetValidareCursa();

        if (!ValideazaCursa(out Cursa cursa))
        {
            return;
        }

        if (_cursaIdEditare == null)
        {
            _viewModel.AdaugaCursa(cursa);
            CurataFormularCursa();
            AfiseazaMesajCursa("Cursa a fost adaugata.", esteEroare: false);
            AfiseazaToast("Cursa a fost adaugata.");
            return;
        }

        bool actualizat = _viewModel.ActualizeazaCursa(_cursaIdEditare.Value, cursa);
        _cursaIdEditare = actualizat ? cursa.Id : _cursaIdEditare;
        AfiseazaMesajCursa(actualizat ? "Modificarile au fost salvate." : "Cursa nu a fost gasita.", esteEroare: !actualizat);
        if (actualizat)
        {
            AfiseazaToast("Cursa a fost actualizata.");
        }
    }

    private void StergeCursa_Click(object sender, RoutedEventArgs e)
    {
        if (_cursaIdEditare == null)
        {
            AfiseazaMesajCursa("Selecteaza o cursa din tabel inainte de stergere.", esteEroare: true);
            return;
        }

        if (!ConfirmaStergere("Stergi cursa selectata?"))
        {
            return;
        }

        bool sters = _viewModel.StergeCursa(_cursaIdEditare.Value);
        if (sters)
        {
            CurataFormularCursa();
            AfiseazaToast("Cursa a fost stearsa.");
        }

        AfiseazaMesajCursa(sters ? "Cursa a fost stearsa." : "Cursa nu a fost gasita.", esteEroare: !sters);
    }

    private void CursaNoua_Click(object sender, RoutedEventArgs e)
    {
        CurataFormularCursa();
        ResetValidareCursa();
    }

    private void CurseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CurseDataGrid.SelectedItem is Cursa cursa)
        {
            _viewModel.CursaSelectata = cursa;
            CurseQuickListBox.SelectedItem = cursa;
            IncarcaCursa(cursa);
        }
    }

    private void CurseDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DeschideDetaliiCursaSelectata();
    }

    private void CurseQuickListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CurseQuickListBox.SelectedItem is Cursa cursa)
        {
            _viewModel.CursaSelectata = cursa;
            CurseDataGrid.SelectedItem = cursa;
            IncarcaCursa(cursa);
        }
    }

    private void EditeazaCursaContext_Click(object sender, RoutedEventArgs e)
    {
        if (CurseDataGrid.SelectedItem is not Cursa cursa)
        {
            AfiseazaToast("Selecteaza o cursa pentru editare.");
            return;
        }

        _viewModel.CursaSelectata = cursa;
        IncarcaCursa(cursa);
        MainTabControl.SelectedItem = CurseTab;
        ClientTextBox.Focus();
        ClientTextBox.SelectAll();
        AfiseazaToast("Cursa este pregatita pentru editare.");
    }

    private void StergeCursaContext_Click(object sender, RoutedEventArgs e)
    {
        StergeCursa_Click(sender, e);
    }

    private void VeziDetaliiCursaContext_Click(object sender, RoutedEventArgs e)
    {
        DeschideDetaliiCursaSelectata();
    }

    private void CursaFiltru_Changed(object sender, EventArgs e)
    {
        if (!_initializareCompleta)
        {
            return;
        }

        StatusCursa? status = CursaStatusFilterComboBox.SelectedItem is StatusCursa statusSelectat ? statusSelectat : null;
        TipCursa? tip = CursaTipFilterComboBox.SelectedItem is TipCursa tipSelectat ? tipSelectat : null;
        _viewModel.FiltreazaCurse(CursaSearchTextBox.Text, status, tip);
    }

    private void ResetFiltreCurse_Click(object sender, RoutedEventArgs e)
    {
        CursaSearchTextBox.Clear();
        CursaStatusFilterComboBox.SelectedIndex = -1;
        CursaTipFilterComboBox.SelectedIndex = -1;
        _viewModel.ResetFiltreCurse();
        SelecteazaPrimaCursaDisponibila();
    }

    private void DataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        DataGridRow? row = GasesteParinte<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row == null)
        {
            e.Handled = true;
            return;
        }

        row.Focus();
        dataGrid.SelectedItem = row.Item;
    }

    private void CursaDetalii_Click(object sender, RoutedEventArgs e)
    {
        DeschideDetaliiCursaSelectata();
    }

    private void CursaCamp_Changed(object sender, EventArgs e)
    {
        if (_actualizareProgramatica)
        {
            return;
        }

        ResetValidareCursa();
        ActualizeazaCostEstimativ();
        ActualizeazaDisponibilitateCursa();
    }

    private bool ValideazaCursa(out Cursa cursa)
    {
        cursa = CreeazaCursaGoala();
        var mesaje = new StringBuilder();
        bool valid = true;

        if (!int.TryParse(CursaIdTextBox.Text, out int id) || id < CursaIdMinim)
        {
            MarcheazaInvalid(CursaIdLabel, mesaje, $"ID-ul cursei trebuie sa fie >= {CursaIdMinim}.");
            valid = false;
        }
        else if (_viewModel.ExistaCursa(id, _cursaIdEditare))
        {
            MarcheazaInvalid(CursaIdLabel, mesaje, "Exista deja o cursa cu acest ID.");
            valid = false;
        }

        string client = ClientTextBox.Text.Trim();
        if (client.Length < CursaTextMinLength || client.Length > CursaTextMaxLength)
        {
            MarcheazaInvalid(ClientLabel, mesaje, $"Clientul trebuie sa aiba intre {CursaTextMinLength} si {CursaTextMaxLength} caractere.");
            valid = false;
        }

        string marfa = MarfaTextBox.Text.Trim();
        if (marfa.Length < CursaTextMinLength || marfa.Length > CursaTextMaxLength)
        {
            MarcheazaInvalid(MarfaLabel, mesaje, $"Marfa trebuie sa aiba intre {CursaTextMinLength} si {CursaTextMaxLength} caractere.");
            valid = false;
        }

        string plecare = LocPlecareTextBox.Text.Trim();
        if (plecare.Length < CursaTextMinLength || plecare.Length > CursaTextMaxLength)
        {
            MarcheazaInvalid(LocPlecareLabel, mesaje, $"Plecare trebuie sa aiba intre {CursaTextMinLength} si {CursaTextMaxLength} caractere.");
            valid = false;
        }

        string destinatie = DestinatieTextBox.Text.Trim();
        if (destinatie.Length < CursaTextMinLength || destinatie.Length > CursaTextMaxLength)
        {
            MarcheazaInvalid(DestinatieLabel, mesaje, $"Destinatia trebuie sa aiba intre {CursaTextMinLength} si {CursaTextMaxLength} caractere.");
            valid = false;
        }

        if (!CitesteDataOra(DataPlecareDatePicker, OraPlecareTextBox, out DateTime dataPlecare))
        {
            MarcheazaInvalid(DataPlecareLabel, mesaje, "Alege data plecarii si ora in format HH:mm.");
            MarcheazaInvalid(OraPlecareLabel, mesaje, "Ora de plecare nu este valida.");
            valid = false;
        }

        if (!CitesteDataOra(DataSosireDatePicker, OraSosireTextBox, out DateTime dataSosire))
        {
            MarcheazaInvalid(DataSosireLabel, mesaje, "Alege data sosirii si ora in format HH:mm.");
            MarcheazaInvalid(OraSosireLabel, mesaje, "Ora de sosire nu este valida.");
            valid = false;
        }

        if (valid && dataSosire <= dataPlecare)
        {
            MarcheazaInvalid(DataSosireLabel, mesaje, "Data sosirii trebuie sa fie dupa data plecarii.");
            valid = false;
        }

        if (CursaMasinaComboBox.SelectedItem is not Masina masina)
        {
            MarcheazaInvalid(CursaMasinaLabel, mesaje, "Alege masina pentru cursa.");
            valid = false;
            masina = CreeazaCursaGoala().MasinaAlocata;
        }

        if (CursaSoferComboBox.SelectedItem is not Sofer sofer)
        {
            MarcheazaInvalid(CursaSoferLabel, mesaje, "Alege soferul pentru cursa.");
            valid = false;
            sofer = CreeazaCursaGoala().SoferAlocat;
        }

        if (CursaStatusComboBox.SelectedItem is not StatusCursa status)
        {
            MarcheazaInvalid(CursaStatusLabel, mesaje, "Alege statusul cursei.");
            valid = false;
            status = StatusCursa.Planificata;
        }

        TipCursa tip = CursaInternationalaRadioButton.IsChecked == true
            ? TipCursa.Internationala
            : TipCursa.Interna;

        if (PrioritateCursaComboBox.SelectedItem is not PrioritateCursa prioritate)
        {
            MarcheazaInvalid(PrioritateLabel, mesaje, "Alege prioritatea cursei.");
            valid = false;
            prioritate = PrioritateCursa.Normala;
        }

        if (!CitesteDouble(DistantaTextBox.Text, out double distanta) ||
            distanta < DistantaMinima ||
            distanta > DistantaMaxima)
        {
            MarcheazaInvalid(DistantaLabel, mesaje, $"Distanta trebuie sa fie intre {DistantaMinima:N0} si {DistantaMaxima:N0} km.");
            valid = false;
        }

        if (!CitesteDecimal(PretKmTextBox.Text, out decimal pretKm) ||
            pretKm < PretKmMinim ||
            pretKm > PretKmMaxim)
        {
            MarcheazaInvalid(PretKmLabel, mesaje, $"Pretul pe km trebuie sa fie intre {PretKmMinim:N1} si {PretKmMaxim:N0}.");
            valid = false;
        }

        if (!CitesteDecimal(CostTextBox.Text, out decimal cost) ||
            cost < CostMinim ||
            cost > CostMaxim)
        {
            MarcheazaInvalid(CostLabel, mesaje, $"Costul trebuie sa fie intre {CostMinim:N0} si {CostMaxim:N0}.");
            valid = false;
        }

        if (valid && status != StatusCursa.Anulata)
        {
            if (!_viewModel.EsteMasinaDisponibila(masina, dataPlecare, dataSosire, _cursaIdEditare))
            {
                MarcheazaInvalid(CursaMasinaLabel, mesaje, "Masina este deja alocata intr-o cursa care se suprapune.");
                valid = false;
            }

            if (!_viewModel.EsteSoferDisponibil(sofer, dataPlecare, dataSosire, _cursaIdEditare))
            {
                MarcheazaInvalid(CursaSoferLabel, mesaje, "Soferul este deja alocat intr-o cursa care se suprapune.");
                valid = false;
            }
        }

        cursa = new Cursa(
            id,
            client,
            marfa,
            plecare,
            destinatie,
            dataPlecare,
            dataSosire,
            masina,
            sofer,
            tip,
            status,
            prioritate,
            distanta,
            pretKm,
            cost,
            ObservatiiTextBox.Text.Trim());

        if (valid)
        {
            return true;
        }

        AfiseazaMesajCursa(mesaje.ToString().TrimEnd(), esteEroare: true);
        return false;
    }

    private void IncarcaCursa(Cursa cursa)
    {
        _actualizareProgramatica = true;
        _cursaIdEditare = cursa.Id;
        CursaIdTextBox.Text = cursa.Id.ToString(CultureInfo.CurrentCulture);
        ClientTextBox.Text = cursa.Client;
        MarfaTextBox.Text = cursa.Marfa;
        LocPlecareTextBox.Text = cursa.LocPlecare;
        DestinatieTextBox.Text = cursa.Destinatie;
        DataPlecareDatePicker.SelectedDate = cursa.DataPlecare.Date;
        OraPlecareTextBox.Text = cursa.DataPlecare.ToString("HH:mm", CultureInfo.InvariantCulture);
        DataSosireDatePicker.SelectedDate = cursa.DataSosire.Date;
        OraSosireTextBox.Text = cursa.DataSosire.ToString("HH:mm", CultureInfo.InvariantCulture);
        _viewModel.ActualizeazaSelectiiCurse(cursa.DataPlecare, cursa.DataSosire, cursa.Id, cursa.MasinaAlocata, cursa.SoferAlocat);
        CursaMasinaComboBox.SelectedItem = cursa.MasinaAlocata;
        CursaSoferComboBox.SelectedItem = cursa.SoferAlocat;
        CursaStatusComboBox.SelectedItem = cursa.Status;
        CursaInternaRadioButton.IsChecked = cursa.Tip == TipCursa.Interna;
        CursaInternationalaRadioButton.IsChecked = cursa.Tip == TipCursa.Internationala;
        PrioritateCursaComboBox.SelectedItem = cursa.Prioritate;
        DistantaTextBox.Text = cursa.DistantaKm.ToString(CultureInfo.CurrentCulture);
        PretKmTextBox.Text = cursa.PretPerKm.ToString(CultureInfo.CurrentCulture);
        CostTextBox.Text = cursa.CostEstimativ.ToString(CultureInfo.CurrentCulture);
        ObservatiiTextBox.Text = cursa.Observatii;
        _actualizareProgramatica = false;
        ActualizeazaDisponibilitateCursa();
        ResetValidareCursa();
    }

    private void CurataFormularCursa()
    {
        _actualizareProgramatica = true;
        _cursaIdEditare = null;
        CurseDataGrid.SelectedIndex = -1;
        CurseQuickListBox.SelectedIndex = -1;
        CursaIdTextBox.Text = _viewModel.GenereazaIdCursa().ToString(CultureInfo.CurrentCulture);
        ClientTextBox.Clear();
        MarfaTextBox.Clear();
        LocPlecareTextBox.Clear();
        DestinatieTextBox.Clear();
        DateTime acum = DateTime.Now;
        DataPlecareDatePicker.SelectedDate = acum.Date;
        OraPlecareTextBox.Text = acum.AddMinutes(30).ToString("HH:mm", CultureInfo.InvariantCulture);
        DataSosireDatePicker.SelectedDate = acum.Date;
        OraSosireTextBox.Text = acum.AddHours(2).ToString("HH:mm", CultureInfo.InvariantCulture);
        _viewModel.ActualizeazaSelectiiCurse(acum.AddMinutes(30), acum.AddHours(2));
        CursaMasinaComboBox.SelectedIndex = _viewModel.MasiniPentruSelectie.Count > 0 ? 0 : -1;
        CursaSoferComboBox.SelectedIndex = _viewModel.SoferiPentruSelectie.Count > 0 ? 0 : -1;
        CursaStatusComboBox.SelectedItem = StatusCursa.Planificata;
        CursaInternaRadioButton.IsChecked = true;
        PrioritateCursaComboBox.SelectedItem = PrioritateCursa.Normala;
        DistantaTextBox.Text = "100";
        PretKmTextBox.Text = "4.5";
        CostTextBox.Text = "0";
        ObservatiiTextBox.Clear();
        _actualizareProgramatica = false;
        ActualizeazaCostEstimativ();
        ActualizeazaDisponibilitateCursa();
    }

    private static Cursa CreeazaCursaGoala()
    {
        var masina = new Masina(string.Empty, string.Empty, 0, Culoare.Alb, Optiuni.Niciuna);
        var sofer = new Sofer(0, string.Empty);
        return new Cursa(0, string.Empty, string.Empty, DateTime.Now, DateTime.Now.AddHours(1), masina, sofer, TipCursa.Interna, StatusCursa.Planificata, 0, 0);
    }

    private void ActualizeazaCostEstimativ()
    {
        if (!CitesteDouble(DistantaTextBox.Text, out double distanta) ||
            !CitesteDecimal(PretKmTextBox.Text, out decimal pretKm))
        {
            return;
        }

        decimal factor = 1m;
        if (CursaInternationalaRadioButton.IsChecked == true)
        {
            factor += 0.18m;
        }

        if (PrioritateCursaComboBox.SelectedItem is PrioritateCursa.Rapida)
        {
            factor += 0.12m;
        }
        else if (PrioritateCursaComboBox.SelectedItem is PrioritateCursa.Urgenta)
        {
            factor += 0.25m;
        }

        decimal cost = Math.Round((decimal)distanta * pretKm * factor, 2);
        if (cost < CostMinim || cost > CostMaxim)
        {
            return;
        }

        _actualizareProgramatica = true;
        CostTextBox.Text = cost.ToString("N2", CultureInfo.CurrentCulture);
        _actualizareProgramatica = false;
    }

    private void ActualizeazaDisponibilitateCursa()
    {
        if (CursaAvailabilityTextBlock == null)
        {
            return;
        }

        if (!CitesteDataOra(DataPlecareDatePicker, OraPlecareTextBox, out DateTime plecare) ||
            !CitesteDataOra(DataSosireDatePicker, OraSosireTextBox, out DateTime sosire) ||
            sosire <= plecare)
        {
            CursaAvailabilityTextBlock.Text = "Alege un interval valid ca sa vezi disponibilitatea in timp real.";
            return;
        }

        Masina? masinaCurenta = CursaMasinaComboBox.SelectedItem as Masina;
        Sofer? soferCurent = CursaSoferComboBox.SelectedItem as Sofer;

        _actualizareProgramatica = true;
        _viewModel.ActualizeazaSelectiiCurse(plecare, sosire, _cursaIdEditare, masinaCurenta, soferCurent);

        if (masinaCurenta != null && _viewModel.MasiniPentruSelectie.Contains(masinaCurenta))
        {
            CursaMasinaComboBox.SelectedItem = masinaCurenta;
        }
        else
        {
            CursaMasinaComboBox.SelectedIndex = _viewModel.MasiniPentruSelectie.Count > 0 ? 0 : -1;
        }

        if (soferCurent != null && _viewModel.SoferiPentruSelectie.Contains(soferCurent))
        {
            CursaSoferComboBox.SelectedItem = soferCurent;
        }
        else
        {
            CursaSoferComboBox.SelectedIndex = _viewModel.SoferiPentruSelectie.Count > 0 ? 0 : -1;
        }

        _actualizareProgramatica = false;
        CursaAvailabilityTextBlock.Text =
            $"{_viewModel.MasiniPentruSelectie.Count} masini si {_viewModel.SoferiPentruSelectie.Count} soferi disponibili pentru intervalul selectat.";
    }

    private bool ConfirmaStergere(string mesaj)
    {
        return MessageBox.Show(
            mesaj,
            "Confirmare stergere",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    private void AfiseazaToast(string mesaj)
    {
        ToastTextBlock.Text = mesaj;
        ToastPanel.Visibility = Visibility.Visible;

        _toastTimer?.Stop();
        _toastTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2.7)
        };
        _toastTimer.Tick += (_, _) =>
        {
            ToastPanel.Visibility = Visibility.Collapsed;
            _toastTimer?.Stop();
        };
        _toastTimer.Start();
    }

    private static T? GasesteParinte<T>(DependencyObject? element)
        where T : DependencyObject
    {
        while (element != null)
        {
            if (element is T match)
            {
                return match;
            }

            element = VisualTreeHelper.GetParent(element);
        }

        return null;
    }

    private static bool CitesteDouble(string text, out double valoare)
    {
        return double.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out valoare) ||
               double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out valoare);
    }

    private static bool CitesteDecimal(string text, out decimal valoare)
    {
        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out valoare) ||
               decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out valoare);
    }

    private static bool CitesteDataOra(DatePicker datePicker, TextBox oraTextBox, out DateTime dataOra)
    {
        dataOra = default;
        if (datePicker.SelectedDate == null)
        {
            return false;
        }

        if (!TimeSpan.TryParseExact(oraTextBox.Text.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan ora) &&
            !TimeSpan.TryParse(oraTextBox.Text.Trim(), CultureInfo.CurrentCulture, out ora))
        {
            return false;
        }

        dataOra = datePicker.SelectedDate.Value.Date.Add(ora);
        return true;
    }

    private void MarcheazaInvalid(TextBlock label, StringBuilder mesaje, string mesaj)
    {
        label.Foreground = _labelInvalidBrush;
        mesaje.AppendLine(mesaj);
    }

    private void AfiseazaMesajMasina(string mesaj, bool esteEroare)
    {
        AfiseazaMesaj(MasinaMessagePanel, MasinaMessageTextBlock, mesaj, esteEroare);
    }

    private void AfiseazaMesajSofer(string mesaj, bool esteEroare)
    {
        AfiseazaMesaj(SoferMessagePanel, SoferMessageTextBlock, mesaj, esteEroare);
    }

    private void AfiseazaMesajCursa(string mesaj, bool esteEroare)
    {
        AfiseazaMesaj(CursaMessagePanel, CursaMessageTextBlock, mesaj, esteEroare);
    }

    private void AfiseazaMesaj(Border panel, TextBlock textBlock, string mesaj, bool esteEroare)
    {
        textBlock.Text = mesaj;
        textBlock.Foreground = esteEroare
            ? _labelInvalidBrush
            : new SolidColorBrush(Color.FromRgb(23, 107, 91));
        panel.Background = esteEroare
            ? new SolidColorBrush(Color.FromRgb(255, 241, 241))
            : new SolidColorBrush(Color.FromRgb(237, 245, 242));
        panel.BorderBrush = esteEroare
            ? new SolidColorBrush(Color.FromRgb(240, 184, 184))
            : new SolidColorBrush(Color.FromRgb(207, 225, 219));
        panel.Visibility = Visibility.Visible;
    }

    private void ResetValidareMasina()
    {
        if (NrInmatriculareLabel == null)
        {
            return;
        }

        foreach (TextBlock label in new[] { NrInmatriculareLabel, MarcaLabel, ModelLabel, AnLabel, KilometrajLabel, CuloareLabel, CombustibilLabel, StatusMasinaLabel })
        {
            label.Foreground = _labelNormalBrush;
        }

        MasinaMessagePanel.Visibility = Visibility.Collapsed;
        MasinaMessageTextBlock.Text = string.Empty;
    }

    private void ResetValidareSofer()
    {
        if (SoferIdLabel == null)
        {
            return;
        }

        foreach (TextBlock label in new[] { SoferIdLabel, SoferNumeLabel, SoferTelefonLabel, SoferPermisLabel, SoferStatusLabel, SoferKmLabel })
        {
            label.Foreground = _labelNormalBrush;
        }

        SoferMessagePanel.Visibility = Visibility.Collapsed;
        SoferMessageTextBlock.Text = string.Empty;
    }

    private void ResetValidareCursa()
    {
        if (CursaIdLabel == null)
        {
            return;
        }

        foreach (TextBlock label in new[]
                 {
                     CursaIdLabel, CursaStatusLabel, ClientLabel, MarfaLabel, LocPlecareLabel, DestinatieLabel, DataPlecareLabel,
                     OraPlecareLabel, DataSosireLabel, OraSosireLabel, CursaMasinaLabel, CursaSoferLabel,
                     CursaTipLabel, PrioritateLabel, DistantaLabel, PretKmLabel, CostLabel, ObservatiiLabel
                 })
        {
            label.Foreground = _labelNormalBrush;
        }

        CursaMessagePanel.Visibility = Visibility.Collapsed;
        CursaMessageTextBlock.Text = string.Empty;
    }

    private void SelecteazaPrimaCursaDisponibila()
    {
        if (CurseDataGrid.Items.Count > 0)
        {
            CurseDataGrid.SelectedIndex = 0;
        }
        else
        {
            _viewModel.CursaSelectata = null;
        }
    }

    private void DashboardMenu_Click(object sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedItem = DashboardTab;
    }

    private void MasiniMenu_Click(object sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedItem = MasiniTab;
    }

    private void SoferiMenu_Click(object sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedItem = SoferiTab;
    }

    private void CurseMenu_Click(object sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedItem = CurseTab;
    }

    private void DeschideDetaliiCursaSelectata()
    {
        Cursa? cursa = CurseDataGrid.SelectedItem as Cursa ?? CurseQuickListBox.SelectedItem as Cursa;
        if (cursa == null)
        {
            AfiseazaToast("Selecteaza o cursa pentru detalii.");
            MainTabControl.SelectedItem = CurseTab;
            return;
        }

        var window = new CursaDetailsWindow(cursa)
        {
            Owner = this
        };
        window.ShowDialog();
    }

    private void Iesire_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
