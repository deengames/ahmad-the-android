using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GoRogue.MapViews;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
using Global = SadConsole.Global;
using AliTheAndroid.Enums;
using GoRogue.Pathing;
using DeenGames.AliTheAndroid.Model.Entities;
using DeenGames.AliTheAndroid.Enums;
using DeenGames.AliTheAndroid.Model;

namespace DeenGames.AliTheAndroid.Consoles
{
    public class CoreGameConsole : SadConsole.Console
    {
        private const int RotatePowerUpColorEveryMilliseconds = 200;
        private TimeSpan gameTime;
        private Dungeon dungeon;

        public CoreGameConsole(int width, int height) : base(width, height)
        {
            this.dungeon = new Dungeon(width, height);
            this.dungeon.Generate();
        }

        override public void Update(TimeSpan delta)
        {
            this.dungeon.Update(delta);
            gameTime += delta;
            this.RedrawEverything();
        }

        private void RedrawEverything()
        {
            this.Fill(Palette.BlackAlmost, Palette.BlackAlmost, ' ');

            // One day, I will do better. One day, I will efficiently draw only what changed!
            for (var y = 0; y < this.dungeon.Height; y++)
            {
                for (var x = 0; x < this.dungeon.Width; x++)
                {
                    if (this.dungeon.CurrentFloor.IsInPlayerFov(x, y))
                    {
                        this.SetGlyph(x, y, '.', Palette.LightGrey);
                    }
                    else if (this.dungeon.CurrentFloor.IsSeen(x, y))
                    {
                        this.SetGlyph(x, y, '.', Palette.Grey);
                    }
                }
            }

            foreach (var residue in this.dungeon.CurrentFloor.PlasmaResidue) {
                if (this.dungeon.CurrentFloor.IsInPlayerFov(residue.X, residue.Y)) {
                    this.SetGlyph(residue.X, residue.Y, residue.Character, residue.Color);
                }
            }

            var allWalls = this.dungeon.CurrentFloor.Walls.Union(this.dungeon.CurrentFloor.FakeWalls);

            foreach (var wall in allWalls)
            {
                var x = wall.X;
                var y = wall.Y;

                var colour = Options.ShowFakeWalls && this.dungeon.CurrentFloor.FakeWalls.Contains(wall) ? Palette.Blue : Palette.LightGrey;

                if (this.dungeon.CurrentFloor.IsInPlayerFov(x, y))
                {
                    this.SetGlyph(wall.X, wall.Y, wall.Character, colour);
                }
                else if (this.dungeon.CurrentFloor.IsSeen(x, y))
                {
                  this.SetGlyph(wall.X, wall.Y, wall.Character, colour);
                }
            }

            foreach (var chasm in this.dungeon.CurrentFloor.Chasms) {
                if (this.dungeon.CurrentFloor.IsInPlayerFov(chasm.X, chasm.Y)) {
                    this.SetGlyph(chasm.X, chasm.Y, chasm.Character, chasm.Color);
                } else if (this.dungeon.CurrentFloor.IsSeen(chasm.X, chasm.Y)) {
                    this.SetGlyph(chasm.X, chasm.Y, chasm.Character, Palette.Grey);
                }
            }

            
            foreach (var door in this.dungeon.CurrentFloor.Doors)
            {
                var x = door.X;
                var y = door.Y;

                if (this.dungeon.CurrentFloor.IsInPlayerFov(x, y))
                {
                    this.SetGlyph(x, y, door.Character, door.Color);
                }
                else if (this.dungeon.CurrentFloor.IsSeen(x, y))
                {
                  this.SetGlyph(x, y, door.Character, Palette.Grey);
                }
            }
            
            foreach (var wave in this.dungeon.CurrentFloor.GravityWaves) {
                if (this.dungeon.CurrentFloor.IsInPlayerFov(wave.X, wave.Y)) {
                    this.SetGlyph(wave.X, wave.Y, wave.Character, wave.Color);
                }
            }

            foreach (var monster in this.dungeon.CurrentFloor.Monsters)
            {                
                if (this.dungeon.CurrentFloor.IsInPlayerFov(monster.X, monster.Y))
                {
                    var character = monster.Character;

                    this.SetGlyph(monster.X, monster.Y, character, monster.Color);
                    
                    if (monster.CurrentHealth < monster.TotalHealth) {
                        this.SetGlyph(monster.X, monster.Y, character, Palette.Orange);
                    }
                }
            }

            foreach (var effect in this.dungeon.CurrentFloor.EffectEntities) {
                if (this.dungeon.CurrentFloor.IsInPlayerFov(effect.X, effect.Y)) {
                    this.SetGlyph(effect.X, effect.Y, effect.Character, effect.Color);
                }
            }

            foreach (var powerUp in this.dungeon.CurrentFloor.PowerUps) {
                if (this.dungeon.CurrentFloor.IsInPlayerFov(powerUp.X, powerUp.Y)) {
                    var elapsedSeconds = this.gameTime.TotalMilliseconds;
                    var colourIndex = (int)Math.Floor(elapsedSeconds / RotatePowerUpColorEveryMilliseconds) % PowerUp.DisplayColors.Length;
                    this.SetGlyph(powerUp.X, powerUp.Y, powerUp.Character, PowerUp.DisplayColors[colourIndex]);
                }
            }

            int stairsX = this.dungeon.CurrentFloor.StairsLocation.X;
            int stairsY = this.dungeon.CurrentFloor.StairsLocation.Y;

            if (this.dungeon.CurrentFloor.IsInPlayerFov(stairsX, stairsY) || this.dungeon.CurrentFloor.IsSeen(stairsX, stairsY)) {
                this.SetGlyph(stairsX, stairsY, '>', this.dungeon.CurrentFloor.IsInPlayerFov(stairsX, stairsY) ? Palette.White : Palette.Grey);
            }

            this.SetGlyph(this.dungeon.Player.X, this.dungeon.Player.Y, this.dungeon.Player.Character, this.dungeon.Player.Color);

            this.DrawLine(new Point(0, this.dungeon.Height - 2), new Point(this.dungeon.Width, this.dungeon.Height - 2), null, Palette.BlackAlmost, ' ');
            this.DrawLine(new Point(0, this.dungeon.Height - 1), new Point(this.dungeon.Width, this.dungeon.Height - 1), null, Palette.BlackAlmost, ' ');
            this.DrawHealthIndicators();
            this.Print(0, this.dungeon.Height - 1, this.dungeon.CurrentFloor.LatestMessage, Palette.White);
            this.Print(this.dungeon.Width - 4, this.dungeon.Height - 2, $"B{this.dungeon.CurrentFloorNum}", Palette.White);
        }

        private void DrawHealthIndicators()
        {
            var weaponString = $"{this.dungeon.Player.CurrentWeapon}";
            if (this.dungeon.Player.CurrentWeapon == Weapon.GravityCannon && !this.dungeon.Player.CanFireGravityCannon) {
                weaponString += " (charging)";
            }
            string message = $"You: {this.dungeon.Player.CurrentHealth}/{this.dungeon.Player.TotalHealth} (facing {this.dungeon.Player.DirectionFacing.ToString()}) Equipped: {weaponString}";
            
            foreach (var monster in this.dungeon.CurrentFloor.Monsters)
            {
                var distance = Math.Sqrt(Math.Pow(monster.X - this.dungeon.Player.X, 2) + Math.Pow(monster.Y - this.dungeon.Player.Y, 2));
                if (distance <= 1)
                {
                    // compact
                    message = $"{message} {monster.Character}: {monster.CurrentHealth}/{monster.TotalHealth}"; 
                }
            }

            this.Print(1, this.dungeon.Height - 2, message, Palette.White);
        }
    }
}