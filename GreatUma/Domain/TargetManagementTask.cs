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
        private TargetManager TargetManager { get; set; }
        private object LockObject { get; } = new object();
        public TargetConfigRepository TargetConfigRepository { get; set; }

        public void SetInitialized(bool initialized)
        {
            lock (LockObject)
            {
                TargetManager.IsInitialized = initialized;
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
            lock (LockObject)
            {
                if (TargetManager == null)
                {
                    TargetManager = new TargetManager(DateTime.Today, TargetConfigRepository);
                }
                if (TargetManager.TargetDate != DateTime.Today)
                {
                    TargetManager = new TargetManager(DateTime.Today, TargetConfigRepository);
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