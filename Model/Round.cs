using OrganizaceTurnaje.Model;
using System;
using System.ComponentModel;

namespace OrganizaceTurnaje.Model
{
    public class Round : INotifyPropertyChanged
    {
        public Player PlayerA { get; set; }
        public int? PlayerAPoints { get; set; } = null;
        public int? PlayerBPoints { get; set; } = null;
        public Player PlayerB { get; set; }
        public Player Winner { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override string? ToString()
        {
            return PlayerA.ToString() + " vs " + PlayerB.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Round))
            {
                return false;
            }
            return (this.PlayerA == ((Round)obj).PlayerA)
                && (this.PlayerB == ((Round)obj).PlayerB);
        }
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

}
