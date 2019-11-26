using Bet_Simulator_by_ZnZ.Annotations;
using Bet_Simulator_by_ZnZ.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bet_Simulator_by_ZnZ.ViewModel
{
    public class SimulatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public SynchronizationContext sync;

        public enum BetOnType { HIGH, LOW, ALTERNATE }

        public List<BetOnType> BetOnList { get; } = new List<BetOnType>() { BetOnType.HIGH, BetOnType.LOW, BetOnType.ALTERNATE };

        public BetSettingModel OnLossSetting { get; set; }
        public BetSettingModel OnWinSetting { get; set; }
        public FuseModel Fuse { get; set; }

        public ObservableCollection<GameThreadModel> GameThreadModels { get; set; } = new ObservableCollection<GameThreadModel>();

        public SimulatorViewModel()
        {
            sync = SynchronizationContext.Current;

            MyBets = false;

            OnLossSetting = new BetSettingModel("Если проиграли ставку", this);
            OnWinSetting = new BetSettingModel("Если выиграли ставку", this);
            Fuse = new FuseModel(this);
        }

        private BetOnType _BetOn = BetOnType.ALTERNATE;
        public BetOnType BetOn
        {
            get => _BetOn;
            set
            {
                _BetOn = value;
                OnPropertyChanged();
            }
        }

        private double _Balance = 1;
        public double Balance
        {
            get => _Balance;
            set
            {
                _Balance = value;
                OnPropertyChanged();
            }
        }

        private double _BaseBet = 0.00000001;
        public double BaseBet
        {
            get => _BaseBet;
            set
            {
                _BaseBet = value;
                OnPropertyChanged();
            }
        }

        private double _Multiplier = 2;
        public double Multiplier
        {
            get => _Multiplier;
            set
            {
                _Multiplier = Helper.Range(value, 1.01, 4750.0);
                OnPropertyChanged();
            }
        }

        private int _RollCount = 100;
        public int RollCount
        {
            get => _RollCount;
            set
            {
                _RollCount = Helper.Range(value, 1, int.MaxValue);
                OnPropertyChanged();
            }
        }

        private int _ThreadCount = 1;
        public int ThreadCount
        {
            get => _ThreadCount;
            set
            {
                _ThreadCount = Helper.Range(value, 1, int.MaxValue);
                OnPropertyChanged();
            }
        }

        private int _ThreadSleep = 10;
        public int ThreadSleep
        {
            get => _ThreadSleep;
            set
            {
                _ThreadSleep = value;
                OnPropertyChanged();
            }
        }

        private bool _MyBets;
        public bool MyBets
        {
            get => _MyBets;
            set
            {
                _MyBets = value;
                OnPropertyChanged();
            }
        }

        private GameThreadModel _SelectedThread;
        public GameThreadModel SelectedThread
        {
            get => _SelectedThread;
            set
            {
                _SelectedThread = value;
                OnPropertyChanged();
            }
        }

        private object calc = new object();

        private ICommand _CalcCommand;
        public ICommand CalcCommand => _CalcCommand ?? (_CalcCommand = new RelayCommand(async o =>
        {
            GameThreadModels.Clear();

            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < ThreadCount; i++)
                {
                    GameThreadModel model = new GameThreadModel()
                    {
                        BaseBet = BaseBet,
                        Balance = Balance,
                        BetOn = BetOn,
                        OnLoss = OnLossSetting,
                        OnWin = OnWinSetting,
                        Fuse = Fuse,
                        Multiplier = Multiplier,
                        RollCount = RollCount,
                        sleep = ThreadSleep,
                        sync = sync
                    };

                    model.Start();

                    sync.Send(q =>
                    {
                        GameThreadModels.Add(model);
                        if (i == 0)
                            SelectedThread = GameThreadModels[0];
                    }, null);
                }
            });
        }));
    }
}