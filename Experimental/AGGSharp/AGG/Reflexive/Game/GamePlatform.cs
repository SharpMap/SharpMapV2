using System;
using AGG.Color;
using AGG.Transform;
using AGG.UI;
using NPack.Interfaces;
using Reflexive.Audio;
using NPack;

namespace Reflexive.Game
{
    public class GamePlatform<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        System.Diagnostics.Stopwatch m_PotentialDrawsStopWatch = new System.Diagnostics.Stopwatch();
        IVector<T> m_PotentialDrawsBudgetPosition;
        AGG.UI.cbox_ctrl<T> m_ShowPotentialDrawsBudgetGraph;
        DataViewGraph<T> m_PotentialDrawsBudgetGraph;

        System.Diagnostics.Stopwatch m_PotentialUpdatesStopWatch = new System.Diagnostics.Stopwatch();
        IVector<T> m_PotentialUpdatesBudgetPosition;
        AGG.UI.cbox_ctrl<T> m_ShowPotentialUpdatesBudgetGraph;
        DataViewGraph<T> m_PotentialUpdatesBudgetGraph;

        System.Diagnostics.Stopwatch m_ActualDrawsStopWatch = new System.Diagnostics.Stopwatch();
        IVector<T> m_ActualDrawsBudgetPosition;
        AGG.UI.cbox_ctrl<T> m_ShowActualDrawsBudgetGraph;
        DataViewGraph<T> m_ActualDrawsBudgetGraph;

        bool m_ShowFrameRate;
        int m_LastSystemTickCount;
        double m_SecondsLeftOverFromLastUpdate;
        double m_SecondsPerUpdate;
        int m_MaxUpdatesPerDraw;
        double m_NumSecondsSinceStart;

        static String m_PotentialDrawsPerSecondString = "Potential Draws Per Second";
        static String m_ActualDrawsPerSecondString = "Actual Draws Per Second";
        static String m_PotentialUpdatesPerSecondString = "Potential Updates Per Second";

        public GamePlatform(double SecondsPerUpdate, int MaxUpdatesPerDraw, PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_ShowFrameRate = true;
            m_SecondsPerUpdate = SecondsPerUpdate;
            m_MaxUpdatesPerDraw = MaxUpdatesPerDraw;
            wait_mode(false);

            AudioSystem<T>.Startup();
        }

        public bool ShowFrameRate
        {
            get { return m_ShowFrameRate; }
            set
            {
                m_ShowFrameRate = value;
                if (m_ShowFrameRate)
                {
                    m_ShowActualDrawsBudgetGraph.Visible = true;
                    m_ShowPotentialUpdatesBudgetGraph.Visible = true;
                    m_ShowPotentialDrawsBudgetGraph.Visible = true;
                }
                else
                {
                    m_ShowActualDrawsBudgetGraph.Visible = false;
                    m_ShowPotentialUpdatesBudgetGraph.Visible = false;
                    m_ShowPotentialDrawsBudgetGraph.Visible = false;
                }
            }
        }

        public virtual void OnUpdate(double NumSecondsPassed)
        {
        }

        public override void OnInitialize()
        {
            int FrameRateOffset = -15;
            RGBA_Doubles FrameRateControlColor = new RGBA_Doubles(.2, .2, .2, 1);

            m_PotentialDrawsBudgetPosition = MatrixFactory<T>.CreateVector2D(M.New<T>(10), height().Add(FrameRateOffset));
            m_ShowPotentialDrawsBudgetGraph = new AGG.UI.cbox_ctrl<T>(m_PotentialDrawsBudgetPosition[0], m_PotentialDrawsBudgetPosition[1], "D:0.0");
            m_ShowPotentialDrawsBudgetGraph.text_color(FrameRateControlColor);
            m_ShowPotentialDrawsBudgetGraph.inactive_color(FrameRateControlColor);
            AddChild(m_ShowPotentialDrawsBudgetGraph);
            m_PotentialDrawsBudgetGraph = new DataViewGraph<T>(m_PotentialDrawsBudgetPosition, 100, 100);

            m_PotentialUpdatesBudgetPosition = MatrixFactory<T>.CreateVector2D(M.New<T>(115), height().Add(FrameRateOffset));
            m_ShowPotentialUpdatesBudgetGraph = new AGG.UI.cbox_ctrl<T>(m_PotentialUpdatesBudgetPosition[0], m_PotentialUpdatesBudgetPosition[1], "U:0.0");
            m_ShowPotentialUpdatesBudgetGraph.text_color(FrameRateControlColor);
            m_ShowPotentialUpdatesBudgetGraph.inactive_color(FrameRateControlColor);
            AddChild(m_ShowPotentialUpdatesBudgetGraph);
            m_PotentialUpdatesBudgetGraph = new DataViewGraph<T>(m_PotentialUpdatesBudgetPosition, 100, 100);

            m_ActualDrawsBudgetPosition = MatrixFactory<T>.CreateVector2D(M.New<T>(220), height().Add(FrameRateOffset));
            m_ShowActualDrawsBudgetGraph = new AGG.UI.cbox_ctrl<T>(m_ActualDrawsBudgetPosition[0], m_ActualDrawsBudgetPosition[1], "A:0.0");
            m_ShowActualDrawsBudgetGraph.text_color(FrameRateControlColor);
            m_ShowActualDrawsBudgetGraph.inactive_color(FrameRateControlColor);
            AddChild(m_ShowActualDrawsBudgetGraph);
            m_ActualDrawsBudgetGraph = new DataViewGraph<T>(m_ActualDrawsBudgetPosition, 100, 100);

            base.OnInitialize();
        }

        public override void OnClosed()
        {
            AudioSystem<T>.Shutdown();

            base.OnClosed();
        }

