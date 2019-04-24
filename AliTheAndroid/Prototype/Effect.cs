using AliTheAndroid.Enums;
using DeenGames.AliTheAndroid.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DeenGames.AliTheAndroid.Prototype
{
    public abstract class Effect : AbstractEntity
    {
        public bool IsAlive {get; set;} = true;

        protected int tickEveryMilliseconds = 0;

        private DateTime lastTickOn = DateTime.Now;
        

        public Effect(int x, int y, char character, Color color, int tickEveryMs) : base(x, y, character, color)
        {
            this.tickEveryMilliseconds = tickEveryMs;
        }

        // Returns true if we updated
        public bool OnUpdate()
        {
            var now = DateTime.Now;
            var elapsed = now - this.lastTickOn;
            if (elapsed.TotalMilliseconds >= this.tickEveryMilliseconds) {
                this.OnAction();
                this.lastTickOn = now;
                return true;
            } else {
                return false;
            }
        }

        internal abstract void OnAction();
    }

    public class Shot : Effect
    {
        public Direction Direction { get; private set; }
        private const int ShotUpdateTimeMs = 100; // 100ms
        private Func<int, int, bool, bool> isWalkableCheck;
        private Vector2 createdOnTile;

        // Not technically correct, but since shots only go one-way, this will never rebound to be false
        public bool HasMoved { get { return  this.X != createdOnTile.X || this.Y != createdOnTile.Y; }}

        public Shot(int x, int y, char character, Color color, Direction direction, Func<int, int, bool, bool> isWalkable) : base(x, y, character, Palette.Red, ShotUpdateTimeMs)
        {
            this.Direction = direction;
            this.isWalkableCheck = isWalkable;
            this.createdOnTile = new Vector2(x, y);
        }

        override internal void OnAction()
        {
            if (this.isWalkableCheck(this.X, this.Y, false))
            {
                switch (this.Direction) {
                    case Direction.Up:
                        this.Y -= 1;
                        break;
                    case Direction.Right:
                        this.X += 1;
                        break;
                    case Direction.Down:
                        this.Y += 1;
                        break;
                    case Direction.Left:
                        this.X -= 1;
                        break;
                }
            }
        }
    }

    public class Explosion : Effect
    {
        private const int LifetimeMs = 100;

        public Explosion(int x, int y) : base(x, y, '*', Palette.Orange, LifetimeMs)
        {
        }

        internal override void OnAction()
        {
            this.IsAlive = false;
        }
    }

    public class Bolt : Explosion
    {
        public Bolt(int x, int y) : base(x, y)
        {
            this.Character = '$';
            this.Color = Palette.Blue;
        }
    }

    public class Flare : Explosion
    {
        protected DateTime createdOn;

        public Flare(int x, int y) : base(x, y)
        {
            this.Character = '%';
            this.Color = Palette.Cyan;
            this.createdOn = DateTime.Now;
        }

        override internal void OnAction()
        {
            // Stay alive a bit longer so we spread properly in rooms
            this.IsAlive = (DateTime.Now - this.createdOn).TotalMilliseconds <= this.tickEveryMilliseconds * 3;
        }
    }

}