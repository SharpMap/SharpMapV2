using System;
using System.Xml;
using NPack.Interfaces;

namespace Reflexive.Game
{
    public class AssetReference<T,GameObjectType> : GameObject<T> where GameObjectType : GameObject<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        [GameData("AssetName")]
        private string m_AssetName = null;
        private GameObjectType m_AssetReference = null;

        public AssetReference(String defaultAssetName) { m_AssetName = defaultAssetName; }

        #region GameObjectStuff
        public AssetReference() { }
        public new static GameObject<T> Load(String PathName)
        {
            return GameObject<T>.Load(PathName);
        }
        #endregion

        public override void WriteGameObjectData(XmlWriter writer)
        {
            writer.WriteStartAttribute("AssetName");
            if (m_AssetName == null)
            {
                writer.WriteValue("!Default");
            }
            else
            {
                writer.WriteValue(m_AssetName);
            }
            writer.WriteEndAttribute();
        }

        public override void LoadGameObjectData(XmlReader xmlReader)
        {
            m_AssetName = xmlReader.GetAttribute("AssetName");
        }

        public GameObjectType Instance
        {
            get
            {
                if (m_AssetReference == null)
                {
                    m_AssetReference = (GameObjectType)DataAssetCache<T>.Instance.GetAsset(typeof(GameObjectType), m_AssetName);
                }
                return m_AssetReference;
            }
        }
    };
}
