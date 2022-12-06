using LiteDB;
using OrganizaceTurnaje.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace OrganizaceTurnaje.ViewModel
{
    public partial class StartedTournamentViewModel : ICloseWindows
    {
        public Tournament Tournament { get; set; }
        public List<Player> Players { get; set; }
        public ObservableCollection<Round> Rounds { get; set; }
        public ObservableCollection<Score> Scores { get; set; }
        public ObservableCollection<PlayerScorePair> Points { get; set; }
        public MyICommand EndTournamentCommand { get; set; }
        public MyICommand FinishTournamentCommand { get; set; }
        public MyICommand FinishTournament { get; set; }
        private bool canFinish;
        public StartedTournamentViewModel()
        {

        }
        public StartedTournamentViewModel(Tournament tournament)
        {
            Tournament = tournament;
            Players = new List<Player>();
            Players = tournament.Players;
            EndTournamentCommand = new MyICommand(OnEndTournament, CanEndTournament);
            Scores = new ObservableCollection<Score>();
            Points = new ObservableCollection<PlayerScorePair>();

            FinishTournament = new MyICommand(OnCloseTournament, CanCloseTournament);

            canFinish = false;

            CreateRounds();
        }

        private bool CanCloseTournament()
        {
            return true;
        }

        private void OnCloseTournament()
        {
            MessageBoxResult result = MessageBox.Show($"Chcete opravdu zavřít toto okno ?", "Potvrzení", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new LiteDatabase("Score.db"))
                {
                    var column = db.GetCollection<Score>("score");

                    foreach (var item in Scores)
                    {
                        column.Insert(item);
                    }
                    column.EnsureIndex(x => x.Player);
                }
                using (var db = new LiteDatabase("PlayerScorePair.db"))
                {
                    var column = db.GetCollection<PlayerScorePair>("playerScorePair");

                    foreach (var item in Points)
                    {
                        column.Insert(item);
                    }
                    column.EnsureIndex(x => x.PlayerPair);
                }

                Close?.Invoke();
            }
        }

        private bool CanEndTournament()
        {
            var result = Rounds.Where(x =>
             x.PlayerAPoints == null && x.PlayerBPoints == null
            ).ToList();

            return !result.Any();
        }

        private void OnEndTournament()
        {
            //Přiřazení vítězů
            foreach (var round in Rounds)
            {
                if (round.PlayerAPoints > round.PlayerBPoints)
                {
                    round.Winner = round.PlayerA;
                }
                else if (round.PlayerAPoints < round.PlayerBPoints)
                {
                    round.Winner = round.PlayerB;
                }

                //Přidělení celkových bodů (vyhraná kola)
                if (round.Winner is not null)
                {
                    if (Scores.Contains(new Score() { Player = round.Winner }))
                    {
                        Score score = Scores.First(s => s.Player.Equals(round.Winner));
                        int indexOf = Scores.IndexOf(score);
                        Scores[indexOf].Points++;
                    }
                    else
                    {
                        Scores.Add(new Score() { Player = round.Winner, Points = 1 });
                    }
                    var sortedList = Scores.OrderByDescending(x => x.Points).ToList();
                    Scores.Clear();
                    foreach (var sortedItem in sortedList)
                    {
                        Scores.Add(sortedItem);
                    }
                }

            }

            //Počítání Vítěze
            var grouped = Rounds.ToLookup(x => x.Winner);
            var maxRepetitions = grouped.Max(x => x.Count());
            var maxRepeatedItems = grouped.Where(x => x.Count() == maxRepetitions)
                                          .Select(x => x.Key).ToList();

            List<PlayerScorePair> playerScorePairs = BestPlayerByScore();
            foreach (var playerPair in playerScorePairs)
            {
                Points.Add(playerPair);
            }

            if (maxRepeatedItems.Count > 1)
            {
                var maxPointsPlayer = Points.First().PlayerPair;

                var message = string.Join(Environment.NewLine, maxRepeatedItems);
                MessageBox.Show("Hráči \n" + message + "\n mají stejný počet vítězství \n \n" +
                    maxPointsPlayer.ToString() + " má nejvíce bodů, tudíž se stává vítězem!",
                    "Vítěz", MessageBoxButton.OK, MessageBoxImage.Information);

                Score score = Scores.First(s => s.Player.Equals(maxPointsPlayer));
                int indexOf = Scores.IndexOf(score);
                Scores[indexOf].Points++;
            }
            else
            {
                var message = string.Join(Environment.NewLine, maxRepeatedItems);
                MessageBox.Show("Vítězem turnaje se stává: " + message, "Vítěz", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            canFinish = true;
        }

        private void CreateRounds()
        {
            Rounds = new ObservableCollection<Round>();

            foreach (var player in Players)
            {
                foreach (var opponent in Players.Where((x => x != player)))
                {
                    if (Rounds.Contains(new Round() { PlayerA = player, PlayerB = opponent }) ||
                        Rounds.Contains(new Round() { PlayerA = opponent, PlayerB = player }))
                    {
                        continue;
                    }
                    Rounds.Add(new Round() { PlayerA = player, PlayerB = opponent });
                }
            }
        }
        private Round selectedRound;

        public Round SelectedRound
        {
            get
            {
                return selectedRound;
            }

            set
            {
                if (value != null)
                {
                    selectedRound = value;
                }
                EndTournamentCommand.RaiseCanExecuteChanged();
            }
        }

        private List<PlayerScorePair> BestPlayerByScore()
        {
            var result = new List<PlayerScorePair>();

            foreach (var round in Rounds)
            {
                if (result.Contains(new PlayerScorePair() { PlayerPair = round.PlayerA }))
                {
                    result.Find(x => x.PlayerPair == round.PlayerA).Score += round.PlayerAPoints;
                }
                else
                {
                    result.Add(new PlayerScorePair() { PlayerPair = round.PlayerA, Score = round.PlayerAPoints });
                }

                if (result.Contains(new PlayerScorePair() { PlayerPair = round.PlayerB }))
                {
                    result.Find(x => x.PlayerPair == round.PlayerB).Score += round.PlayerBPoints;
                }
                else
                {
                    result.Add(new PlayerScorePair() { PlayerPair = round.PlayerB, Score = round.PlayerBPoints });
                }
            }
            var sortedList = result.OrderByDescending(x => x.Score).ToList();
            return sortedList;
        }
        public Action Close { get; set; }
        public bool CanClose()
        {
            return canFinish;
        }
    }
    interface ICloseWindows
    {
        Action Close { get; set; }
        bool CanClose();
    }
}
