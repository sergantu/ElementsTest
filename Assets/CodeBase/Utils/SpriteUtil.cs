using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace CodeBase.Utils
{
    public static class SpriteUtil
    {
        private static readonly Dictionary<string, Sprite[]> _atlases = new();
        private static readonly Dictionary<string, Sprite> _sprites = new();

        public static Sprite GetSprite(string name)
        {
            Sprite item;

            if (_sprites.ContainsKey(name)) {
                item = _sprites[name];
            } else {
                item = Resources.Load<Sprite>("Sprites/" + name);
                _sprites.Add(name, item);
            }

            if (item == null) {
                return null;
            }

            return item;
        }

        [CanBeNull]
        public static Sprite GetSpriteFromAtlas(string atlas, string spriteName)
        {
            Sprite[] items;

            if (_atlases.ContainsKey(atlas)) {
                items = _atlases[atlas];
            } else {
                items = Resources.LoadAll<Sprite>("Sprites/" + atlas);
                _atlases.Add(atlas, items);
            }

            return items?.FirstOrDefault(item => item.name.Equals(spriteName));
        }
    }
}