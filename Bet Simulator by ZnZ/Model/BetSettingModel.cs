using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Bet_Simulator_by_ZnZ.Annotations;
using Bet_Simulator_by_ZnZ.ViewModel;

namespace Bet_Simulator_by_ZnZ.Model
{
    public class BetSettingModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public enum Change { Percent, Plus, ToBaseBet, Increase }

        private SimulatorViewModel _Owner;
        public SimulatorViewModel Owner
        {
            get => _Owner;
            set
            {
                _Owner = value;
                OnPropertyChanged();
            }
        }

        public BetSettingModel(string Header, SimulatorViewModel owner)
        {
            this.Header = Header;
            Owner = owner;
        }

        private Change _ChangeType = Change.ToBaseBet;
        public Change ChangeType
        {
            get => _ChangeType;
            set
            {
                _ChangeType = value;
                OnPropertyChanged();
            }
        }

        private string _Header = "Header";
        public string Header
        {
            get => _Header;
            set
            {
                _Header = value;
                OnPropertyChanged();
            }
        }

        private double _Percent = 1;
        public double Percent
        {
            get => _Percent;
            set
            {
                _Percent = value;
                OnPropertyChanged();
            }
        }

        private double _Increase = 2;
        public double Increase
        {
            get => _Increase;
            set
            {
                _Increase = value;
                OnPropertyChanged();
            }
        }

        private double _Plus = 0.00000001;
        public double Plus
        {
            get => _Plus;
            set
            {
                _Plus = value;
                OnPropertyChanged();
            }
        }

        private bool _ChangeMultiplier;
        public bool ChangeMultiplier
        {
            get => _ChangeMultiplier;
            set
            {
                _ChangeMultiplier = value;
                OnPropertyChanged();
            }
        }

        private double _Multiplier = 2;
        public double Multiplier
        {
            get => _Multiplier;
            set
            {
                _Multiplier = Helper.Range(value, 1.01, 4750);
                OnPropertyChanged();
            }
        }
    }
}
