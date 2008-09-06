
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *
 *  Original notices below.
 * 
 */
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2007 Lars Brubaker
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: larsbrubaker@gmail.com
//----------------------------------------------------------------------------
// Description:	A simple performance timer.
//*********************************************************************************************************************

namespace AGG
{
#if use_timers
    public class CNamedTimer
    {
//         [DllImport("Kernel32.dll")]
//         private static extern bool QueryPerformanceCounter(
//             out long lpPerformanceCount);
// 
//         [DllImport("Kernel32.dll")]
//         private static extern bool QueryPerformanceFrequency(
//             out long lpFrequency);

        internal long m_LastStartTime;
        internal uint m_RecurseLevel;
        internal uint m_Counter;

        internal String m_Name;

        internal double m_TotalTime;
        internal double m_TotalTimeExcludingSubroutines;
        static long s_Frequency = 1000;

        public CNamedTimer(string Name)
        {
            Reset();
            m_Name = Name;

//             if (frequency == 0)
//             {
//                 QueryPerformanceFrequency(out frequency);
//             }

            CExecutionTimer.Instance.AddTimer(this);
        }

        public void Start()
        {
            if (m_RecurseLevel != 0)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                long Now = Environment.TickCount;
                m_LastStartTime = Now;
                CExecutionTimer.Instance.Starting(this);
                m_Counter++;
            }

            m_RecurseLevel++;
        }

        public void Stop()
        {
            if (m_LastStartTime == 0 || m_RecurseLevel == 0)
            {
                // You tried to exit without ever entering?
                throw new System.InvalidOperationException();
            }

            m_RecurseLevel--;
            if (m_RecurseLevel == 0)
            {
                long NowTime = Environment.TickCount;
                //QueryPerformanceCounter(out NowTime);
                long TotalTime = NowTime - m_LastStartTime;
                m_LastStartTime = 0;

                double TimeToAdd = (double)((double)TotalTime / (double)s_Frequency);
                m_TotalTime += TimeToAdd;
                m_TotalTimeExcludingSubroutines += TimeToAdd;
                CExecutionTimer.Instance.Stoping(this, TimeToAdd);
            }
        }

        internal void Reset()
        {
            m_LastStartTime = 0;
            m_RecurseLevel = 0;
            m_Counter = 0;
            m_TotalTime = 0.0f;
            m_TotalTimeExcludingSubroutines = 0.0f;
        }

        public double GetTotalSeconds()
        {
            return m_TotalTime;
        }

        public double GetTotalSecondsExcludingSubroutines()
        {
            return m_TotalTimeExcludingSubroutines;
        }
    };

    public sealed class CExecutionTimer
    {
        List<CNamedTimer> m_CallStack;
        List<CNamedTimer> m_NamedTimerArray;
        static readonly CExecutionTimer s_ExecutionTimer = new CExecutionTimer();

        private CExecutionTimer()
        {
            m_NamedTimerArray = new List<CNamedTimer>();
            m_CallStack = new List<CNamedTimer>();
        }

        public static CExecutionTimer Instance
        {
            get
            {
                return s_ExecutionTimer;
            }
        }

        internal void Starting(CNamedTimer Timer)
        {
            m_CallStack.Add(Timer);
        }

        internal void Stoping(CNamedTimer Timer, double TimeThisRun)
        {
            int Count = m_CallStack.Count;
            if(Count > 1)
            {
                m_CallStack[Count - 2].m_TotalTimeExcludingSubroutines -= TimeThisRun;
            }
            m_CallStack.RemoveAt(Count - 1);
        }

        internal void AddTimer(CNamedTimer Timer)
        {
            m_NamedTimerArray.Add(Timer);
        }

        public void Reset()
        {
            foreach (CNamedTimer NamedTimer in m_NamedTimerArray)
            {
                NamedTimer.Reset();
            }
        }

	    public void AppendResultsToFile(String Name, double TotalTime)
        {
            FileStream file;
            file = new FileStream(Name, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(file);

            sw.Write("***************************************\n");

            sw.Write("Total  | No Subs| %Total |%No Subs| Name\n");

            foreach (CNamedTimer NamedTimer in m_NamedTimerArray)
            {
                if (NamedTimer.GetTotalSeconds() > 0)
                {
                    String OutString;

                    OutString = String.Format("{0:0.0000}", NamedTimer.GetTotalSeconds())
                        + " | " + String.Format("{0:0.0000}", NamedTimer.GetTotalSecondsExcludingSubroutines())
                        + " | " + String.Format("{0:00.00}", System.Math.Min(99.99, NamedTimer.GetTotalSeconds() / TotalTime * 100)) + "%"
                        + " | " + String.Format("{0:00.00}", NamedTimer.GetTotalSecondsExcludingSubroutines() / TotalTime * 100) + "%"
                        + " | "
                        + NamedTimer.m_Name;

                    OutString += " (" + NamedTimer.m_Counter.ToString() + ")\n";
                    sw.Write(OutString);
                }
            }

            sw.Write("\n\n");

            sw.Close();
            file.Close();
        }
    };
#endif
}
