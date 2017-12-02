using System.Collections.Generic;
using System.Reflection;

namespace decliner.Helpers
{
    public struct Variance
    {
        public readonly string Property;
        public readonly object FirstVal;
        public readonly object SecondVal;

        public Variance(string property, object firstVal, object secondVal)
        {
            Property = property;
            FirstVal = firstVal;
            SecondVal = secondVal;
        }
    }

    public class Comparators
    {
        public static List<Variance> Compare<T>(T one, T two)
        {
            List<Variance> variances = new List<Variance>();
            FieldInfo[] fi = one.GetType().GetFields();
            foreach (FieldInfo f in fi)
            {
                if (f.FieldType.IsValueType)
                {
                    Variance v = new Variance(
                        f.Name,
                        f.GetValue(one),
                        f.GetValue(two)
                    );
                    if (!v.FirstVal.Equals(v.SecondVal))
                        variances.Add(v);
                }
                else
                {
                    variances.AddRange(Compare(f.GetValue(one), f.GetValue(two)));
                }
                
            }
            return variances;
        }
    }
}