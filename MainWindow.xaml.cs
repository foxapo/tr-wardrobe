using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TRWardrobe.Classes;

namespace TRWardrobe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public BitmapImage Logo { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Images/logo.png"));
    public BitmapImage CurrentImage { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Images/antar.jpg"));
    private string _selectedOutfit = "";


    public List<MappingObject> ModdedOutfits => OutfitUtils.ModdedOutfits;

    public MainWindow()
    {
        OutfitUtils.Init();
        InitializeComponent();
        this.DataContext = this;
    }

    private void UI_OnRadioChecked(object sender, RoutedEventArgs e)
    {
        RadioButton radioButton = (RadioButton)sender;

        if (!OutfitUtils.IsBackup())
        {
            OutfitUtils.CreateBackup();
        }

        _selectedOutfit = (string)radioButton.Tag;

        bool isModded = _selectedOutfit.Contains("_MODDED");

        OutfitUtils.SetOutfit(_selectedOutfit, isModded);
    }


    private void UI_OnRadioMouseEnter(object sender, MouseEventArgs e)
    {
        string imagePath = "pack://application:,,,/Images/" + (string)((RadioButton)sender).Tag + ".jpg";
        Console.WriteLine(imagePath);
        CurrentImage = new BitmapImage(new Uri(imagePath));
        OnPropertyChanged(nameof(CurrentImage));
        BackgroundImageBrush.ImageSource = CurrentImage;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void UI_RestoreBackup(object sender, RoutedEventArgs e)
    {
        OutfitUtils.RestoreBackup();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void UI_ResetTextures(object sender, RoutedEventArgs e)
    {
        OutfitUtils.ResetTextures();
    }

    protected static void UI_About(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "TRWardrobe v1.0.0\n\nDeveloped by: @mikemikemikemike\n\nSpecial thanks to: @sapper, @sapper, @sapper");
    }

    protected void UI_Exit(object sender, RoutedEventArgs routedEventArgs)
    {
        Application.Current.Shutdown();
    }
}