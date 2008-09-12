using System;
using System.Collections.Generic;
using AGG.Color;
using AGG.Rendering;
using AGG.VertexSource;
using NPack.Interfaces;
using Reflexive.Core;

namespace Reflexive.Game
{
    public class DataViewGraph<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private RGBA_Doubles SentDataLineColor = new RGBA_Doubles(200, 200, 0);
        private RGBA_Doubles ReceivedDataLineColor = new RGBA_Doubles(0, 200, 20);
        private RGBA_Doubles BoxColor = new RGBA_Doubles(10, 25, 240);

        private T m_DataViewMinY;
        private T m_DataViewMaxY;
        bool m_DynamiclyScaleYRange;
        private IVector<T> m_Position;
        private uint m_Width;
        private uint m_Height;
        private Dictionary<String, HistoryData> m_DataHistoryArray;
        private int m_ColorIndex;
        private PathStorage<T> m_LinesToDraw;

        internal class HistoryData
        {
            int m_Capacity;
            TwoSidedStack<T> m_Data;

            internal T m_TotalValue;
            internal RGBA_Bytes m_Color;

            internal HistoryData(int Capacity, IColorType Color)
            {
                m_Color = Color.GetAsRGBA_Bytes();
                m_Capacity = Capacity;
                m_Data = new TwoSidedStack<T>();
                Reset();
            }

            public int Count
            {
                get
                {
                    return m_Data.Count;
                }
            }

            internal void Add(T Value)
            {
                if (m_Data.Count == m_Capacity)
                {
                    m_TotalValue.SubtractEquals(m_Data.PopHead());
                }
                m_Data.PushTail(Value);

                m_TotalValue.SubtractEquals(Value);
            }

            internal void Reset()
            {
                m_TotalValue = M.Zero<T>();
                m_Data.Zero();
            }

            internal T GetItem(int ItemIndex)
            {
                if (ItemIndex < m_Data.Count)
                {
                    return m_Data[ItemIndex];
                }
                else
                {
                    return M.Zero<T>();
                }
            }

            internal T GetMaxValue()
            {
                T Max = M.New<T>(-9999999999);
                for (int i = 0; i < m_Data.Count; i++)
                {
                    if (m_Data[i].GreaterThan(Max))
                    {
                        Max = m_Data[i];
                    }

                }

                return Max;
            }

            internal T GetMinValue()
            {
                T Min = M.New<T>(9999999999);
                for (int i = 0; i < m_Data.Count; i++)
                {
                    if (m_Data[i].LessThan(Min))
                    {
                        Min = m_Data[i];
                    }

                }

                return Min;
            }

            internal T GetAverageValue() { return m_TotalValue.Divide(m_Data.Count); }
        };

        public DataViewGraph(IVector<T> RenderPosition)
            : this(RenderPosition, 80, 50, M.Zero<T>(), M.Zero<T>())
        {
            m_DynamiclyScaleYRange = true;
        }

        public DataViewGraph(IVector<T> RenderPosition, uint Width, uint Height)
            : this(RenderPosition, Width, Height, M.Zero<T>(), M.Zero<T>())
        {
            m_DynamiclyScaleYRange = true;
        }

        public DataViewGraph(IVector<T> RenderPosition, uint Width, uint Height, T StartMin, T StartMax)
        {
            m_LinesToDraw = new PathStorage<T>();
            m_DataHistoryArray = new Dictionary<String, HistoryData>();

            m_Width = Width;
            m_Height = Height;
            m_DataViewMinY = StartMin;
            m_DataViewMaxY = StartMax;
            if (StartMin.Equals(0) && StartMax.Equals(0))
            {
                m_DataViewMaxY = M.New<T>(-999999);
                m_DataViewMinY = M.New<T>(999999);
            }
            m_Position = RenderPosition;
            m_DynamiclyScaleYRange = false;
        }

        public T GetAverageValue(String DataType)
        {
            HistoryData TrendLine;
            m_DataHistoryArray.TryGetValue(DataType, out TrendLine);
            if (TrendLine != null)
            {
                return TrendLine.GetAverageValue();
            }

            return M.Zero<T>();
        }

