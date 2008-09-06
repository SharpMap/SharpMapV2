using System;
using System.IO;
using System.Reflection;
using System.Xml;
using AGG.UI;
using NPack.Interfaces;
using Reflexive.Editor;

namespace Reflexive.Game
{
    public class GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public GameObject()
        {
        }

        public virtual void WriteGameObjectData(XmlWriter xmlWriter)
        {
            Type gameObjectType = this.GetType();

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            System.Reflection.FieldInfo[] fieldsOfGameObject = gameObjectType.GetFields(bindingFlags);
            foreach (FieldInfo fieldOfGameObject in fieldsOfGameObject)
            {
                object[] gameDataAttributes = fieldOfGameObject.GetCustomAttributes(typeof(GameDataAttribute), false);
                if (gameDataAttributes.Length > 0)
                {
                    if (gameDataAttributes.Length > 1)
                    {
                        throw new Exception("You can only have one GameDataAttribute on any given Field.");
                    }

                    object objectWithAttribute = fieldOfGameObject.GetValue(this);
                    if (objectWithAttribute != null)
                    {
                        GameDataAttribute singleGameDataAttribute = (GameDataAttribute)gameDataAttributes[0];
                        String Name = singleGameDataAttribute.Name;

                        if (Name.Contains(" "))
                        {
                            throw new Exception(this.ToString() + " : '" + Name + "' has a space. Attribute names con not contain spaces.");
                        }

                        xmlWriter.WriteStartElement(Name);
                        GameDataAttribute.WriteTypeAttributes(xmlWriter, objectWithAttribute);
                        xmlWriter.WriteEndAttribute();

                        if (objectWithAttribute == null)
                        {
                            throw new Exception(this.ToString() + " : " + fieldOfGameObject.ToString() + " must have a default Value.\n"
                                + "\n"
                                + "All data marked as [GameData] must be a primitive or a struct or if a class have a DEFALT Value or filed initializer.");
                        }

                        if (objectWithAttribute is GameObject<T>)
                        {
                            ((GameObject<T>)objectWithAttribute).WriteGameObjectData(xmlWriter);
                        }
                        else
                        {
                            singleGameDataAttribute.WriteField(xmlWriter, objectWithAttribute);
                        }
                        xmlWriter.WriteEndElement();
                    }
                }
            }
        }

        public virtual void LoadGameObjectData(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    string AttributeNameForElement = xmlReader.Name;

                    BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    System.Reflection.FieldInfo[] fieldsOfGameObject = this.GetType().GetFields(bindingFlags);
                    foreach (FieldInfo fieldOfGameObject in fieldsOfGameObject)
                    {
                        object[] gameDataAttributes = fieldOfGameObject.GetCustomAttributes(typeof(GameDataAttribute), false);
                        if (gameDataAttributes.Length > 0)
                        {
                            GameDataAttribute singleGameDataAttribute = (GameDataAttribute)gameDataAttributes[0];
                            string AttributeNameForField = singleGameDataAttribute.Name;
                            if (AttributeNameForField == AttributeNameForElement)
                            {
                                if (fieldOfGameObject.FieldType.IsSubclassOf(typeof(GameObject<T>)))
                                {
                                    GameObject<T> newGameObject = (GameObject<T>)GameDataAttribute.ReadTypeAttributes(xmlReader);
                                    newGameObject.LoadGameObjectData(xmlReader);

                                    fieldOfGameObject.SetValue(this, newGameObject);
                                }
                                else
                                {
                                    object objectReadByAttribute = singleGameDataAttribute.ReadField(xmlReader);
                                    fieldOfGameObject.SetValue(this, objectReadByAttribute);
                                }
                                break;
                            }
                        }
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }

        }

        public static GameObject<T> Load(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.Name == "RootObject")
                {
                    string AssemblyString = xmlReader.GetAttribute("Assembly");
                    string TypeString = xmlReader.GetAttribute("Type");
                    Type rootType = Type.GetType(TypeString + ", " + AssemblyString);
                    GameObject<T> newGameObject = (GameObject<T>)Activator.CreateInstance(rootType);
                    newGameObject.LoadGameObjectData(xmlReader);
                    return newGameObject;
                }
            }

            return null;
        }

        public static GameObject<T> Load(String PathName)
        {
            FileStream stream;
            try
            {
                stream = File.Open(PathName + ".xml", FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            return Load(new XmlTextReader(stream));
        }

        public void SaveXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("RootObject");

            xmlWriter.WriteStartAttribute("Assembly");
            int firstComma = GetType().Assembly.ToString().IndexOf(",");
            xmlWriter.WriteValue(GetType().Assembly.ToString().Substring(0, firstComma));
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("Type");
            xmlWriter.WriteValue(GetType().ToString());
            xmlWriter.WriteEndAttribute();

            WriteGameObjectData(xmlWriter);

            xmlWriter.WriteEndElement();
        }

        public void SaveXML(String PathName)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(PathName + ".xml", System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                SaveXML(xmlWriter);
            }
        }

        public virtual void CreateEditor()
        {
            EditorWindow<T> editor = new EditorWindow<T>();
            editor.init(800, 600, (uint)(PlatformSupportAbstract<T>.WindowFlags.Risizeable));
            editor.Caption = "Editing Default Something";
            Type gameObjectType = this.GetType();
            T y_location = M.Zero<T>();

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            System.Reflection.FieldInfo[] gameObjectFields = gameObjectType.GetFields(bindingFlags);
            foreach (FieldInfo gameObjectField in gameObjectFields)
            {
                object[] TestAttributes = gameObjectField.GetCustomAttributes(typeof(GameDataAttribute), false);
                if (TestAttributes.Length > 0)
                {
                    GameDataAttribute gameDataAttribute = (GameDataAttribute)TestAttributes[0];
                    TextWidget<T> name = new TextWidget<T>(gameDataAttribute.Name, M.Zero<T>(), y_location, M.New<T>(15));
                    editor.AddChild(name);

                    object test = gameObjectField.GetValue(this);
                    TextWidget<T> value = new TextWidget<T>(test.ToString(), name.Width, y_location, M.New<T>(15));
                    editor.AddChild(value);

                    y_location.AddEquals(name.Height);
                }
            }
        }
    };
}
