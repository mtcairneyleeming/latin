using System;
using System.Collections.Generic;
using System.Reflection;
using decliner.Database;
using decliner.Tables;

namespace decliner.Helpers
{
    public static class FormsParser
    {
        public static Noun ProcessNoun(List<Form> forms)
        {
            var nounTable = new Noun();
            foreach (var form in forms)
            {
                Type type;
                PropertyInfo prop;
                if (MorphCodeParser.ParsePartOfSpeech(form.MorphCode) != Part.Noun) continue;
                var formCase = MorphCodeParser.ParseCase(form.MorphCode);
                if (formCase == Case.Locative || formCase == Case.Instrumental)
                    continue;

                var num = MorphCodeParser.ParseNumber(form.MorphCode);
                switch (num)
                {
                    case Number.Singular:
                        if (nounTable.SingularCases == null)
                            nounTable.SingularCases = new Cases();
                        type = nounTable.SingularCases.GetType();

                        prop = type.GetProperty(MorphCodeParser.ParseCase(form.MorphCode).ToString());

                        prop.SetValue(nounTable.SingularCases, form.Text, null);


                        nounTable.UseSingular = true;
                        break;
                    case Number.Plural:
                        if (nounTable.PluralCases != null)
                        {
                            type = nounTable.PluralCases.GetType();

                            prop = type.GetProperty(MorphCodeParser.ParseCase(form.MorphCode).ToString());

                            prop.SetValue(nounTable.PluralCases, form.Text, null);
                        }

                        break;
                }
            }

            return nounTable;
        }
    }
}