        public override void OnDraw()
        {
            base.OnDraw();
            if (m_ShowFrameRate)
            {
                T GraphOffsetY = M.New<T>(-105);
                if (m_ShowPotentialDrawsBudgetGraph.status())
                {
                    IAffineTransformMatrix<T> Position = MatrixFactory<T>.NewTranslation(M.Zero<T>(), GraphOffsetY);
                    m_PotentialDrawsBudgetGraph.Draw(Position, GetRenderer());
                }

                if (m_ShowPotentialUpdatesBudgetGraph.status())
                {
                    IAffineTransformMatrix<T> Position = MatrixFactory<T>.NewTranslation(M.Zero<T>(), GraphOffsetY);
                    m_PotentialUpdatesBudgetGraph.Draw(Position, GetRenderer());
                }

                if (m_ShowActualDrawsBudgetGraph.status())
                {
                    IAffineTransformMatrix<T> Position = MatrixFactory<T>.NewTranslation(M.Zero<T>(), GraphOffsetY);
                    m_ActualDrawsBudgetGraph.Draw(Position, GetRenderer());
                }
            }
        }

        public override void OnIdle()
        {
            double NumSecondsPassedSinceLastUpdate = 0.0f;

            int ThisSystemTickCount = Environment.TickCount;

            // handle the counter rolling over
            if (ThisSystemTickCount < m_LastSystemTickCount)
            {
                m_LastSystemTickCount = ThisSystemTickCount;
            }

            // figure out how many seconds have passed
            NumSecondsPassedSinceLastUpdate = (double)((ThisSystemTickCount - m_LastSystemTickCount) / 1000.0f);

            // add to it what we had left over from last time.
            NumSecondsPassedSinceLastUpdate += m_SecondsLeftOverFromLastUpdate;

            // limit it to the max that we are willing to consider
            double MaxSecondsToCatchUpOn = m_MaxUpdatesPerDraw * m_SecondsPerUpdate;
            if (NumSecondsPassedSinceLastUpdate > MaxSecondsToCatchUpOn)
            {
                NumSecondsPassedSinceLastUpdate = MaxSecondsToCatchUpOn;
                m_SecondsLeftOverFromLastUpdate = 0.0f;
            }

            // Reset our last tick count. Do this as soon as we can, to make the time more accurate.
            m_LastSystemTickCount = ThisSystemTickCount;

            bool WasUpdate = false;

            // if enough time has gone by that we are willing to do an update
            while (NumSecondsPassedSinceLastUpdate >= m_SecondsPerUpdate)
            {
                WasUpdate = true;

                m_PotentialUpdatesStopWatch.Reset();
                m_PotentialUpdatesStopWatch.Start();
                // call update with time slices that are as big as m_SecondsPerUpdate
                OnUpdate(m_SecondsPerUpdate);
                m_PotentialUpdatesStopWatch.Stop();
                double Seconds = (double)(m_PotentialUpdatesStopWatch.Elapsed.TotalMilliseconds / 1000);
                if (Seconds == 0) Seconds = 1;
                m_PotentialUpdatesBudgetGraph.AddData(m_PotentialUpdatesPerSecondString, M.New<T>(1.0 / Seconds));
                string Lable = string.Format("U:{0:F2}", m_PotentialUpdatesBudgetGraph.GetAverageValue(m_PotentialUpdatesPerSecondString));
                m_ShowPotentialUpdatesBudgetGraph.label(Lable);

                m_NumSecondsSinceStart += m_SecondsPerUpdate;
                // take out the amount of time we updated and check again
                NumSecondsPassedSinceLastUpdate -= m_SecondsPerUpdate;
            }

            // if there was an update do a draw
            if (WasUpdate)
            {
                m_PotentialDrawsStopWatch.Reset();
                m_PotentialDrawsStopWatch.Start();
                OnDraw();
                update_window();
                m_PotentialDrawsStopWatch.Stop();
                double Seconds = (double)(m_PotentialDrawsStopWatch.Elapsed.TotalMilliseconds / 1000);
                if (Seconds == 0) Seconds = 1;
                m_PotentialDrawsBudgetGraph.AddData(m_PotentialDrawsPerSecondString, M.New<T>(1.0 / Seconds));
                string Lable = string.Format("D:{0:F2}", m_PotentialDrawsBudgetGraph.GetAverageValue(m_PotentialDrawsPerSecondString));
                m_ShowPotentialDrawsBudgetGraph.label(Lable);


                m_ActualDrawsStopWatch.Stop();
                Seconds = (double)(m_ActualDrawsStopWatch.Elapsed.TotalMilliseconds / 1000);
                if (Seconds == 0) Seconds = 1;
                m_ActualDrawsBudgetGraph.AddData(m_ActualDrawsPerSecondString, M.New<T>(1.0 / Seconds));
                Lable = string.Format("A:{0:F2}", m_ActualDrawsBudgetGraph.GetAverageValue(m_ActualDrawsPerSecondString));
                m_ShowActualDrawsBudgetGraph.label(Lable);
                m_ActualDrawsStopWatch.Reset();
                m_ActualDrawsStopWatch.Start();
            }
            else // if there is more than 3 ms before the next update could happen then sleep for 1 ms.
            {
                double ThreeMiliSeconds = 3 / 1000.0f;
                if (ThreeMiliSeconds < m_SecondsPerUpdate - NumSecondsPassedSinceLastUpdate)
                {
                    System.Threading.Thread.Sleep(1);
                }
            }

            // remember the time that we didn't use up yet
            m_SecondsLeftOverFromLastUpdate = NumSecondsPassedSinceLastUpdate;
        }
    }
}
