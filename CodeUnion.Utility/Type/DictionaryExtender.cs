using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeUnion.Utility.Type
{
    /// <summary>
    /// 以整数为主键的字典类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumberDcitionary<T> : Dictionary<int, T>
    {
        #region field
        T _emptyValue = default(T);
        #endregion
        #region ctor
        public NumberDcitionary() { }
        public NumberDcitionary(int capacity)
            : base(capacity)
        {

        }
        #endregion

        public new T this[int key]
        {
            get
            {
                return base.Keys.Contains(key) ? base[key] : _emptyValue;
            }
            set
            {
                base[key] = value;
            }
        }

        public void SetDefaultEmptyValue(T emptyValue)
        {
            _emptyValue = emptyValue;
        }

    }
}
