using Bet_Simulator_by_ZnZ.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Bet_Simulator_by_ZnZ.ViewModel;
using System.Xml.Serialization;

namespace Bet_Simulator_by_ZnZ.Model
{
    public class GameThreadModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static Random rand = new Random();

        private double _Balance;
        public double Balance
        {
            get => _Balance;
            set
            {
                _Balance = value;
                OnPropertyChanged();
            }
        }
        public double BaseBet { get; set; }
        public double Bet { get; set; }
        public double Multiplier { get; set; }
        public int RollCount { get; set; }
        public SimulatorViewModel.BetOnType BetOn { get; set; }
        public BetSettingModel OnWin { get; set; }
        public BetSettingModel OnLoss { get; set; }
        public FuseModel Fuse { get; set; }
        public int sleep = 50;
        public SynchronizationContext sync;

        public double lossBet = 0;
        public double winBet = 0;

        private Thread _thread;
        private Action end;

        public ObservableCollection<GameResult> GameResultsList { get; set; } = new ObservableCollection<GameResult>();

        public GameThreadModel(Action endCalc = null)
        {
            end = endCalc;
            GameResultsList.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(GameResultsList)); };
        }

        public int lt(double multiplier) => Convert.ToInt32(Math.Round(9500 / multiplier, 2));
        public int gt(double multiplier) => Convert.ToInt32(Math.Round(10000 - 9500 / multiplier, 2));

        public void Start()
        {
            if (_thread != null)
                return;

            _thread = new Thread(Excecute)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        private int CurrentRollsCount;

        private void Excecute()
        {
            sync.Send(q => { GameResultsList.Clear(); }, null);

            double balance = Balance;

            WinCount = LossCount = 0;

            sync.Send(q =>
            {
                Status = "Расчёт запущен!";
                ClearProfit = SummBet = lossBet = winBet = 0;
            }, null);

            if (!CanBet(Bet = BaseBet))
                return;

            bool onLow = BetOn == SimulatorViewModel.BetOnType.ALTERNATE || Convert.ToBoolean((int)BetOn);

            if (BetOn == SimulatorViewModel.BetOnType.ALTERNATE)
                onLow = !onLow;

            GameResult res = PlaceBet(onLow, Bet, sync);
            sync.Send(q => { res.ClearProfit = ClearProfit = Model.FixedTo8(Balance - balance); }, null);
            res.SummBet = SummBet;
            sync.Send(q => { GameResultsList.Add(res); }, null);

            while (CurrentRollsCount != RollCount)
            {
                if (!CanBet(Bet))
                    return;

                if (BetOn == SimulatorViewModel.BetOnType.ALTERNATE)
                    onLow = !onLow;

                res = PlaceBet(onLow, Bet, sync);
                sync.Send(q => { res.ClearProfit = ClearProfit = Model.FixedTo8(Balance - balance); }, null);
                res.SummBet = SummBet;
                sync.Send(q => { GameResultsList.Add(res); }, null);
            }

            end?.Invoke();
            sync.Send(q => { Status = "Расчёт закончен!"; }, null);
        }

        private bool CanBet(double bet)
        {
            string s = "";

            if (bet <= 0)
                s = "Ставка слишком мала";

            if (bet > Balance)
            {
                s = "Неньги закончились =(";
            }

            if (Fuse.StopMaxSumm && SummBet >= Fuse.MaxSumm)
            {
                s = $"Сработал предохранитель на сумму ставки {SummBet:n8} : {Fuse.MaxSumm:n8}";
            }

            if (Fuse.StopOnProfit && ClearProfit >= Fuse.IfProfit)
            {
                s = $"Сработал предохранитель на профит {ClearProfit:n8} : {Fuse.IfProfit:n8}";
            }

            if (Fuse.StopOnLoss && ClearProfit < 0 && -ClearProfit >= Fuse.IfLoss)
            {
                s = $"Сработал предохранитель на проигрыш {-ClearProfit:n8} : {Fuse.IfLoss:n8}";
            }

            if (Fuse.StopMaxBet && Bet >= Fuse.MaxBet)
            {
                s = $"Сработал предохранитель на максимальную ставку {Bet:n8} : {Fuse.MaxBet:n8}";
            }

            sync.Send(q => { Status = s; }, null);

            return s == "";
        }

        public GameResult PlaceBet(bool onLow, double bet, SynchronizationContext sync)
        {
            int number = rand.Next(0, 10001);

            bet = Model.FixedTo8(bet);

            bool win = onLow ? number < lt(Multiplier) : number > gt(Multiplier);

            BetSettingModel betSetting = win ? OnWin : OnLoss;

            double profit = 0;

            sync.Send(q =>
            {
                if (win)
                {
                    double a = Model.CalcWinBet(bet, Multiplier);

                    Balance += profit = Model.FixedTo8(a);
                    winBet += a;
                }
                else
                {
                    Balance += profit = -bet;
                    lossBet += bet;
                }
            }, null);

            switch (betSetting.ChangeType)
            {
                case BetSettingModel.Change.Increase:
                    {
                        Bet = Model.FixedTo8(Bet *= betSetting.Increase);
                        break;
                    }

                case BetSettingModel.Change.Percent:
                    {
                        Bet = Model.FixedTo8(Bet * (betSetting.Percent + 1));
                        break;
                    }

                case BetSettingModel.Change.Plus:
                    {
                        Bet = Model.FixedTo8(Bet += betSetting.Plus);
                        break;
                    }

                case BetSettingModel.Change.ToBaseBet:
                    {
                        Bet = Model.FixedTo8(Bet = BaseBet);
                        break;
                    }
            }

            if (betSetting.ChangeMultiplier)
                Multiplier = betSetting.Multiplier;

            Thread.Sleep(sleep);

            sync.Send(q => { SummBet += bet; }, null);

            if (win) WinCount++; else LossCount++;

            return new GameResult
            {
                ID = ++CurrentRollsCount,
                Win = win,
                Bet = bet,
                Profit = profit,
                Number = number,
                Balance = Balance,
                BetOn = onLow ? SimulatorViewModel.BetOnType.LOW : SimulatorViewModel.BetOnType.HIGH,
            };
        }

        public void Stop()
        {
            _thread?.Abort();
            _thread = null;
        }

        private int _WinCount;
        public int WinCount
        {
            get => _WinCount;
            set
            {
                _WinCount = value;
                OnPropertyChanged();
            }
        }

        private int _LossCount;
        public int LossCount
        {
            get => _LossCount;
            set
            {
                _LossCount = value;
                OnPropertyChanged();
            }
        }

        private string _Status;
        public string Status
        {
            get => _Status;
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }

        private double _SummBet;
        public double SummBet
        {
            get => _SummBet;
            set
            {
                _SummBet = value;
                OnPropertyChanged();
            }
        }

        private double _ClearProfit;
        public double ClearProfit
        {
            get => _ClearProfit;
            set
            {
                _ClearProfit = value;
                OnPropertyChanged();
            }
        }
    }

    public class GameResult
    {
        public int ID { get; set; }
        public SimulatorViewModel.BetOnType BetOn { get; set; }
        public bool Win { get; set; }
        public int Number { get; set; }
        public double Balance { get; set; }
        public double Bet { get; set; }
        public double Profit { get; set; }
        public double ClearProfit { get; set; }
        public double SummBet { get; set; }
    }
}
