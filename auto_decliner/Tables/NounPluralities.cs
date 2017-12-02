namespace decliner.Tables
{
    /// <summary>
    ///     A class that holds a table of endings: e.g. for the first declension singular feminine.
    /// </summary>
    public class NounPluralities
    {
        public NounPluralities()
        {
            Singular = new Cases();
            Plural = new Cases();
        }

        public NounPluralities(Cases singular, Cases plural)
        {
            Singular = singular;
            Plural = plural;
        }

        public Cases Singular { get; set; }
        public Cases Plural { get; set; }


        public override string ToString()
        {
            return $"Sing: {Singular}, Pl: {Plural}";
        }

        public string GetForm(Number num, Case cas)
        {
            if (num == Number.Singular)
                return Singular.GetForm(cas);
            return Plural.GetForm(cas);
        }
    }
}