using OrganizaceTurnaje.Model;
using OrganizaceTurnaje.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrganizaceTurnaje.Views
{
    /// <summary>
    /// Interakční logika pro InfoTournament.xaml
    /// </summary>
    public partial class InfoTournament : Window
    {
        public InfoTournament()
        {
            InitializeComponent();
            Loaded += InfoTournament_Loaded;
        }

        private void InfoTournament_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ICloseWindows vm)
            {
                vm.Close += () =>
                {
                    this.Close();
                };
                Closing += (s, e) =>
                {
                    e.Cancel = !vm.CanClose();
                };
            }
        }
    }
}
