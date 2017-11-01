namespace LatinAutoDecline
{
    /// <summary>
    /// A class that holds a table of endings: e.g. for the first declension singular feminine.
    /// </summary>
    class EndingsTable
    {

        public CaseTable SingularCaseTable { get; set; }
        public CaseTable PluralCaseTable { get; set; }


        public override string ToString()
        {
            return $"Sing: {SingularCaseTable}, Pl: {PluralCaseTable}";
        }

        public string GetForm(Number num, Case cas)
        {
            if (num == Number.Singular)
            {
                return SingularCaseTable.GetForm(cas);
            }
            return PluralCaseTable.GetForm(cas);
        }
    }
}