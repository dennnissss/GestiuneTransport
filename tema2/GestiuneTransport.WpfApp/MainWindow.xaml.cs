using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GestiuneTransport.Models;

namespace GestiuneTransport.WpfApp;

public partial class MainWindow : Window
{
    private const int NrInmatriculareMinLength = 5;
    private const int NrInmatriculareMaxLength = 12;
    private const int ModelMinLength = 2;
    private const int ModelMaxLength = 40;
    private const double KilometrajMinim = 0;
    private const double KilometrajMaxim = 2_000_000;
    private const int SoferIdMinim = 1;
    private const int SoferNumeMinLength = 2;
    private const int SoferNumeMaxLength = 50;
    private const double SoferKmMinim = 0;
    private const double SoferKmMaxim = 5_000_000;

    private readonly Brush _labelNormalBrush = new SolidColorBrush(Color.FromRgb(94, 107, 104));
    private readonly Brush _labelInvalidBrush = new SolidColorBrush(Color.FromRgb(180, 45, 45));
    private readonly MainWindowViewModel _viewModel = new();
    private ModFormular _modCurent = ModFormular.Adaugare;
    private string? _nrInmatriculareEditare;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;

        CuloareComboBox.ItemsSource = Enum.GetValues<Culoare>();
        CuloareComboBox.SelectedIndex = -1;
        SetMod(ModFormular.Adaugare);
    }

    private void SalveazaMasina_Click(object sender, RoutedEventArgs e)
    {
        ResetValidare();

        if (!ValideazaFormular(out Masina masina))
        {
            return;
        }

        if (_modCurent == ModFormular.Adaugare)
        {
            _viewModel.AdaugaMasina(masina);
            ReseteazaFormular();
            AfiseazaMesaj("Masina a fost adaugata cu succes.", esteEroare: false);
            return;
        }

        if (string.IsNullOrWhiteSpace(_nrInmatriculareEditare))
        {
            AfiseazaMesaj("Selectati o masina din lista pentru editare.", esteEroare: true);
            return;
        }

        bool actualizat = _viewModel.ActualizeazaMasina(_nrInmatriculareEditare, masina);
        AfiseazaMesaj(
            actualizat ? "Masina a fost actualizata cu succes." : "Masina selectata nu a fost gasita.",
            esteEroare: !actualizat);
    }

    private void Cauta_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.CautaDupaNrInmatriculare(SearchTextBox.Text);
        AfiseazaMesaj("Cautarea dupa numar de inmatriculare a fost aplicata.", esteEroare: false);
    }

    private void ResetCautare_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Clear();
        _viewModel.ResetCautare();
        ResetValidare();
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        ReseteazaFormular();
        ResetValidare();
        SetMod(ModFormular.Adaugare);
    }

    private void SetAdaugare_Click(object sender, RoutedEventArgs e)
    {
        SetMod(ModFormular.Adaugare);
    }

    private void SetEditare_Click(object sender, RoutedEventArgs e)
    {
        SetMod(ModFormular.Editare);
    }

    private void Iesire_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ModOperare_Checked(object sender, RoutedEventArgs e)
    {
        if (AdaugareRadioButton == null || EditareRadioButton == null)
        {
            return;
        }

        SetMod(AdaugareRadioButton.IsChecked == true ? ModFormular.Adaugare : ModFormular.Editare);
    }

    private void MasiniDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_modCurent == ModFormular.Editare && MasiniDataGrid.SelectedItem is Masina masina)
        {
            MasiniListBox.SelectedItem = masina;
            IncarcaMasinaInFormular(masina);
        }
    }

    private void MasiniListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MasiniListBox.SelectedItem is not Masina masina)
        {
            return;
        }

        MasiniDataGrid.SelectedItem = masina;

        if (_modCurent == ModFormular.Editare)
        {
            IncarcaMasinaInFormular(masina);
        }
    }

    private void Camp_TextChanged(object sender, TextChangedEventArgs e)
    {
        ResetValidare();
    }

    private void Camp_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ResetValidare();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
        {
            _viewModel.ResetCautare();
        }
    }

    private bool ValideazaFormular(out Masina masina)
    {
        string nrInmatriculare = NrInmatriculareTextBox.Text.Trim();
        string model = ModelTextBox.Text.Trim();
        var mesaje = new StringBuilder();

        bool dateValide = true;

        if (nrInmatriculare.Length < NrInmatriculareMinLength ||
            nrInmatriculare.Length > NrInmatriculareMaxLength)
        {
            MarcheazaInvalid(
                NrInmatriculareLabel,
                mesaje,
                $"Numarul de inmatriculare trebuie sa aiba intre {NrInmatriculareMinLength} si {NrInmatriculareMaxLength} caractere.");
            dateValide = false;
        }

        if (_modCurent == ModFormular.Adaugare && _viewModel.ExistaNrInmatriculare(nrInmatriculare))
        {
            MarcheazaInvalid(NrInmatriculareLabel, mesaje, "Exista deja o masina cu acest numar de inmatriculare.");
            dateValide = false;
        }

        if (model.Length < ModelMinLength || model.Length > ModelMaxLength)
        {
            MarcheazaInvalid(
                ModelLabel,
                mesaje,
                $"Modelul trebuie sa aiba intre {ModelMinLength} si {ModelMaxLength} caractere.");
            dateValide = false;
        }

        if (!IncearcaCitireKilometraj(out double kilometraj) ||
            kilometraj < KilometrajMinim ||
            kilometraj > KilometrajMaxim)
        {
            MarcheazaInvalid(
                KilometrajLabel,
                mesaje,
                $"Kilometrajul trebuie sa fie un numar intre {KilometrajMinim:N0} si {KilometrajMaxim:N0}.");
            dateValide = false;
        }

        Culoare culoare = default;
        if (CuloareComboBox.SelectedItem is Culoare culoareSelectata)
        {
            culoare = culoareSelectata;
        }
        else
        {
            MarcheazaInvalid(CuloareLabel, mesaje, "Selectati o culoare pentru masina.");
            dateValide = false;
        }

        masina = new Masina(nrInmatriculare, model, kilometraj, culoare, CitesteOptiuniSelectate());

        if (dateValide)
        {
            return true;
        }

        ErrorTextBlock.Text = mesaje.ToString().TrimEnd();
        ErrorPanel.Visibility = Visibility.Visible;
        return false;
    }

    private bool IncearcaCitireKilometraj(out double kilometraj)
    {
        return double.TryParse(KilometrajTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out kilometraj) ||
               double.TryParse(KilometrajTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out kilometraj);
    }

    private Optiuni CitesteOptiuniSelectate()
    {
        Optiuni optiuni = Optiuni.Niciuna;

        if (AerConditionatCheckBox.IsChecked == true)
        {
            optiuni |= Optiuni.AerConditionat;
        }

        if (NavigatieCheckBox.IsChecked == true)
        {
            optiuni |= Optiuni.Navigatie;
        }

        if (SenzoriParcareCheckBox.IsChecked == true)
        {
            optiuni |= Optiuni.SenzoriParcare;
        }

        if (ScauneIncalziteCheckBox.IsChecked == true)
        {
            optiuni |= Optiuni.ScauneIncalzite;
        }

        return optiuni;
    }

    private void IncarcaMasinaInFormular(Masina masina)
    {
        _nrInmatriculareEditare = masina.NrInmatriculare;
        NrInmatriculareTextBox.Text = masina.NrInmatriculare;
        ModelTextBox.Text = masina.Model;
        KilometrajTextBox.Text = masina.Kilometraj.ToString(CultureInfo.CurrentCulture);
        CuloareComboBox.SelectedItem = masina.Culoare;
        AerConditionatCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.AerConditionat);
        NavigatieCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.Navigatie);
        SenzoriParcareCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.SenzoriParcare);
        ScauneIncalziteCheckBox.IsChecked = masina.Optiuni.HasFlag(Optiuni.ScauneIncalzite);
        ResetValidare();
    }

    private void SetMod(ModFormular mod)
    {
        _modCurent = mod;

        if (AdaugareRadioButton != null)
        {
            AdaugareRadioButton.IsChecked = mod == ModFormular.Adaugare;
            EditareRadioButton.IsChecked = mod == ModFormular.Editare;
        }

        if (SalveazaButton == null)
        {
            return;
        }

        if (mod == ModFormular.Adaugare)
        {
            FormTitleTextBlock.Text = "Date masina";
            SalveazaButton.Content = "Adauga masina";
            NrInmatriculareTextBox.IsEnabled = true;
            _nrInmatriculareEditare = null;
            return;
        }

        FormTitleTextBlock.Text = "Editare masina";
        SalveazaButton.Content = "Salveaza modificari";
        NrInmatriculareTextBox.IsEnabled = false;

        if (MasiniDataGrid.SelectedItem is Masina masina)
        {
            IncarcaMasinaInFormular(masina);
        }
        else
        {
            AfiseazaMesaj("Selectati o masina din lista pentru editare.", esteEroare: true);
        }
    }

    private void MarcheazaInvalid(TextBlock label, StringBuilder mesaje, string mesaj)
    {
        label.Foreground = _labelInvalidBrush;
        mesaje.AppendLine(mesaj);
    }

    private void AfiseazaMesaj(string mesaj, bool esteEroare)
    {
        ErrorTextBlock.Text = mesaj;
        ErrorTextBlock.Foreground = esteEroare
            ? _labelInvalidBrush
            : new SolidColorBrush(Color.FromRgb(23, 107, 91));
        ErrorPanel.Background = esteEroare
            ? new SolidColorBrush(Color.FromRgb(255, 241, 241))
            : new SolidColorBrush(Color.FromRgb(237, 245, 242));
        ErrorPanel.BorderBrush = esteEroare
            ? new SolidColorBrush(Color.FromRgb(240, 184, 184))
            : new SolidColorBrush(Color.FromRgb(207, 225, 219));
        ErrorPanel.Visibility = Visibility.Visible;
    }

    private void ResetValidare()
    {
        NrInmatriculareLabel.Foreground = _labelNormalBrush;
        ModelLabel.Foreground = _labelNormalBrush;
        KilometrajLabel.Foreground = _labelNormalBrush;
        CuloareLabel.Foreground = _labelNormalBrush;

        ErrorTextBlock.Foreground = _labelInvalidBrush;
        ErrorPanel.Background = new SolidColorBrush(Color.FromRgb(255, 241, 241));
        ErrorPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(240, 184, 184));
        ErrorPanel.Visibility = Visibility.Collapsed;
        ErrorTextBlock.Text = string.Empty;
    }

    private void ReseteazaFormular()
    {
        NrInmatriculareTextBox.Clear();
        ModelTextBox.Clear();
        KilometrajTextBox.Clear();
        CuloareComboBox.SelectedIndex = -1;
        AerConditionatCheckBox.IsChecked = false;
        NavigatieCheckBox.IsChecked = false;
        SenzoriParcareCheckBox.IsChecked = false;
        ScauneIncalziteCheckBox.IsChecked = false;
        _nrInmatriculareEditare = null;
    }

    private void AdaugaSofer_Click(object sender, RoutedEventArgs e)
    {
        ResetValidareSofer();

        if (!ValideazaFormularSofer(verificaIdUnic: true, out int id, out string nume, out double kilometri))
        {
            return;
        }

        var sofer = new Sofer(id, nume)
        {
            TotalKilometriParcursi = kilometri
        };

        _viewModel.AdaugaSofer(sofer);
        ReseteazaFormularSofer();
        AfiseazaMesajSofer("Soferul a fost adaugat cu succes.", esteEroare: false);
    }

    private void ActualizeazaSofer_Click(object sender, RoutedEventArgs e)
    {
        ResetValidareSofer();

        if (!ValideazaFormularSofer(verificaIdUnic: false, out int id, out string nume, out double kilometri))
        {
            return;
        }

        bool actualizat = _viewModel.ActualizeazaSofer(id, nume, kilometri);
        AfiseazaMesajSofer(
            actualizat ? "Soferul a fost actualizat cu succes." : "Soferul nu a fost gasit.",
            esteEroare: !actualizat);
    }

    private void StergeSofer_Click(object sender, RoutedEventArgs e)
    {
        ResetValidareSofer();

        if (!int.TryParse(SoferIdTextBox.Text, out int id))
        {
            MarcheazaInvalid(SoferIdLabel, new StringBuilder(), string.Empty);
            AfiseazaMesajSofer("Introduceti ID-ul soferului care trebuie sters.", esteEroare: true);
            return;
        }

        bool sters = _viewModel.StergeSofer(id);
        if (sters)
        {
            ReseteazaFormularSofer();
        }

        AfiseazaMesajSofer(
            sters ? "Soferul a fost sters cu succes." : "Soferul nu a fost gasit.",
            esteEroare: !sters);
    }

    private void ResetSofer_Click(object sender, RoutedEventArgs e)
    {
        ReseteazaFormularSofer();
        ResetValidareSofer();
    }

    private void CautaSofer_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.CautaSoferDupaNume(SoferSearchTextBox.Text);
        AfiseazaMesajSofer("Cautarea dupa nume a fost aplicata.", esteEroare: false);
    }

    private void ResetCautareSofer_Click(object sender, RoutedEventArgs e)
    {
        SoferSearchTextBox.Clear();
        _viewModel.ResetCautareSoferi();
        ResetValidareSofer();
    }

    private void SoferSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SoferSearchTextBox.Text))
        {
            _viewModel.ResetCautareSoferi();
        }
    }

    private void SoferCamp_TextChanged(object sender, TextChangedEventArgs e)
    {
        ResetValidareSofer();
    }

    private void SoferComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SoferComboBox.SelectedItem is not Sofer sofer)
        {
            return;
        }

        SoferDataGrid.SelectedItem = sofer;
        IncarcaSoferInFormular(sofer);
    }

    private void SoferDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SoferDataGrid.SelectedItem is not Sofer sofer)
        {
            return;
        }

        SoferComboBox.SelectedItem = sofer;
        IncarcaSoferInFormular(sofer);
    }

    private bool ValideazaFormularSofer(bool verificaIdUnic, out int id, out string nume, out double kilometri)
    {
        var mesaje = new StringBuilder();
        bool dateValide = true;

        nume = SoferNumeTextBox.Text.Trim();

        if (!int.TryParse(SoferIdTextBox.Text, out id) || id < SoferIdMinim)
        {
            MarcheazaInvalid(
                SoferIdLabel,
                mesaje,
                $"ID-ul soferului trebuie sa fie un numar intreg >= {SoferIdMinim}.");
            dateValide = false;
        }
        else if (verificaIdUnic && _viewModel.ExistaSofer(id))
        {
            MarcheazaInvalid(SoferIdLabel, mesaje, "Exista deja un sofer cu acest ID.");
            dateValide = false;
        }

        if (nume.Length < SoferNumeMinLength || nume.Length > SoferNumeMaxLength)
        {
            MarcheazaInvalid(
                SoferNumeLabel,
                mesaje,
                $"Numele trebuie sa aiba intre {SoferNumeMinLength} si {SoferNumeMaxLength} caractere.");
            dateValide = false;
        }

        if (!double.TryParse(SoferKmTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out kilometri) &&
            !double.TryParse(SoferKmTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out kilometri))
        {
            kilometri = 0;
            MarcheazaInvalid(SoferKmLabel, mesaje, "Kilometrii trebuie sa fie un numar valid.");
            dateValide = false;
        }
        else if (kilometri < SoferKmMinim || kilometri > SoferKmMaxim)
        {
            MarcheazaInvalid(
                SoferKmLabel,
                mesaje,
                $"Kilometrii trebuie sa fie intre {SoferKmMinim:N0} si {SoferKmMaxim:N0}.");
            dateValide = false;
        }

        if (dateValide)
        {
            return true;
        }

        SoferErrorTextBlock.Text = mesaje.ToString().TrimEnd();
        SoferErrorPanel.Visibility = Visibility.Visible;
        return false;
    }

    private void IncarcaSoferInFormular(Sofer sofer)
    {
        SoferIdTextBox.Text = sofer.Id.ToString(CultureInfo.CurrentCulture);
        SoferNumeTextBox.Text = sofer.Nume;
        SoferKmTextBox.Text = sofer.TotalKilometriParcursi.ToString(CultureInfo.CurrentCulture);
        ResetValidareSofer();
    }

    private void ResetValidareSofer()
    {
        if (SoferIdLabel == null)
        {
            return;
        }

        SoferIdLabel.Foreground = _labelNormalBrush;
        SoferNumeLabel.Foreground = _labelNormalBrush;
        SoferKmLabel.Foreground = _labelNormalBrush;

        SoferErrorTextBlock.Foreground = _labelInvalidBrush;
        SoferErrorPanel.Background = new SolidColorBrush(Color.FromRgb(255, 241, 241));
        SoferErrorPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(240, 184, 184));
        SoferErrorPanel.Visibility = Visibility.Collapsed;
        SoferErrorTextBlock.Text = string.Empty;
    }

    private void ReseteazaFormularSofer()
    {
        SoferIdTextBox.Clear();
        SoferNumeTextBox.Clear();
        SoferKmTextBox.Clear();
        SoferComboBox.SelectedIndex = -1;
        SoferDataGrid.SelectedIndex = -1;
    }

    private void AfiseazaMesajSofer(string mesaj, bool esteEroare)
    {
        SoferErrorTextBlock.Text = mesaj;
        SoferErrorTextBlock.Foreground = esteEroare
            ? _labelInvalidBrush
            : new SolidColorBrush(Color.FromRgb(23, 107, 91));
        SoferErrorPanel.Background = esteEroare
            ? new SolidColorBrush(Color.FromRgb(255, 241, 241))
            : new SolidColorBrush(Color.FromRgb(237, 245, 242));
        SoferErrorPanel.BorderBrush = esteEroare
            ? new SolidColorBrush(Color.FromRgb(240, 184, 184))
            : new SolidColorBrush(Color.FromRgb(207, 225, 219));
        SoferErrorPanel.Visibility = Visibility.Visible;
    }

    private enum ModFormular
    {
        Adaugare,
        Editare
    }
}
