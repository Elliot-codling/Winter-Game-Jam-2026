using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Frankencard.Utils {
    [DeclareHorizontalGroup("Pair")]
    
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField, Group("Pair"), ListDrawerSettings(HideRemoveButton = true, HideAddButton = true)] private List<TKey> m_keys = new List<TKey>();
        [SerializeField, Group("Pair"), ListDrawerSettings(HideRemoveButton = true, HideAddButton = true)] private List<TValue> m_values = new List<TValue>();
        
        public void OnBeforeSerialize() {
            m_keys.Clear();
            m_values.Clear();
            
            foreach (KeyValuePair<TKey, TValue> pair in this) {
                m_keys.Add(pair.Key);
                m_values.Add(pair.Value);
            }
        }
        
        public void OnAfterDeserialize() {
            Clear();
            
            for (int i = 0; i < m_keys.Count; i++) {
                Add(m_keys[i], m_values[i]);
            }
        }
    }
}
