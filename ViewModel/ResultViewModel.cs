using LiteDB;
using OrganizaceTurnaje.Model;
using OrganizaceTurnaje.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizaceTurnaje.ViewModel
{
    public class ResultViewModel
    {
        public List<Score> Scores { get; set; }
        public List<Score> AllScores { get; set; }
        public List<PlayerScorePair> Points { get; set; }
        public List<PlayerScorePair> AllPoints { get; set; }
        public Tournament SelectedTournament { get; }

        public ResultViewModel()
        {
            Scores= new List<Score>();
            AllScores= new List<Score>();
            Points = new List<PlayerScorePair>();
            AllPoints = new List<PlayerScorePair>();

            LoadScore();
        }

        public ResultViewModel(Tournament selectedTournament)
        {
            SelectedTournament = selectedTournament;

            Scores = new List<Score>();
            AllScores = new List<Score>();
            Points = new List<PlayerScorePair>();
            AllPoints = new List<PlayerScorePair>();

            LoadScore();
            LoadPoints();
        }

        private void LoadScore()
        {
            using (var db = new LiteDatabase("Score.db"))
            {
                var column = db.GetCollection<Score>("score");

                var result = column.Query().ToList();
                Scores.Clear();

                ObservableCollection<Score> allScores = new ObservableCollection<Score>();

                foreach (var item in result)
                {
                    allScores.Add(item);
                    AllScores.Add(item);
                }

                var sortedList = AllScores.OrderByDescending(x => x.Points).ToList();
                foreach (var item in sortedList)
                {
                    if (item.Tournament.Equals(SelectedTournament))
                    {
                        Scores.Add(item);
                    }
                }
            }
        }
        private void LoadPoints()
        {
            using (var db = new LiteDatabase("PlayerScorePair.db"))
            {
                var column = db.GetCollection<PlayerScorePair>("playerScorePair");

                var result = column.Query().ToList();
                Points.Clear();

                ObservableCollection<PlayerScorePair> allPoints = new ObservableCollection<PlayerScorePair>();

                foreach (var item in result)
                {
                    allPoints.Add(item);
                    AllPoints.Add(item);
                }

                var sortedList = AllPoints.OrderByDescending(x => x.Score).ToList();
                foreach (var item in sortedList)
                {
                    if (item.Tournament.Equals(SelectedTournament))
                    {
                        Points.Add(item);
                    }
                }
            }
        }
    }
}
