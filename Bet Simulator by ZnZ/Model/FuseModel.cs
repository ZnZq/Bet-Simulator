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
    public class FuseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public FuseModel(SimulatorViewModel owner)
        {
            Owner = owner;
        }

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

        private bool _StopOnProfit;
        public bool StopOnProfit
        {
            get => _StopOnProfit;
            set
            {
                _StopOnProfit = value;
                OnPropertyChanged();
            }
        }

        private double _IfProfit = 0.00000001;
        public double IfProfit
        {
            get => _IfProfit;
            set
            {
                _IfProfit = value;
                OnPropertyChanged();
            }
        }

        private double _IfLoss = 0.00000001;
        public double IfLoss
        {
            get => _IfLoss;
            set
            {
                _IfLoss = value;
                OnPropertyChanged();
            }
        }

        private bool _StopOnLoss;
        public bool StopOnLoss
        {
            get => _StopOnLoss;
            set
            {
                _StopOnLoss = value;
                OnPropertyChanged();
            }
        }

        private bool _StopMaxSumm;
        public bool StopMaxSumm
        {
            get => _StopMaxSumm;
            set
            {
                _StopMaxSumm = value;
                OnPropertyChanged();
            }
        }

        private double _MaxSumm = 1;
        public double MaxSumm
        {
            get => _MaxSumm;
            set
            {
                _MaxSumm = Helper.Range(value, 0, double.MaxValue);
                OnPropertyChanged();
            }
        }

        private bool _StopMaxBet;
        public bool StopMaxBet
        {
            get => _StopMaxBet;
            set
            {
                _StopMaxBet = value;
                OnPropertyChanged();
            }
        }

        private double _MaxBet = .1;
        public double MaxBet
        {
            get => _MaxBet;
            set
            {
                _MaxBet = value;
                OnPropertyChanged();
            }
        }
    }
}
