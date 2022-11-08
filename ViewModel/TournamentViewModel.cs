using OrganizaceTurnaje.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizaceTurnaje.ViewModel
{
    public class TournamentViewModel
    {
        public ObservableCollection<Tournament> Tournaments { get; set; }
        public MyICommand DeleteTournamentCommand { get; set; }
        public MyICommand NewTournamentCommand { get; set; }
        public TournamentViewModel()
        {
            ObservableCollection<Tournament> tournaments = new ObservableCollection<Tournament>();

            tournaments.Add(new Tournament() { Name = "testovaci", Players = { new Player(0, "Jan", "Hendrych"), 
                    new Player(0, "Petra", "Tichá"), new Player(0, "Jaroslav", "Veselý") } });
            tournaments.Add(new Tournament() { Name = "testovaci2", Players = { new Player(0, "Petr", "Dlouhý"),
                new Player(0, "Aleš", "Smutný")} });

            Tournaments = tournaments;
            DeleteTournamentCommand = new MyICommand(OnDelete, CanDelete);
            NewTournamentCommand = new MyICommand(OnAdd, CanAdd);
        }
        private void OnAdd()
        {
            Tournaments.Add(new Tournament() { Name="null"});
        }
        private bool CanAdd()
        {
            return true;
        }

        private void OnDelete()
        {
            Tournaments.Remove(SelectedTournament);
        }

        private bool CanDelete()
        {
            return true;
        }
        private Tournament selectedTournament;

        public Tournament SelectedTournament
        {
            get
            {
                return selectedTournament;
            }

            set
            {
                selectedTournament = value;
                DeleteTournamentCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
