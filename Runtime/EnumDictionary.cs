using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xoletis.EditorTools
{
    [Serializable]
    public class EnumDictionary<TEnum, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TEnum, TValue>>
        where TEnum : Enum
    {
        private static readonly TEnum[] Keys = (TEnum[])Enum.GetValues(typeof(TEnum));

        [SerializeField] private TValue[] values = Array.Empty<TValue>();

        public int Count => Keys.Length;

        public TValue this[TEnum key]
        {
            get => values[IndexOf(key)];
            set => values[IndexOf(key)] = value;
        }

        public IEnumerator<KeyValuePair<TEnum, TValue>> GetEnumerator()
        {
            EnsureSize();
            for (int i = 0; i < Keys.Length; i++)
            {
                yield return new KeyValuePair<TEnum, TValue>(Keys[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static int IndexOf(TEnum key)
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                if (Keys[i].Equals(key))
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(key), key, "Unknown enum value.");
        }

        private void EnsureSize()
        {
            if (values == null || values.Length != Keys.Length)
            {
                Array.Resize(ref values, Keys.Length);
            }
        }

        public void OnBeforeSerialize() => EnsureSize();

        public void OnAfterDeserialize() => EnsureSize();
    }
}
