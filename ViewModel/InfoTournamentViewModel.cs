using OrganizaceTurnaje.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace OrganizaceTurnaje.ViewModel
{
    public class InfoTournamentViewModel : ICloseWindows
    {
        public ObservableCollection<Tournament> Tournaments { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public MyICommand AddPlayerCommand { get; set; }
        public MyICommand DeletePlayerCommand { get; set; }
        public MyICommand OnConfirmChanges { get; set; }

        public InfoTournamentViewModel()
        {
            ObservableCollection<Tournament> tournaments = new ObservableCollection<Tournament>();

            Tournaments = tournaments;
        }
        public InfoTournamentViewModel(Tournament tournament)
        {
            ObservableCollection<Tournament> tournaments = new ObservableCollection<Tournament>();

            tournaments.Add(tournament);

            Tournaments = tournaments;

            Name = tournament.Name;
            Players = new ObservableCollection<Player>();

            foreach (Player player in tournament.Players)
            {
                Players.Add(player);
            }

            AddPlayerCommand = new MyICommand(OnAddPlayer, CanAddPlayer);
            DeletePlayerCommand = new MyICommand(OnDeletePlayer, CanDeletePlayer);
            OnConfirmChanges = new MyICommand(OnConfirm, CanConfirm);
        }

        private void OnConfirm()
        {
            Close?.Invoke();
        }

        private bool CanDeletePlayer()
        {
            return SelectedPlayer != null;
        }

        private void OnDeletePlayer()
        {
            MessageBoxResult result = MessageBox.Show($"Chcete opravdu smazat hráce {SelectedPlayer}?", "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Tournaments.First().Players.Remove(SelectedPlayer);
                Players.Remove(SelectedPlayer);
            }

            OnConfirmChanges.RaiseCanExecuteChanged();
        }

        private bool CanConfirm()
        {
            return true;
        }
        private bool CanAddPlayer()
        {
            return true;
        }

        private void OnAddPlayer()
        {
            Player player = new Player() { FirstName = "firstName", LastName = "lastname" };

            Tournaments.First().AddPlayer(player);
            Players.Add(player);

            OnConfirmChanges.RaiseCanExecuteChanged();
        }

        public bool CanClose()
        {
            if (Players.Count < 2)
            {
                MessageBox.Show("Nízký počet hráčů!", "Varování", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            List<Player> distinctPeople = Players
                                          .GroupBy(p => new { p.FirstName, p.LastName })
                                          .Select(g => g.First())
                                          .ToList();
            if (Players.Count != distinctPeople.Count())
            {
                MessageBox.Show("Duplicity!", "Varování", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private Player selectedPlayer;

        public Player SelectedPlayer
        {
            get
            {
                return selectedPlayer;
            }

            set
            {
                selectedPlayer = value;
                DeletePlayerCommand.RaiseCanExecuteChanged();
                OnConfirmChanges.RaiseCanExecuteChanged();
            }
        }

        public Action Close { get; set; }
    }
}
