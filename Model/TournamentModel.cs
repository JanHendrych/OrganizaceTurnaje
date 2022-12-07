using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizaceTurnaje.Model
{
    public class TournamentModel
    {
    }
    public class Tournament : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public int ID { get; set; }
        public string Name { get; set; }
        private List<Player> players = new List<Player>();
        public List<Player> Players {
            get
            {
                return players;
            }

            set
            {
                    players = value;
                    RaisePropertyChanged("Players");  
            }
        }
        public bool IsStarted { get; set; } = false;
        public void AddPlayer(Player player)
        {
            players.Add(player);
        }
        public void Remove(Player player)
        {
            players.Remove(player);
        }
        public void RemoveAll()
        {
            players.RemoveAll(x => x != null);
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Tournament tournament &&
                   Name == tournament.Name;
        }
    }
}
