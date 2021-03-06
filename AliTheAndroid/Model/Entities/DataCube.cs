using System;
using System.Collections.Generic;
using DeenGames.AliTheAndroid.Enums;

namespace DeenGames.AliTheAndroid.Model.Entities
{
    public  class DataCube : AbstractEntity
    {
        internal const int FirstDataCubeFloor = 1; // 1 = B1 = Khalifa's Message
        internal static int NumCubes { get { return cubeTexts.Count; } }

        // Buzz-words: Experiment Chamber, AllCure, prototype #37, Ameer
        // Tuple of (title, text)
        private static List<Tuple<string, string>> cubeTexts = new List<Tuple<string, string>>()
        {
            // B1 = intro
            new Tuple<string, string>("The Khalifa's Message", @"Ali, I'm dispatching you to you this space station to investigate what happened. They were researching a nanotech cure that could help us defeat The Hive before Earth is overrun. Communications went dark two weeks ago and I fear the Hive may have reached them. Find out what happened and report back to me."),
            // B2
            new Tuple<string, string>("First Day", @"Shifaa Corporation hired me to work on some sort of cure. It's exciting to start working at this space station, because the Qur'an teaches us that saving one life is like saving all of humanity!"),
            new Tuple<string, string>("Explosion", @"There was an explosion in the Experiment Chamber. AllCure prototype #37 leaked and affected a couple of the crew. The Ameer put them in quarantine. Why? Was it an accident? Or ... sabotage?"),
            new Tuple<string, string>("Weapon", @"We're not building a cure. We're building a weapon. We told the Ameer, but he didn't even blink.  He knew.  How can he support this? Doesn't he know that he will be held accountable in the Afterlife?"),
            new Tuple<string, string>("Virus", @"After working hours, I gained access to the Experiment Chamber through the vents.  I found and analyzed a blood sample with AllCure.  It's an air-borne virus. It's in the vents. It's everywhere."),
            new Tuple<string, string>("Infection", @"I cut myself on a metal fragment coming back from the Experiment Chamber. Feel strange. I'm infected. I can feel my senses heightening, muscles growing. What am I becoming?"),
            new Tuple<string, string>("Plasma Drive", @"The scientists finished AllCure - the Ameer turned them into Zugs with the prototype. They damaged our warp drive ... inert gravity waves spread everywhere."),
            new Tuple<string, string>("Ameer", @"I am the last. The last scientist, the last Zug. The Ameer holed himself up on B10. He may still be there. If I can finish my plasma cannon prototype, we may have a chance."),
            // B9
            new Tuple<string, string>("Too Late", @"The Ameer finished the AllCure, infected himself, and killed everyone. His wounds heal instantly. If I can overload the ship core with my plasma cannon, the resulting quantum plasma will vapourize everything - that abomindable AllCure ceases to exist. There are no escape pods.")
        };

        internal static DataCube EndGameCube = new DataCube(0, 0, 0, "Intercepted: Ameer's Message", "At last! The experiments were a success - I used the final version of the AllCure virus on myself, and I am indestructible! The Khalifa, the empire, and then the Hive will be no match for our armies. I sent you the genetic blueprint for replication - begin production immediately.");

        private const char DisplayCharacter = (char)240; // ≡

        public int FloorNumber { get; private set; } // 5 => B5
        public string Title { get; private set; }
        public string Text { get; private set; }
        public bool IsRead { get; set; } = false;

        public DataCube(int x, int y, int floorNumber, string title, string text) : base(x, y, DisplayCharacter, Palette.White)
        {
            this.FloorNumber = floorNumber;
            this.Title = title;
            this.Text = text;
        }

        // floor number is 2 for B2
        public static DataCube GetCube(int floorNumber, GoRogue.Coord coordinates)
        {
            if (floorNumber < FirstDataCubeFloor || floorNumber > FirstDataCubeFloor + cubeTexts.Count - 1)
            {
                throw new InvalidOperationException($"There is no data cube for B{floorNumber}");
            }

            var index = floorNumber - DataCube.FirstDataCubeFloor;
            var data = cubeTexts[index];
            return new DataCube(coordinates.X, coordinates.Y, floorNumber, data.Item1, data.Item2);
        }
    }
}