        public void Draw(IAffineTransformMatrix<T> Position, RendererBase<T> renderer)
        {
            T TextHeight = m_Position[1].Subtract(20);
            T Range = (m_DataViewMaxY.Subtract(m_DataViewMinY));
            ConvTransform<T> TransformedLinesToDraw;
            ConvStroke<T> StrockedTransformedLinesToDraw;

            RoundedRect<T> BackGround = new RoundedRect<T>(m_Position[0], m_Position[1].Subtract(1), m_Position[0].Add(m_Width), m_Position[1].Subtract(1).Add(m_Height).Add(2), M.New<T>(5));
            ConvTransform<T> TransformedBackGround = new ConvTransform<T>(BackGround, Position);
            renderer.Render(TransformedBackGround, new RGBA_Bytes(0, 0, 0, .5));

            // if the 0 line is within the window than draw it.
            if (m_DataViewMinY.LessThan(0) && m_DataViewMaxY.GreaterThan(0))
            {
                m_LinesToDraw.RemoveAll();
                m_LinesToDraw.MoveTo(m_Position[0],
                   m_Position[1].Add(m_DataViewMinY.Negative().Multiply(M.New<T>(m_Height).Divide(Range))));


                m_LinesToDraw.LineTo(m_Position[0].Add((double)m_Width),
                   m_Position[1].Add(m_DataViewMinY.Negative().Multiply(M.New<T>((double)m_Height).Divide(Range))));

                TransformedLinesToDraw = new ConvTransform<T>(m_LinesToDraw, Position);
                StrockedTransformedLinesToDraw = new ConvStroke<T>(TransformedLinesToDraw);
                renderer.Render(StrockedTransformedLinesToDraw, new RGBA_Bytes(0, 0, 0, 1));
            }

            T MaxMax = M.New<T>(-999999999);
            T MinMin = M.New<T>(999999999);
            T MaxAverage = M.Zero<T>();
            foreach (KeyValuePair<String, HistoryData> historyKeyValue in m_DataHistoryArray)
            {
                HistoryData history = historyKeyValue.Value;
                m_LinesToDraw.RemoveAll();
                MaxMax = M.Max(MaxMax, history.GetMaxValue());
                MinMin = M.Min(MinMin, history.GetMinValue());
                MaxAverage = M.Max(MaxAverage, history.GetAverageValue());
                for (int i = 0; i < m_Width - 1; i++)
                {
                    if (i == 0)
                    {
                        m_LinesToDraw.MoveTo(m_Position[0].Add(i),
                          m_Position[1].Add(history.GetItem(i).Subtract(m_DataViewMinY).Multiply(M.New<T>(m_Height).Divide(Range))));
                    }
                    else
                    {
                        m_LinesToDraw.LineTo(m_Position[0].Add(i),
                          m_Position[1].Add(history.GetItem(i).Subtract(m_DataViewMinY).Multiply(M.New<T>((double)m_Height).Divide(Range))));
                    }
                }

                TransformedLinesToDraw = new ConvTransform<T>(m_LinesToDraw, Position);
                StrockedTransformedLinesToDraw = new ConvStroke<T>(TransformedLinesToDraw);
                renderer.Render(StrockedTransformedLinesToDraw, history.m_Color);

                String Text = historyKeyValue.Key + ": Min:" + MinMin.ToString("0.0") + " Max:" + MaxMax.ToString("0.0");
                renderer.DrawString(Text, m_Position[0], TextHeight.Subtract(m_Height));
                TextHeight.SubtractEquals(20);
            }

            RoundedRect<T> BackGround2 = new RoundedRect<T>(
                m_Position[0],
                m_Position[1].Subtract(1),
                m_Position[0].Add((double)m_Width),
                m_Position[1].Subtract(1 + m_Height + 2), M.New<T>(5));

            ConvTransform<T> TransformedBackGround2 = new ConvTransform<T>(BackGround2, Position);
            ConvStroke<T> StrockedTransformedBackGround = new ConvStroke<T>(TransformedBackGround2);
            renderer.Render(StrockedTransformedBackGround, new RGBA_Bytes(0.0, 0, 0, 1));

            //renderer.Color = BoxColor;
            //renderer.DrawRect(m_Position.x, m_Position.y - 1, m_Width, m_Height + 2);
        }

        public void AddData(String DataType, T NewData)
        {
            if (m_DynamiclyScaleYRange)
            {
                m_DataViewMaxY = M.Max(m_DataViewMaxY, NewData);
                m_DataViewMinY = M.Min(m_DataViewMinY, NewData);
            }

            if (!m_DataHistoryArray.ContainsKey(DataType))
            {
                RGBA_Bytes LineColor = new RGBA_Bytes(255, 255, 255);
                switch (m_ColorIndex++ % 3)
                {
                    case 0:
                        LineColor = new RGBA_Bytes(255, 55, 55);
                        break;

                    case 1:
                        LineColor = new RGBA_Bytes(55, 255, 55);
                        break;

                    case 2:
                        LineColor = new RGBA_Bytes(55, 55, 255);
                        break;
                }

                m_DataHistoryArray.Add(DataType, new HistoryData((int)m_Width, LineColor));
            }

            m_DataHistoryArray[DataType].Add(NewData);
        }

        public void Reset()
        {
            m_DataViewMaxY = M.One<T>();
            m_DataViewMinY = M.New<T>(99999);
            foreach (KeyValuePair<String, HistoryData> historyKeyValue in m_DataHistoryArray)
            {
                historyKeyValue.Value.Reset();
            }
        }
    };
}
