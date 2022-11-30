using OrganizaceTurnaje.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrganizaceTurnaje.ViewModel
{
    public class InfoTournamentViewModel
    {
        public ObservableCollection<Tournament> Tournaments { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public MyICommand AddPlayerCommand { get; set; }

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
            
        }
    }
}
