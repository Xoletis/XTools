using UnityEngine;

namespace Xoletis.EditorTools
{
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; }
        public bool Inverse { get; }
        public object CompareValue { get; }
        public bool HasCompareValue { get; }

        /// <summary>
        /// Shows the field only while <paramref name="conditionFieldName"/> (a sibling
        /// bool/enum/int/float/string/object field) is "truthy". Set <paramref name="inverse"/>
        /// to true to hide instead when it's truthy.
        /// </summary>
        public ConditionalFieldAttribute(string conditionFieldName, bool inverse = false)
        {
            ConditionFieldName = conditionFieldName;
            Inverse = inverse;
        }

        /// <summary>
        /// Shows the field only while the sibling field named <paramref name="conditionFieldName"/>
        /// equals <paramref name="compareValue"/> (e.g. a specific enum member or int).
        /// </summary>
        public ConditionalFieldAttribute(string conditionFieldName, object compareValue)
        {
            ConditionFieldName = conditionFieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }
    }
}
