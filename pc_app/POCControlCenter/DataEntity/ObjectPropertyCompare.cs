using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace POCControlCenter.DataEntity
{
    class ObjectPropertyCompare<T> : IComparer<T>
    {
        private PropertyDescriptor property;
        private ListSortDirection direction;

        // 构造函数
        public ObjectPropertyCompare(PropertyDescriptor property, ListSortDirection direction)
        {
            this.property = property;
            this.direction = direction;
        }

        // 实现IComparer中方法
        public int Compare(T x, T y)
        {
            object xValue = x.GetType().GetProperty(property.Name).GetValue(x, null);
            object yValue = y.GetType().GetProperty(property.Name).GetValue(y, null);

            int returnValue;

            if (xValue is IComparable)
            {
                returnValue = ((IComparable)xValue).CompareTo(yValue);
            }
            else if (xValue.Equals(yValue))
            {
                returnValue = 0;
            }
            else
            {
                returnValue = xValue.ToString().CompareTo(yValue.ToString());
            }

            if (direction == ListSortDirection.Ascending)
            {
                return returnValue;
            }
            else
            {
                return returnValue * -1;
            }
        }
    }
}
