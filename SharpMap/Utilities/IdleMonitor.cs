// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SharpMap.Utilities
{
    public class IdleMonitor : IDisposable
    {
        public static readonly Int32 MachineUtilizationConsideredIdle = 3;

        private Int32 _userIdleThresholdSeconds;
        private Int32 _machineIdleThresholdSeconds;
        private Int32 _checkIdleFrequencyInSeconds = 1;
        private Thread _pollIdleThread;
        private EventWaitHandle _terminateEvent;
        private bool _isDisposed;
        private Int32 _terminating = 0;
        private bool _wasUserIdle = false;
        private bool _wasMachineIdle = false;

        public IdleMonitor(Int32 userIdleThresholdInSeconds, Int32 machineIdleThresholdInSeconds)
        {
            _userIdleThresholdSeconds = userIdleThresholdInSeconds;
            _machineIdleThresholdSeconds = machineIdleThresholdInSeconds;

            _terminateEvent = new ManualResetEvent(false);
        }

        #region Dispose pattern

        ~IdleMonitor()
        {
            dispose(false);
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        private void dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
            }

            terminateThreads();
            _terminateEvent.Close();
        }

        #endregion

        public event EventHandler UserIdle;
        public event EventHandler UserBusy;
        public event EventHandler MachineIdle;
        public event EventHandler MachineBusy;

        public void StartMonitoring()
        {
            _pollIdleThread = new Thread(checkIdleness);
            _pollIdleThread.Start();
        }

        public void StopMonitoring()
        {
            _terminateEvent.Set();
        }

        public Int32 CheckIdleFrequencyInSeconds
        {
            get { return _checkIdleFrequencyInSeconds; }
            set { Interlocked.Exchange(ref _checkIdleFrequencyInSeconds, value); }
        }

        /// <summary>
        /// Gets or sets the amount of time in seconds during which no 
        /// user activity is detected before a user is considered idle.
        /// </summary>
        public Int32 UserIdleThresholdInSeconds
        {
            get { return _userIdleThresholdSeconds; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value,
                                                          "UserIdleThresholdInSeconds cannot be negative.");
                }

                _userIdleThresholdSeconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time in seconds during which 
        /// machine activity is less than or equal to <see cref="MachineUtilizationConsideredIdle"/> 
        /// before a machine is considered idle.
        /// </summary>
        public Int32 MachineIdleThresholdInSeconds
        {
            get { return _machineIdleThresholdSeconds; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value,
                                                          "MachineIdleThresholdInSeconds cannot be negative.");
                }

                _machineIdleThresholdSeconds = value;
            }
        }

        public static bool IsUserIdle(Int32 userIdleThresholdInSeconds)
        {
            LastInputInfo info = LastInputInfo.Create();

            if (GetLastInputInfo(ref info))
            {
                Int32 idleTicks = 0;
                Int32 tickCount = Environment.TickCount;

                if (tickCount - info.dwTime < 0)
                {
                    Int32 tickCountWrapDifference = Int32.MaxValue - info.dwTime;
                    idleTicks = tickCountWrapDifference + tickCount;
                }
                else
                {
                    idleTicks = tickCount - info.dwTime;
                }

                return idleTicks > userIdleThresholdInSeconds*1000;
            }

            return false;
        }

        public bool IsMachineIdle(Int32 machineIdleThresholdInSeconds, Int32 machineUtilizationConsideredIdle)
        {
            // TODO: Implement machine idle detection
            throw new NotImplementedException();
        }

        private void checkIdleness()
        {
            while (Thread.VolatileRead(ref _terminating) == 0)
            {
                Int32 userIdleThreshold = Thread.VolatileRead(ref _userIdleThresholdSeconds);

                if (IsUserIdle(userIdleThreshold))
                {
                    onUserIdle();
                }
                else
                {
                    onUserBusy();
                }

                Int32 machineIdleThreshold = Thread.VolatileRead(ref _machineIdleThresholdSeconds);

                if (IsMachineIdle(machineIdleThreshold, MachineUtilizationConsideredIdle))
                {
                    onMachineIdle();
                }
                else
                {
                    onMachineBusy();
                }

                Int32 sleepTime = Thread.VolatileRead(ref _checkIdleFrequencyInSeconds);
                Thread.Sleep(sleepTime*1000);
            }
        }

        private void onMachineIdle()
        {
            if (!_wasMachineIdle)
            {
                _wasMachineIdle = true;
                EventHandler e = MachineIdle;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        private void onMachineBusy()
        {
            if (_wasMachineIdle)
            {
                _wasMachineIdle = false;
                EventHandler e = MachineBusy;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        private void onUserIdle()
        {
            if (!_wasUserIdle)
            {
                _wasUserIdle = true;
                EventHandler e = UserIdle;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        private void onUserBusy()
        {
            if (_wasUserIdle)
            {
                _wasUserIdle = false;
                EventHandler e = UserBusy;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        private void terminateThreads()
        {
#pragma warning disable 420
            Interlocked.Increment(ref _terminating);
            _terminateEvent.Set();
#pragma warning restore 420

            if (_pollIdleThread.IsAlive && !_pollIdleThread.Join(5000))
            {
                _pollIdleThread.Abort();
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo lastInput);

        [StructLayout(LayoutKind.Sequential)]
        private struct LastInputInfo
        {
            [MarshalAs(UnmanagedType.U4)] public Int32 cbSize;
            [MarshalAs(UnmanagedType.U4)] public Int32 dwTime;

            public static LastInputInfo Create()
            {
                LastInputInfo info = new LastInputInfo();
                info.cbSize = Marshal.SizeOf(typeof (LastInputInfo));
                return info;
            }
        }
    }
}