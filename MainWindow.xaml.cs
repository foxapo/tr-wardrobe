using System.Windows;
using System.Windows.Controls;

namespace TRWardrobe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (!OutfitUtils.IsBackup())
        {
            OutfitUtils.CreateBackup();
        }
    }

    private void OnRadioChecked(object sender, RoutedEventArgs e)
    {
        RadioButton radioButton = (RadioButton)sender;

        if (!OutfitUtils.IsBackup())
        {
            OutfitUtils.CreateBackup();
        }

        OutfitUtils.SetOutfit((string)radioButton.Tag);
    }

    private void Restore(object sender, RoutedEventArgs e)
    {
        OutfitUtils.RestoreBackup();
    }
}