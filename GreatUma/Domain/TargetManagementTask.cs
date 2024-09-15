using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreatUma.Utils;
using GreatUma.Infrastructures;
using GreatUma.Models;
using GreatUma.Model;
using GreatUma.Infrastructure;

namespace GreatUma.Domain
{
    public class TargetManagementTask
    {
        public bool Running => CancellationTokenSource != null;
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private CancellationToken CancelToken { get; set; }
        private Scraper Scraper { get; set; }
        private TargetManager TargetManager { get; set; }
        private object LockObject { get; } = new object();
        private TargetStatusRepository TargetStatusRepository { get; set; }

        ~TargetManagementTask()
        {
            if (Scraper != null)
            {
                Scraper.Dispose();
            }
        }

        public void Run()
        {
            if (Running)
            {
                return;
            }
            LoggerWrapper.Info("Start TargetManagementTask");
            CancellationTokenSource = new CancellationTokenSource();
            CancelToken = CancellationTokenSource.Token;
            if (Scraper == null)
            {
                Scraper = new Scraper();
            }
            lock (LockObject)
            {
                if (TargetManager == null)
                {
                    TargetManager = new TargetManager(DateTime.Today);
                }
                if (TargetManager.TargetDate != DateTime.Today)
                {
                    TargetManager = new TargetManager(DateTime.Today);
                }
            }
            //Store時にエラーが起きたなどの場合に重複ベットしないためのメモ
            Task.Run(() =>
            {
                try
                {
                    if (CancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    lock (LockObject)
                    {
                        if (!TargetManager.IsInitialized)
                        {
                            TargetManager.Initialize();
                        }
                        TargetManager.Update(DateTime.Now);
                        var currentStatus = TargetStatusRepository.ReadAll();
                        currentStatus.HorseAndOddsConditionList = TargetManager.TargetList;
                        TargetStatusRepository.Store(currentStatus);
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Error(ex);
                    throw;
                }
                finally
                {
                    LoggerWrapper.Info("End TargetManagementTask");
                    CancellationTokenSource.Dispose();
                    CancellationTokenSource = null;
                }
            }, CancelToken);
        }

        public List<HorseAndOddsCondition> GetHorseAndOddsCondition()
        {
            lock (LockObject)
            {
                return TargetManager.TargetList;
            }
        }

        public void Stop()
        {
            if (!Running)
            {
                return;
            }
            CancellationTokenSource.Cancel();
        }
    }
}