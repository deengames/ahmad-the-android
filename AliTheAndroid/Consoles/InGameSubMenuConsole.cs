using System;
using DeenGames.AliTheAndroid.Consoles.SubConsoleStrategies;
using DeenGames.AliTheAndroid.Enums;
using DeenGames.AliTheAndroid.Infrastructure.Common;
using DeenGames.AliTheAndroid.Model.Entities;
using DeenGames.AliTheAndroid.Model.Events;
using Ninject;

namespace DeenGames.AliTheAndroid.Consoles
{
    public class InGameSubMenuConsole : AbstractConsole
    {
        internal static bool IsOpen = false;

        
        private const int DefaultWidth = 35;
        private const int DefaultHeight = 20;
        private readonly SadConsole.Cell BorderCell = new SadConsole.Cell(Palette.White, Palette.White, ' ');        
        private IKeyboard keyboard;
        private ISubConsoleStrategy currentStrategy;
        private Player player;

        public InGameSubMenuConsole(Player player) : base(DefaultWidth, DefaultHeight)
        {
            this.player = player;
            this.currentStrategy = new TopLevelMenuStrategy(DefaultWidth, DefaultHeight, player);
            this.IsFocused = true;
            this.keyboard = DependencyInjection.kernel.Get<IKeyboard>();
            this.keyboard.Clear();
            InGameSubMenuConsole.IsOpen = true;
            EventBus.Instance.AddListener(GameEvent.ChangeSubMenu, (strategyType) =>
            {
                Type type = (Type)strategyType;
                // Constructor: width, height, player
                ISubConsoleStrategy instance = Activator.CreateInstance(type, new object[] { DefaultWidth, DefaultHeight, player }) as ISubConsoleStrategy;
                this.currentStrategy = instance;
            });
        }

        override public void Update(System.TimeSpan delta)
        {
            if (this.ShouldProcessInput())
            {
                if (this.currentStrategy is TopLevelMenuStrategy && this.keyboard.IsKeyPressed(Key.Escape))
                {
                    EventBus.Instance.Broadcast(GameEvent.HideSubMenu, this);
                }
                else
                {
                    this.RedrawEverything();
                    this.currentStrategy.ProcessInput(this.keyboard);
                }
            }
        }

        private void RedrawEverything()
        {
            this.Fill(Palette.BlackAlmost, Palette.BlackAlmost, ' ');
            this.DrawBox(new Microsoft.Xna.Framework.Rectangle(0, 0, this.Width, this.Height), BorderCell);
            this.currentStrategy.Draw(this);
        }

        private enum SubMenuState {
            ShowOptions,
            ShowingDataCubes
        }
    }
}