using System;
using System.Collections.Generic;
using AGG.Rendering;
using AGG.Transform;
using NPack;
using NPack.Interfaces;
using Reflexive.Game;


namespace RockBlaster
{
    public class Playfield<T> : GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        [GameDataList("RockList")]
        private List<Entity<T>> m_RockList = new List<Entity<T>>();
        [GameDataList("BulletList")]
        private List<Entity<T>> m_BulletList = new List<Entity<T>>();

        private List<Entity<T>> m_SequenceEntityList = new List<Entity<T>>();

        [GameData("Player")]
        private Player<T> m_Player;

        COLLADA.Document m_Model = new COLLADA.Document("..\\..\\GameData\\dice.dae");

        #region GameObjectStuff
        public Playfield()
        {
        }
        public static new GameObject<T> Load(String PathName)
        {
            return GameObject<T>.Load(PathName);
        }
        #endregion

        public List<Entity<T>> BulletList
        {
            get
            {
                return m_BulletList;
            }
        }

        public List<Entity<T>> RockList
        {
            get
            {
                return m_RockList;
            }
        }

        public Player<T> Player
        {
            get
            {
                return m_Player;
            }
        }

        public void StartGame()
        {
            m_SequenceEntityList.Add(new SequenceEntity<T>(MatrixFactory<T>.CreateVector2D(20, 20)));
            m_RockList.Add(new Rock<T>(40, m_RockList));
            m_Player = new Player<T>(m_BulletList);

            m_Player.Position = MatrixFactory<T>.CreateVector2D(Entity<T>.GameWidth / 2, Entity<T>.GameHeight / 2);
        }

        public void Draw(RendererBase<T> currentRenderer)
        {
            foreach (Rock<T> aRock in m_RockList)
            {
                aRock.Draw(currentRenderer);
            }

            foreach (Bullet<T> aBullet in m_BulletList)
            {
                aBullet.Draw(currentRenderer);
            }

            foreach (SequenceEntity<T> aSequenceEntity in m_SequenceEntityList)
            {
                aSequenceEntity.Draw(currentRenderer);
            }

            m_Player.Draw(currentRenderer);
        }

        public void CollideBulletsAndRocks()
        {
            foreach (Rock<T> aRock in m_RockList)
            {
                foreach (Bullet<T> aBullet in m_BulletList)
                {
                    IVector<T> BulletRelRock = aBullet.Position.Subtract(aRock.Position);
                    T BothRadius = aRock.Radius.Add(aBullet.Radius);
                    T BothRadiusSqrd = BothRadius.Multiply(BothRadius);
                    if (BulletRelRock.GetMagnitudeSquared().LessThan(BothRadiusSqrd))
                    {
                        aRock.TakeDamage(aBullet.GiveDamage());
                        PlayerSaveInfo<T>.GetPlayerInfo().Score += 2;
                    }
                }
            }
        }

        public void CollideRocksAndPlayer()
        {
            foreach (Rock<T> aRock in m_RockList)
            {
                IVector<T> BulletRelRock = m_Player.Position.Subtract(aRock.Position);
                T BothRadius = aRock.Radius.Add(m_Player.Radius);
                T BothRadiusSqrd = BothRadius.Multiply(BothRadius);
                if (BulletRelRock.GetMagnitudeSquared().LessThan(BothRadiusSqrd))
                {
                    aRock.TakeDamage(m_Player.GiveDamage());
                    m_Player.TakeDamage(M.New<T>(20));
                }
            }
        }

        protected void RemoveDeadStuff(List<Entity<T>> listToRemoveFrom)
        {
            List<Entity<T>> RemoveList = new List<Entity<T>>();

            foreach (Entity<T> aEntity in listToRemoveFrom)
            {
                if (aEntity.Damage.GreaterThanOrEqualTo(aEntity.MaxDamage))
                {
                    RemoveList.Add(aEntity);
                }
            }

            foreach (Entity<T> aEntity in RemoveList)
            {
                aEntity.Destroying();
                listToRemoveFrom.Remove(aEntity);
            }
        }

        public void Update(double NumSecondsPassed)
        {
            foreach (Rock<T> aRock in m_RockList)
            {
                aRock.Update(NumSecondsPassed);
            }

            foreach (Bullet<T> aBullet in m_BulletList)
            {
                aBullet.Update(NumSecondsPassed);
            }

            foreach (SequenceEntity<T> aSequenceEntity in m_SequenceEntityList)
            {
                aSequenceEntity.Update(NumSecondsPassed);
            }

            m_Player.Update(NumSecondsPassed);

            CollideBulletsAndRocks();
            RemoveDeadStuff(m_RockList);

            CollideRocksAndPlayer();

            RemoveDeadStuff(m_RockList);
            RemoveDeadStuff(m_BulletList);
            RemoveDeadStuff(m_SequenceEntityList);
        }
    }
}
