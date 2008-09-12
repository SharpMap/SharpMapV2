//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
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
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AGG.Color;
using AGG.Transform;
using AGG.VertexSource;
using NPack.Interfaces;
using NPack;

namespace AGG.UI
{
    public class UndoBuffer
    {
        public enum MergeType { Mergable, NotMergable };

        class UndoCheckPoint
        {
            internal ICloneable m_ObjectToUndoTo;
            internal string m_TypeOfObject;
            internal MergeType m_MergeType;

            internal UndoCheckPoint(ICloneable objectToUndoTo, string typeOfObject, MergeType mergeType)
            {
                m_ObjectToUndoTo = objectToUndoTo;
                m_TypeOfObject = typeOfObject;
                m_MergeType = mergeType;
            }
        };

        List<UndoCheckPoint> m_UndoBuffer = new List<UndoCheckPoint>();
        int m_CurrentUndoIndex = -1;
        int m_LastValidUndoIndex = -1;

        public UndoBuffer()
        {

        }

        public void Add(object objectToUndoTo, string typeOfObject, MergeType mergeType)
        {
            ICloneable cloneableObject = (ICloneable)objectToUndoTo;
            if (cloneableObject != null)
            {
                if (m_CurrentUndoIndex <= 0
                    || mergeType == MergeType.NotMergable
                    || m_UndoBuffer[m_CurrentUndoIndex].m_TypeOfObject != typeOfObject)
                {
                    m_CurrentUndoIndex++;
                }

                UndoCheckPoint newUndoCheckPoint = new UndoCheckPoint((ICloneable)cloneableObject.Clone(), typeOfObject, mergeType);
                if (m_CurrentUndoIndex < m_UndoBuffer.Count)
                {
                    m_UndoBuffer[m_CurrentUndoIndex] = newUndoCheckPoint;
                }
                else
                {
                    m_UndoBuffer.Add(newUndoCheckPoint);
                }

                m_LastValidUndoIndex = m_CurrentUndoIndex;
            }
        }

        public object GetPrevUndoObject()
        {
            if (m_CurrentUndoIndex > 0)
            {
                return m_UndoBuffer[--m_CurrentUndoIndex].m_ObjectToUndoTo.Clone();
            }

            return null;
        }

        public object GetNextRedoObject()
        {
            if (m_LastValidUndoIndex > m_CurrentUndoIndex)
            {
                m_CurrentUndoIndex++;
                return m_UndoBuffer[m_CurrentUndoIndex].m_ObjectToUndoTo.Clone();
            }

            return null;
        }
    };

    //------------------------------------------------------------------------
    public class TextEditWidget<T> : GUIWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_BorderSize;
        T m_Thickness;
        T m_CapsHeight;

        internal class UndoData : ICloneable
        {
            private UndoData(String undoString, int charIndexToInsertBefore, int selectionIndexToStartBefore, bool selecting)
            {
                m_String = undoString;
                m_Selecting = selecting;
                m_SelectionIndexToStartBefore = selectionIndexToStartBefore;
                m_CharIndexToInsertBefore = charIndexToInsertBefore;
            }

            internal UndoData(TextEditWidget<T> textEditWidget)
            {
                m_String = textEditWidget.m_TextWidget.Text;
                m_CharIndexToInsertBefore = textEditWidget.m_CharIndexToInsertBefore;
                m_SelectionIndexToStartBefore = textEditWidget.m_SelectionIndexToStartBefore;
                m_Selecting = textEditWidget.m_Selecting;
            }

            public object Clone()
            {
                UndoData clonedUndoData = new UndoData((String)m_String.Clone(), m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore, m_Selecting);
                return clonedUndoData;
            }

            String m_String;
            bool m_Selecting;
            int m_SelectionIndexToStartBefore;
            int m_CharIndexToInsertBefore;

            internal void ExtractData(TextEditWidget<T> textEditWidget)
            {
                textEditWidget.m_TextWidget.Text = m_String;
                textEditWidget.m_CharIndexToInsertBefore = m_CharIndexToInsertBefore;
                textEditWidget.m_SelectionIndexToStartBefore = m_SelectionIndexToStartBefore;
                textEditWidget.m_Selecting = m_Selecting;
            }
        };
        UndoBuffer m_UndoBuffer = new UndoBuffer();

