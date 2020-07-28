using BCA_StoryMode.Models;
using hub_client.Windows.StoryMode.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace hub_client.Windows.StoryMode.Stuff
{
    public class CharacterBody : Character
    {
        public CharacterSprites Sprites { get; set; }
        public DirectionEnum Direction { get; set; }
        public int MoveSpriteIndex { get; set; }
        public TranslateTransform MoveTransform { get; set; }
        public Image CharacterPic { get; set; }
        public Image CharacterDialogPic { get; set; }

        public CharacterBody(Character character)
        {
            this.ID = character.ID;
            this.Name = character.Name;
            this.Deck = character.Deck;
            this.SpawnDialog = character.SpawnDialog;
        }

        public bool HasCollisionWith(CharacterBody target)
        {
            bool collision = target.MoveTransform.X < MoveTransform.X + 30 && target.MoveTransform.X > MoveTransform.X - 30;
            if (collision)
            {
                CharacterDialogPic.RenderTransform = new TranslateTransform(MoveTransform.X + Sprites.SpriteWidth / 2, -(Sprites.SpriteHeight - 40));
                CharacterDialogPic.Visibility = System.Windows.Visibility.Visible;
            }
            else
                CharacterDialogPic.Visibility = System.Windows.Visibility.Hidden;
            return collision;
        }
    }
}
