using LiteDB;
using OrganizaceTurnaje.Model;
using OrganizaceTurnaje.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace OrganizaceTurnaje.ViewModel
{
    public class TournamentViewModel
    {
        public ObservableCollection<Tournament> Tournaments { get; set; }
        public MyICommand DeleteTournamentCommand { get; set; }
        public MyICommand NewTournamentCommand { get; set; }
        public MyICommand SaveTournamentsCommand { get; set; }
        public MyICommand LoadTournamentsCommand { get; set; }
        public MyICommand ShowTournamentCommand { get; set; }
        public MyICommand ShowResultsCommand { get; set; }
        public MyICommand StartTournamentCommand { get; set; }
        public bool NextTimeNotShow { get; set; }

        public TournamentViewModel()
        {
            ObservableCollection<Tournament> tournaments = new ObservableCollection<Tournament>();
            Tournaments = tournaments;

            DeleteTournamentCommand = new MyICommand(OnDelete, CanDelete);
            NewTournamentCommand = new MyICommand(OnAdd, CanAdd);
            SaveTournamentsCommand = new MyICommand(OnSave, CanSave);
            LoadTournamentsCommand = new MyICommand(OnLoad, CanLoad);
            ShowTournamentCommand = new MyICommand(OnShow, CanShow);
            ShowResultsCommand = new MyICommand(OnShowResults, CanShowResults);
            StartTournamentCommand = new MyICommand(OnStartTournament, CanStartTournament);

            NextTimeNotShow = false;
        }

        private bool CanStartTournament()
        {
            return SelectedTournament != null && SelectedTournament?.IsStarted != true && SelectedTournament?.Players.Count >= 2;
        }

        private void OnStartTournament()
        {
            if (SelectedTournament.Players.Count<2 || SelectedTournament.IsStarted == true)
            {
                MessageBox.Show("Příliš nízký počet hráčů","Varování",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Chcete opravdu odstartovat turnaj {SelectedTournament.Name}?", "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                StartedTournament startedTournament = new StartedTournament();

                OrganizaceTurnaje.ViewModel.StartedTournamentViewModel viewModel =
                        new OrganizaceTurnaje.ViewModel.StartedTournamentViewModel(SelectedTournament);

                startedTournament.DataContext = viewModel;

                startedTournament.ShowDialog();

                if (viewModel.ClosedByButton == true)
                {
                    SelectedTournament.IsStarted = true;
                } else
                {
                    MessageBox.Show("Nesprávné uzavření formuláře! \n Data nebyla uložena!", "Varování", MessageBoxButton.OK, MessageBoxImage.Error);
                    SelectedTournament.IsStarted = false;
                }
            }
        }

        private bool CanShowResults()
        {
            bool result = false;
            if (SelectedTournament is not null)
            {
                result = SelectedTournament.IsStarted;
            }
            return result;
        }

        private void OnShowResults()
        {
            Result result = new Result();

            OrganizaceTurnaje.ViewModel.ResultViewModel viewModel =
                    new OrganizaceTurnaje.ViewModel.ResultViewModel(SelectedTournament);

            result.DataContext = viewModel;

            result.ShowDialog();
        }

        private void OnAdd()
        {
            Tournaments.Add(new Tournament() { Name = "null" });
        }
        private bool CanAdd()
        {
            return true;
        }

        private void OnDelete()
        {
            MessageBoxResult result = MessageBox.Show($"Chcete opravdu smazat turnaj {SelectedTournament.Name}?", "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Tournaments.Remove(SelectedTournament);
            }
        }

        private bool CanDelete()
        {
            return SelectedTournament != null;
        }

        private void OnSave()
        {
            using (var db = new LiteDatabase("tournaments.db"))
            {
                var column = db.GetCollection<Tournament>("tournament");
                column.DeleteAll();

                foreach (var item in Tournaments)
                {
                    column.Insert(item);
                }
                column.EnsureIndex(x => x.Name);
            }
        }
        private bool CanSave()
        {
            return true;
        }

        private void OnLoad()
        {
            using (var db = new LiteDatabase("tournaments.db"))
            {
                var column = db.GetCollection<Tournament>("tournament");

                var result = column.Query().ToList();
                Tournaments.Clear();

                ObservableCollection<Tournament> tournaments = new ObservableCollection<Tournament>();

                foreach (var item in result)
                {
                    tournaments.Add(item);
                }
                foreach (var item in tournaments)
                {
                    Tournaments.Add(item);
                }
            }
        }
        private bool CanLoad()
        {
            return true;
        }

        private void OnShow()
        {
            InfoTournament info = new InfoTournament();
            OrganizaceTurnaje.ViewModel.InfoTournamentViewModel viewModel =
                    new OrganizaceTurnaje.ViewModel.InfoTournamentViewModel(SelectedTournament);
            info.DataContext = viewModel;

            info.ShowDialog();

            int oldIndex = Tournaments.IndexOf(viewModel.Tournaments.First());
            Tournaments.RemoveAt(oldIndex);
            Tournaments.Insert(oldIndex, viewModel.Tournaments.First());

            List<Player> allPlayers = new List<Player>();
            foreach (var tournament in Tournaments)
            {
                foreach (var player in tournament.Players)
                {
                    allPlayers.Add(player);
                }
            }
            
            var distinctPeople = allPlayers
                                          .GroupBy(p => new { p.FirstName, p.LastName })
                                          .Where(g => g.Count() > 1)
                                          .Select(g => g.First())
                                          .ToList();
            
            var message = string.Join(Environment.NewLine, distinctPeople);

            if (distinctPeople.Any() && NextTimeNotShow == false)
            {
                MessageBoxResult msgResult = MessageBox.Show("V seznamu hráčů se objevují následující duplicity: \n" + message + "\n" + 
                    "Každý duplicitní hráč se v celkové tabulce vítězů zobrazí jako jeden hráč." + "\n\n" + "Příště nezobrazovat?"
                    ,"Varování",MessageBoxButton.YesNo,MessageBoxImage.Question);
                if (msgResult == MessageBoxResult.Yes)
                {
                    NextTimeNotShow = true;
                }
            }
        }
        private bool CanShow()
        {
            if (SelectedTournament != null)
            {
                return true;
            }
            return false;
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
                ShowTournamentCommand.RaiseCanExecuteChanged();
                StartTournamentCommand.RaiseCanExecuteChanged();
                ShowResultsCommand.RaiseCanExecuteChanged();
            }
        }
        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            item.IsSelected = true;
        }
    }
}