        bool m_Selecting;
        int m_SelectionIndexToStartBefore;
        int m_CharIndexToInsertBefore;
        int m_CharIndexToAcceptAsMerging;

        IVector<T> m_BarPosition;

        T m_DesiredBarX;

        TextWidget<T> m_TextWidget;

        public TextEditWidget(string Text, RectDouble<T> bounds, double CapitalHeight)
            : this(Text, bounds, M.New<T>(CapitalHeight))
        {
        }

        public TextEditWidget(string Text, RectDouble<T> bounds, T CapitalHeight)
        {
            Bounds = bounds;

            m_TextWidget = new TextWidget<T>(Text, M.Zero<T>(), M.Zero<T>(), CapitalHeight);
            AddChild(m_TextWidget);

            m_CharIndexToInsertBefore = Text.Length;

            IAffineTransformMatrix<T> transform = GetTransform();
            IVector<T> v1 = transform.TransformVector(MatrixFactory<T>.CreateVector2D(bounds.x1, bounds.y1));
            IVector<T> v2 = transform.TransformVector(MatrixFactory<T>.CreateVector2D(bounds.x2, bounds.y2));

            SetTransform(transform);

            m_BorderSize = CapitalHeight.Multiply(.2);
            m_Thickness = CapitalHeight.Divide(8);
            m_CapsHeight = CapitalHeight;

            FixBarPosition(true);

            UndoData newUndoData = new UndoData(this);
            m_UndoBuffer.Add(newUndoData, "Initial", UndoBuffer.MergeType.NotMergable);
        }

        public bool Multiline
        {
            get
            {
                return true;
            }
        }

        public override void OnDraw()
        {
            if (m_Selecting)
            {
                IVector<T> selectPosition = m_TextWidget.GetOffsetLeftOfCharacterIndex(m_SelectionIndexToStartBefore);

                IVector<T> screenSelectStart = selectPosition;
                screenSelectStart[0].AddEquals(2);
                IVector<T> screenSelectEnd = MatrixFactory<T>.CreateVector2D(screenSelectStart[0], screenSelectStart[1].Add(Bounds.Height));
                PointToScreen(ref screenSelectStart);
                PointToScreen(ref screenSelectEnd);
                GetRenderer().Line(screenSelectStart, screenSelectEnd, new RGBA_Bytes(0, 1.0, 0));
            }

            base.OnDraw();

            RectDouble<T> boundsPlusPoint5 = Bounds;
            boundsPlusPoint5.Inflate(M.New<T>(.5));
            RoundedRect<T> borderRect = new RoundedRect<T>(boundsPlusPoint5, M.Zero<T>());
            ConvStroke<T> borderLine = new ConvStroke<T>(borderRect);
            GetRenderer().Render(borderLine, new RGBA_Bytes(0, 0, 0));

            IVector<T> screenStart = m_BarPosition;
            screenStart[0].AddEquals(2);
            IVector<T> screenEnd = MatrixFactory<T>.CreateVector2D(screenStart[0], screenStart[1].Add(Bounds.Height));
            PointToScreen(ref screenStart);
            PointToScreen(ref screenEnd);
            GetRenderer().Line(screenStart, screenEnd, new RGBA_Bytes(0, 0, 1.0));
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
        }

        public override string ToString()
        {
            // TODO: build a string that is all the text for all the lines.
            return m_TextWidget.Text;
        }

        void FixBarPosition(bool SetDesiredBarPosition)
        {
            m_BarPosition = m_TextWidget.GetOffsetLeftOfCharacterIndex(m_CharIndexToInsertBefore);
            if (SetDesiredBarPosition)
            {
                m_DesiredBarX = m_BarPosition[0];
            }
        }

        private void DeleteIndex(int startIndexInclusive)
        {
            DeleteIndexRange(startIndexInclusive, startIndexInclusive);
        }

        private void DeleteIndexRange(int startIndexInclusive, int endIndexInclusive)
        {
            int LengthToDelete = (endIndexInclusive + 1) - startIndexInclusive;
            if (LengthToDelete > 0)
            {
                StringBuilder stringBuilder = new StringBuilder(m_TextWidget.Text);
                stringBuilder.Remove(startIndexInclusive, LengthToDelete);
                m_TextWidget.Text = stringBuilder.ToString();
            }
        }

        private void DeleteSelection()
        {
            DeleteSelection(true);
        }

