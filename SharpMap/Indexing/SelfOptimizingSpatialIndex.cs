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
using System.Collections.Generic;
using System.Threading;

namespace SharpMap.Indexing
{
    [Flags]
    public enum RestructureOpportunity
    {
        Default = 0,
        OnInsert = 1,
        OnUserIdle = 2,
        OnMachineIdle = 4,
        Periodic = 8
    }

    public struct RestructuringHuristic
    {
        private RestructureOpportunity _whenToRestructure;
        private double _period;
        private double _executionPercentage;

        public RestructuringHuristic(RestructureOpportunity whenToRestructure, double executionPercentage)
            : this(whenToRestructure, executionPercentage, -1) { }

        public RestructuringHuristic(RestructureOpportunity whenToRestructure, double executionPercentage, double period)
        {
            _whenToRestructure = whenToRestructure;
            _executionPercentage = executionPercentage;
            _period = period;
        }

        public RestructureOpportunity WhenToRestructure
        {
            get { return _whenToRestructure; }
        }

        public double ExecutionPercentage
        {
            get { return _executionPercentage; }
        }

        public double Period
        {
            get { return _period; }
        }
    }

    public sealed class SelfOptimizingDynamicSpatialIndex : DynamicRTree
    {
        private IIndexRestructureStrategy _restructureStrategy;
        private RestructuringHuristic _restructuringHeuristic;
        private EventWaitHandle _insertedEvent;
        private EventWaitHandle _userIdleEvent;
        private EventWaitHandle _machineIdleEvent;
        private Thread _restructureThread;
        private Thread _pollIdleThread;
        private int _periodMilliseconds = -1;

        public SelfOptimizingDynamicSpatialIndex(IIndexRestructureStrategy restructureStrategy, RestructuringHuristic restructureHeuristic, IEntryInsertStrategy insertStrategy, INodeSplitStrategy nodeSplitStrategy, DynamicRTreeHeuristic indexHeuristic)
            : base(insertStrategy, nodeSplitStrategy, indexHeuristic)
        {
            _periodMilliseconds = (int)(restructureHeuristic.Period / 1000.0);
            _restructureThread = new Thread(new ThreadStart(doRestructure));
            _pollIdleThread = new Thread(new ThreadStart(checkIdleness));
            _restructureStrategy = restructureStrategy;
            _restructuringHeuristic = restructureHeuristic;
            _insertedEvent = new AutoResetEvent(false);
            _userIdleEvent = new AutoResetEvent(false);
            _machineIdleEvent = new AutoResetEvent(false);
            _restructureThread.Start();
            if(restructureHeuristic.WhenToRestructure == RestructureOpportunity.OnMachineIdle || restructureHeuristic.WhenToRestructure == RestructureOpportunity.OnUserIdle)
                _pollIdleThread.Start();
        }

        #region Resource management

        ~SelfOptimizingDynamicSpatialIndex()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            base.Dispose(disposing);

            if (disposing)
            {
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        public RestructuringHuristic RestructuringHeuristic
        {
            get { return _restructuringHeuristic; }
        }

        public override void Insert(uint key, SharpMap.Geometries.BoundingBox region)
        {
            base.Insert(key, region);
            _insertedEvent.Set();
        }

        private void restructure(object state)
        {
            switch (RestructuringHeuristic.WhenToRestructure)
            {
                case RestructureOpportunity.OnInsert:
                    _insertedEvent.Set();
                    break;
                case RestructureOpportunity.OnUserIdle:
                    _userIdleEvent.Set();
                    break;
                case RestructureOpportunity.OnMachineIdle:
                    _machineIdleEvent.Set();
                    break;
                case RestructureOpportunity.Periodic:
                    break;
                case RestructureOpportunity.Default:
                default:
                    break;
            }
        }

        private void checkIdleness()
        {
            if (isUserIdle())
                _userIdleEvent.Set();

            if (isMachineIdle())
                _machineIdleEvent.Set();
        }

        private bool isUserIdle()
        {
            // TODO: Implement user idle detection
            throw new NotImplementedException();
        }

        private bool isMachineIdle()
        {
            // TODO: Implement machine idle detection
            throw new NotImplementedException();
        }

        private void doRestructure()
        {
            WaitHandle[] events = new WaitHandle[] { _insertedEvent, _userIdleEvent, _machineIdleEvent };
            Random executionProbability = new Random();
            while (!Disposed)
            {
                WaitHandle.WaitAny(events, _periodMilliseconds, false);
                if (Disposed)
                    return;

                if(_restructuringHeuristic.ExecutionPercentage >= executionProbability.NextDouble() * 100.0)
                    _restructureStrategy.RestructureNode(Root);
            }
        }
    }
}
