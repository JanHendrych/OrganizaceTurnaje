using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizaceTurnaje.Model
{
    public class Score : INotifyPropertyChanged
    {
        public Player Player { get; set; }
        public int? Points { get; set; } = 0;
        public Tournament Tournament { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override string? ToString()
        {
            return Player.ToString() + Points.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Score))
            {
                return false;
            }
            return this.Player == ((Score)obj).Player;
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