        private void DeleteSelection(bool createUndoMarker)
        {
            if (m_Selecting)
            {
                if (m_CharIndexToInsertBefore < m_SelectionIndexToStartBefore)
                {
                    DeleteIndexRange(m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore - 1);
                }
                else
                {
                    DeleteIndexRange(m_SelectionIndexToStartBefore, m_CharIndexToInsertBefore - 1);
                    m_CharIndexToInsertBefore = m_SelectionIndexToStartBefore;
                }

                if (createUndoMarker)
                {
                    UndoData newUndoDeleteData = new UndoData(this);
                    m_UndoBuffer.Add(newUndoDeleteData, "Delete", UndoBuffer.MergeType.NotMergable);
                }

                m_Selecting = false;
            }
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            bool SetDesiredBarPosition = true;
            bool turnOffSelection = false;
            if (keyEvent.Shift)
            {
                if (!m_Selecting)
                {
                    m_Selecting = true;
                    m_SelectionIndexToStartBefore = m_CharIndexToInsertBefore;
                }
            }
            else if (m_Selecting)
            {
                turnOffSelection = true;
            }

            switch (keyEvent.KeyCode)
            {
                case Keys.Left:
                    if (keyEvent.Control)
                    {
                        GotoBeginingOfPreviousToken();
                    }
                    else if (m_CharIndexToInsertBefore > 0)
                    {
                        if (turnOffSelection)
                        {
                            m_CharIndexToInsertBefore = Math.Min(m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore);
                        }
                        else
                        {
                            m_CharIndexToInsertBefore--;
                        }
                    }
                    keyEvent.SuppressKeyPress = true;
                    keyEvent.Handled = true;
                    break;

                case Keys.Right:
                    if (keyEvent.Control)
                    {
                        GotoBeginingOfNextToken();
                    }
                    else if (m_CharIndexToInsertBefore < m_TextWidget.Text.Length)
                    {
                        if (turnOffSelection)
                        {
                            m_CharIndexToInsertBefore = Math.Max(m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore);
                        }
                        else
                        {
                            m_CharIndexToInsertBefore++;
                        }
                    }
                    keyEvent.SuppressKeyPress = true;
                    keyEvent.Handled = true;
                    break;

                case Keys.Up:
                    if (turnOffSelection)
                    {
                        m_CharIndexToInsertBefore = Math.Min(m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore);
                    }
                    GotoLineAbove();
                    SetDesiredBarPosition = false;
                    keyEvent.SuppressKeyPress = true;
                    keyEvent.Handled = true;
                    break;

                case Keys.Down:
                    if (turnOffSelection)
                    {
                        m_CharIndexToInsertBefore = Math.Max(m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore);
                    }
                    GotoLineBelow();
                    SetDesiredBarPosition = false;
                    keyEvent.SuppressKeyPress = true;
                    keyEvent.Handled = true;
                    break;

                case Keys.Space:
                    keyEvent.Handled = true;
                    break;

                case Keys.End:
                    GotoEndOfCurrentLine();
                    keyEvent.SuppressKeyPress = true;
                    keyEvent.Handled = true;
                    break;

                case Keys.Home:
                    GotoStartOfCurrentLine();
                    keyEvent.SuppressKeyPress = true;
                    keyEvent.Handled = true;
                    break;

                case Keys.Back:
                    if (!m_Selecting
                        && m_CharIndexToInsertBefore > 0)
                    {
                        m_SelectionIndexToStartBefore = m_CharIndexToInsertBefore - 1;
                        m_Selecting = true;
                    }

                    DeleteSelection();

                    keyEvent.Handled = true;
                    keyEvent.SuppressKeyPress = true;
                    break;

                case Keys.Delete:
                    if (keyEvent.Shift)
                    {
                        CopySelection();
                        DeleteSelection();
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    else
                    {
                        if (!m_Selecting
                        && m_CharIndexToInsertBefore < m_TextWidget.Text.Length)
                        {
                            m_SelectionIndexToStartBefore = m_CharIndexToInsertBefore + 1;
                            m_Selecting = true;
                        }

                        DeleteSelection();
                    }

                    turnOffSelection = true;
                    keyEvent.Handled = true;
                    keyEvent.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    if (!Multiline)
                    {
                        // TODO: do the right thing.
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Insert:
                    if (keyEvent.Shift)
                    {
                        turnOffSelection = true;
                        PasteFromClipboard();
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    if (keyEvent.Control)
                    {
                        turnOffSelection = false;
                        CopySelection();
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;

                case Keys.X:
                    if (keyEvent.Control)
                    {
                        CopySelection();
                        DeleteSelection();
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;

                case Keys.C:
                    if (keyEvent.Control)
                    {
                        turnOffSelection = false;
                        CopySelection();
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;

                case Keys.V:
                    if (keyEvent.Control)
                    {
                        PasteFromClipboard();
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Z:
                    if (keyEvent.Control)
                    {
                        UndoData undoData = (UndoData)m_UndoBuffer.GetPrevUndoObject();
                        if (undoData != null)
                        {
                            undoData.ExtractData(this);
                        }
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Y:
                    if (keyEvent.Control)
                    {
                        UndoData undoData = (UndoData)m_UndoBuffer.GetNextRedoObject();
                        if (undoData != null)
                        {
                            undoData.ExtractData(this);
                        }
                        keyEvent.Handled = true;
                        keyEvent.SuppressKeyPress = true;
                    }
                    break;
            }

            base.OnKeyDown(keyEvent);

            FixBarPosition(SetDesiredBarPosition);

            // if we are not going to type a character, and therefore replace the selection, turn off the selection now if needed.
            if (keyEvent.SuppressKeyPress && turnOffSelection)
            {
                m_Selecting = false;
            }
        }

        private void CopySelection()
        {
            if (m_Selecting)
            {
                if (m_CharIndexToInsertBefore < m_SelectionIndexToStartBefore)
                {
                    System.Windows.Forms.Clipboard.SetText(m_TextWidget.Text.Substring(m_CharIndexToInsertBefore, m_SelectionIndexToStartBefore - m_CharIndexToInsertBefore));
                }
                else
                {
                    System.Windows.Forms.Clipboard.SetText(m_TextWidget.Text.Substring(m_SelectionIndexToStartBefore, m_CharIndexToInsertBefore - m_SelectionIndexToStartBefore));
                }
            }
            else if (Multiline)
            {
                // copy the line?
            }
        }

        private void PasteFromClipboard()
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                if (m_Selecting)
                {
                    DeleteSelection(false);
                }

                StringBuilder stringBuilder = new StringBuilder(m_TextWidget.Text);
                String stringOnClipboard = System.Windows.Forms.Clipboard.GetText();
                stringBuilder.Insert(m_CharIndexToInsertBefore, stringOnClipboard);
                m_CharIndexToInsertBefore += stringOnClipboard.Length;
                m_TextWidget.Text = stringBuilder.ToString();

                UndoData newUndoData = new UndoData(this);
                m_UndoBuffer.Add(newUndoData, "Paste", UndoBuffer.MergeType.NotMergable);
            }
        }

        public override void OnKeyPress(KeyPressEventArgs keyPressEvent)
        {
            if (m_Selecting)
            {
                DeleteSelection();
                m_Selecting = false;
            }

            StringBuilder tempString = new StringBuilder(m_TextWidget.Text);
            tempString.Insert(m_CharIndexToInsertBefore, keyPressEvent.KeyChar);
            keyPressEvent.Handled = true;
            m_CharIndexToInsertBefore++;
            m_TextWidget.Text = tempString.ToString();
            base.OnKeyPress(keyPressEvent);

            FixBarPosition(true);

            UndoData newUndoData = new UndoData(this);
            if (m_CharIndexToAcceptAsMerging == m_CharIndexToInsertBefore - 1
                && keyPressEvent.KeyChar != '\r')
            {
                m_UndoBuffer.Add(newUndoData, "Typing", UndoBuffer.MergeType.Mergable);
            }
            else
            {
                m_UndoBuffer.Add(newUndoData, "Typing", UndoBuffer.MergeType.NotMergable);
            }
            m_CharIndexToAcceptAsMerging = m_CharIndexToInsertBefore;
        }

        private int GetIndexOffset(int CharacterStartIndexInclusive, int MaxCharacterEndIndexInclusive, T DesiredPixelOffset)
        {
            int OffsetIndex = 0;
            int EndOffsetIndex = MaxCharacterEndIndexInclusive - CharacterStartIndexInclusive;
            IVector<T> offset = MatrixFactory<T>.CreateVector2D(M.Zero<T>(), M.Zero<T>());
            IVector<T> lastOffset = MatrixFactory<T>.CreateVector2D(M.Zero<T>(), M.Zero<T>());
            while (true)
            {
                m_TextWidget.GetSize(CharacterStartIndexInclusive, CharacterStartIndexInclusive + OffsetIndex, out offset);
                OffsetIndex++;
                if (offset[0].GreaterThanOrEqualTo(DesiredPixelOffset) || OffsetIndex >= EndOffsetIndex)
                {
                    if (offset[1].Abs().LessThan(.01)
                        && lastOffset[0].Subtract(DesiredPixelOffset).Abs().LessThan(offset[0].Subtract(DesiredPixelOffset).Abs()))
                    {
                        OffsetIndex--;
                    }
                    break;
                }
                lastOffset = offset;
            }

            int MaxLength = Math.Min(MaxCharacterEndIndexInclusive - CharacterStartIndexInclusive, OffsetIndex);
            return CharacterStartIndexInclusive + MaxLength;
        }

        // the '\r' is always considered to be the end of the line.
        // if startIndexInclusive == endIndexInclusive, the line is empty (other than the return)
        private void GetLineIndexesAboutCharIndex(int charIndexToGetLineIndexesFor, out int startIndexInclusive, out int endIndexInclusive)
        {
            startIndexInclusive = 0;
            endIndexInclusive = m_TextWidget.Text.Length;

            charIndexToGetLineIndexesFor = Math.Max(Math.Min(charIndexToGetLineIndexesFor, m_TextWidget.Text.Length), 0);
            // first lets find the end of the line.  Check if we are on a '\r'
            if (charIndexToGetLineIndexesFor == m_TextWidget.Text.Length
                || m_TextWidget.Text[charIndexToGetLineIndexesFor] == '\r')
            {
                // we are on the end of the line
                endIndexInclusive = charIndexToGetLineIndexesFor;
            }
            else
            {
                int endReturn = m_TextWidget.Text.IndexOf('\r', charIndexToGetLineIndexesFor + 1);
                if (endReturn != -1)
                {
                    endIndexInclusive = endReturn;
                }
            }

            // check if the line is empty (the character to our left is the '\r' on the previous line
            if (m_TextWidget.Text[endIndexInclusive - 1] == '\r')
            {
                // the line is empty the start = the end.
                startIndexInclusive = endIndexInclusive;
            }
            else
            {
                int returnAtStartOfCurrentLine = m_TextWidget.Text.LastIndexOf('\r', endIndexInclusive - 1);
                if (returnAtStartOfCurrentLine != -1)
                {
                    startIndexInclusive = returnAtStartOfCurrentLine + 1;
                }
            }
        }

        private void GotoLineAbove()
        {
            int startIndexInclusive;
            int endIndexInclusive;
            GetLineIndexesAboutCharIndex(m_CharIndexToInsertBefore, out startIndexInclusive, out endIndexInclusive);

            // now get the line above us
            int prevStartIndexInclusive;
            int prevEndIndexInclusive;
            GetLineIndexesAboutCharIndex(startIndexInclusive - 1, out prevStartIndexInclusive, out prevEndIndexInclusive);
            // we found the extents of the line above now put the cursor in the right place.
            m_CharIndexToInsertBefore = GetIndexOffset(prevStartIndexInclusive, prevEndIndexInclusive, m_DesiredBarX);
        }

        private void GotoLineBelow()
        {
            int startIndexInclusive;
            int endIndexInclusive;
            GetLineIndexesAboutCharIndex(m_CharIndexToInsertBefore, out startIndexInclusive, out endIndexInclusive);

            // now get the below above us
            int nextStartIndexInclusive;
            int nextEndIndexInclusive;
            GetLineIndexesAboutCharIndex(endIndexInclusive + 1, out nextStartIndexInclusive, out nextEndIndexInclusive);
            // we found the extents of the line above now put the cursor in the right place.
            m_CharIndexToInsertBefore = GetIndexOffset(nextStartIndexInclusive, nextEndIndexInclusive, m_DesiredBarX);
        }

        private void GotoBeginingOfNextToken()
        {
            if (m_CharIndexToInsertBefore == m_TextWidget.Text.Length)
            {
                return;
            }

            bool SkippedWiteSpace = false;
            if (m_TextWidget.Text[m_CharIndexToInsertBefore] == '\r')
            {
                m_CharIndexToInsertBefore++;
                SkippedWiteSpace = true;
            }
            else
            {
                Regex firstWhiteSpaceRegex = new Regex("\\s");
                Match firstWhiteSpace = firstWhiteSpaceRegex.Match(m_TextWidget.Text, m_CharIndexToInsertBefore);
                if (firstWhiteSpace.Success)
                {
                    SkippedWiteSpace = true;
                    m_CharIndexToInsertBefore = firstWhiteSpace.Index;
                }
            }

            if (SkippedWiteSpace)
            {
                Regex firstNonWhiteSpaceRegex = new Regex("[^\\t ]");
                Match firstNonWhiteSpace = firstNonWhiteSpaceRegex.Match(m_TextWidget.Text, m_CharIndexToInsertBefore);
                if (firstNonWhiteSpace.Success)
                {
                    m_CharIndexToInsertBefore = firstNonWhiteSpace.Index;
                }
            }
            else
            {
                GotoEndOfCurrentLine();
            }
        }

        private void GotoBeginingOfPreviousToken()
        {
            if (m_CharIndexToInsertBefore == 0)
            {
                return;
            }

            Regex firstNonWhiteSpaceRegex = new Regex("[^\\t ]", RegexOptions.RightToLeft);
            Match firstNonWhiteSpace = firstNonWhiteSpaceRegex.Match(m_TextWidget.Text, m_CharIndexToInsertBefore);
            if (firstNonWhiteSpace.Success)
            {
                if (m_TextWidget.Text[firstNonWhiteSpace.Index] == '\r')
                {
                    if (firstNonWhiteSpace.Index < m_CharIndexToInsertBefore - 1)
                    {
                        m_CharIndexToInsertBefore = firstNonWhiteSpace.Index;
                        return;
                    }
                    else
                    {
                        firstNonWhiteSpaceRegex = new Regex("[^\\t\\r ]", RegexOptions.RightToLeft);
                        firstNonWhiteSpace = firstNonWhiteSpaceRegex.Match(m_TextWidget.Text, m_CharIndexToInsertBefore);
                        if (firstNonWhiteSpace.Success)
                        {
                            m_CharIndexToInsertBefore = firstNonWhiteSpace.Index;
                        }
                    }
                }
                else
                {
                    m_CharIndexToInsertBefore = firstNonWhiteSpace.Index;
                }

                Regex firstWhiteSpaceRegex = new Regex("\\s", RegexOptions.RightToLeft);
                Match firstWhiteSpace = firstWhiteSpaceRegex.Match(m_TextWidget.Text, m_CharIndexToInsertBefore);
                if (firstWhiteSpace.Success)
                {
                    m_CharIndexToInsertBefore = firstWhiteSpace.Index + 1;
                }
                else
                {
                    GotoStartOfCurrentLine();
                }
            }
        }

        private void GotoEndOfCurrentLine()
        {
            int indexOfReturn = m_TextWidget.Text.IndexOf('\r', m_CharIndexToInsertBefore);
            if (indexOfReturn == -1)
            {
                m_CharIndexToInsertBefore = m_TextWidget.Text.Length;
            }
            else
            {
                m_CharIndexToInsertBefore = indexOfReturn;
            }
        }

        private void GotoStartOfCurrentLine()
        {
            if (m_CharIndexToInsertBefore > 0)
            {
                int indexOfReturn = m_TextWidget.Text.LastIndexOf('\r', m_CharIndexToInsertBefore - 1);
                if (indexOfReturn == -1)
                {
                    m_CharIndexToInsertBefore = 0;
                }
                else
                {
                    Regex firstNonWhiteSpaceRegex = new Regex("[^\\t ]");
                    Match firstNonWhiteSpace = firstNonWhiteSpaceRegex.Match(m_TextWidget.Text, indexOfReturn + 1);
                    if (firstNonWhiteSpace.Success)
                    {
                        if (firstNonWhiteSpace.Index < m_CharIndexToInsertBefore
                           || m_TextWidget.Text[m_CharIndexToInsertBefore - 1] == '\r')
                        {
                            m_CharIndexToInsertBefore = firstNonWhiteSpace.Index;
                            return;
                        }
                    }

                    m_CharIndexToInsertBefore = indexOfReturn + 1;
                }
            }
        }

        public override bool InRect(T x, T y)
        {
            PointToClient(ref x, ref y);
            return Bounds.HitTest(x, y);
        }
    };
}
