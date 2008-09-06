using System;
using System.Reflection;
using System.Xml;
using NPack;
using NPack.Interfaces;

namespace Reflexive.Game
{
    #region GameDataValueAttribute
    [AttributeUsage(AttributeTargets.Field)]
    public class GameDataNumberAttribute : GameDataAttribute
    {
        double m_MinValue;
        double m_MaxValue;
        double m_Increment;

        public GameDataNumberAttribute(String Name)
            : base(Name)
        {
            m_Increment = 1;
            m_MinValue = double.MinValue;
            m_MaxValue = double.MaxValue;
        }

        public override object ReadField(XmlReader xmlReader)
        {
            return xmlReader.ReadElementContentAsDouble();
        }

        public override void WriteField(XmlWriter xmlWriter, object fieldToWrite)
        {
            if (!fieldToWrite.GetType().IsValueType)
            {
                throw new Exception("You can only put a GameDataNumberAttribute on a ValueType.");
            }

            base.WriteField(xmlWriter, fieldToWrite);
        }

        public double Min
        {
            get
            {
                return m_MinValue;
            }
            set
            {
                m_MinValue = value;
            }
        }

        public double Max
        {
            get
            {
                return m_MaxValue;
            }
            set
            {
                m_MaxValue = value;
            }
        }

        public double Increment
        {
            get
            {
                return m_Increment;
            }
            set
            {
                m_Increment = value;
            }
        }
    };
    #endregion

    #region GameDataBoolAttribute
    [AttributeUsage(AttributeTargets.Field)]
    public class GameDataBoolAttribute : GameDataAttribute
    {
        public GameDataBoolAttribute(String Name)
            : base(Name)
        {
        }

        public override object ReadField(XmlReader xmlReader)
        {
            return xmlReader.ReadElementContentAsBoolean();
        }

        public override void WriteField(XmlWriter xmlWriter, object fieldToWrite)
        {
            if (!fieldToWrite.GetType().IsPrimitive)
            {
                throw new Exception("You can only put a GameDataBoolAttribute on a Boolean.");
            }

            base.WriteField(xmlWriter, fieldToWrite);
        }
    };
    #endregion

    #region GameDataVector2DAttribute
    [AttributeUsage(AttributeTargets.Field)]
    public class GameDataVector2DAttributeDoubleComponent : GameDataAttribute
    {
        public GameDataVector2DAttributeDoubleComponent(String Name)
            : base(Name)
        {
        }

        public override object ReadField(XmlReader xmlReader)
        {
            //this bit may be broken..jd
            IVector<DoubleComponent> newVector2D = (IVector<DoubleComponent>)GameDataAttribute.ReadTypeAttributes(xmlReader);

            string xString = xmlReader.GetAttribute("x");
            string yString = xmlReader.GetAttribute("y");
            newVector2D.Set(Convert.ToDouble(xString), Convert.ToDouble(yString));

            return newVector2D;
        }

        public override void WriteField(XmlWriter xmlWriter, object fieldToWrite)
        {
            if (!(fieldToWrite is IVector<DoubleComponent>))
            {
                throw new Exception("You can only put a GameDataVector2DAttribute on a IVector<DoubleComponent>.");
            }

            IVector<DoubleComponent> vector2DToWrite = (IVector<DoubleComponent>)fieldToWrite;
            xmlWriter.WriteStartAttribute("x");
            xmlWriter.WriteValue((double)vector2DToWrite[0]);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("y");
            xmlWriter.WriteValue((double)vector2DToWrite[1]);
            xmlWriter.WriteEndAttribute();
        }
    };
    #endregion

    #region GameDataListAttribute
    [AttributeUsage(AttributeTargets.Field)]
    public class GameDataListAttribute : GameDataAttribute
    {
        public GameDataListAttribute(String Name)
            : base(Name)
        {
        }

        public override object ReadField(XmlReader xmlReader)
        {
            object list = GameDataAttribute.ReadTypeAttributes(xmlReader);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "Item")
                    {
                        object listItem = GameDataAttribute.ReadTypeAttributes(xmlReader);
                        if (listItem is GameObject<DoubleComponent>)
                        {
                            GameObject<DoubleComponent> listGameObject = (GameObject<DoubleComponent>)listItem;
                            listGameObject.LoadGameObjectData(xmlReader);
                            MethodInfo addMethod = list.GetType().GetMethod("Add");
                            addMethod.Invoke(list, new object[] { listGameObject });
                        }
                        else
                        {
                            throw new NotImplementedException("List of non-GameObjects not deserializable");
                        }
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }

            return list;
        }

        public override void WriteField(XmlWriter xmlWriter, object fieldToWrite)
        {
            object list = fieldToWrite;

            int listCount = (int)list.GetType().GetProperty("Count").GetValue(list, null);
            for (int index = 0; index < listCount; index++)
            {
                object item = list.GetType().GetMethod("get_Item").Invoke(list, new object[] { index });
                if (item is GameObject<DoubleComponent>)
                {
                    xmlWriter.WriteStartElement("Item");

                    GameDataAttribute.WriteTypeAttributes(xmlWriter, item);

                    ((GameObject<DoubleComponent>)item).WriteGameObjectData(xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    xmlWriter.WriteValue(item);
                    xmlWriter.WriteValue(" ");
                }
            }
        }
    };
    #endregion
}
