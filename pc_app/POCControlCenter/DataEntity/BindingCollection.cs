using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace POCControlCenter.DataEntity
{
    class BindingCollection<T> : BindingList<T>
    {
        private bool isSorted;
        private PropertyDescriptor sortProperty;
        private ListSortDirection sortDirection;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BindingCollection()
            : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">IList类型的列表对象</param>
        public BindingCollection(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// 自定义排序操作
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> items = this.Items as List<T>;

            if (items != null)
            {
                ObjectPropertyCompare<T> pc = new ObjectPropertyCompare<T>(property, direction);
                items.Sort(pc);
                isSorted = true;
            }
            else
            {
                isSorted = false;
            }

            sortProperty = property;
            sortDirection = direction;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// 获取一个值，指示列表是否已排序
        /// </summary>
        protected override bool IsSortedCore
        {
            get
            {
                return isSorted;
            }
        }

        /// <summary>
        /// 获取一个值，指示列表是否支持排序
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取一个只，指定类别排序方向
        /// </summary>
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return sortDirection;
            }
        }

        /// <summary>
        /// 获取排序属性说明符
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return sortProperty;
            }
        }

        /// <summary>
        /// 移除默认实现的排序
        /// </summary>
        protected override void RemoveSortCore()
        {
            isSorted = false;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

    }
}
