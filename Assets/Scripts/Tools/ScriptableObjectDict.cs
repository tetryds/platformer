using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer.Tools
{
    [Serializable]
    public abstract class ScriptableObjectDict<Tkey, Tvalue> : ScriptableObject
    {
        [field: SerializeField]
        public SerializableDictItem[] Items { get; set; }

        readonly Lazy<Dictionary<Tkey, Tvalue>> lazyMap;

        public Dictionary<Tkey, Tvalue>.KeyCollection Keys => lazyMap.Value.Keys;
        public Dictionary<Tkey, Tvalue>.ValueCollection Values => lazyMap.Value.Values;

        public ScriptableObjectDict()
        {
            lazyMap = new Lazy<Dictionary<Tkey, Tvalue>>(() => Items.ToDictionary(i => i.Key, i => i.Value));
        }

        public bool TryGet(Tkey key, out Tvalue value) => lazyMap.Value.TryGetValue(key, out value);

        public Tvalue GetOrDefault(Tkey key, Tvalue defaultValue) => lazyMap.Value.GetValueOrDefault(key, defaultValue);

        public Tvalue this[Tkey tkey] => lazyMap.Value[tkey];

        public Dictionary<Tkey, Tvalue> GetDict() => lazyMap.Value;

        [Serializable]
        public class SerializableDictItem
        {
            public Tkey Key;
            public Tvalue Value;
        }
    }
}

