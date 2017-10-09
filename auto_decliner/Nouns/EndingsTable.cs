namespace LatinAutoDecline.Nouns
{
    /// <summary>
    /// A class that holds a table of endings: e.g. for the first declension singular feminine.
    /// </summary>
    class EndingsTable
    {

        public Cases SingularCases { get; set; }
        public Cases PluralCases { get; set; }


        public EndingsTable()
        {
            
        }

        public override string ToString()
        {
            return $"Sing: {SingularCases}, Pl: {PluralCases}";
        }
    }
}