using System.Text;

namespace LatinAutoDecline.Nouns
{
    public enum DeclensionEnum
    {
        One = 1,
        Two = 2,
        TwoREnd = -2,
        Three = 3,
        ThreeIStem = -3,
        Four =4,
        Five =5,
        Irregular = -1
    }

    public enum Gender
    {
        Masculine,
        Feminine,
        Neuter
    }

    public enum Number
    {
        SingularAndPlural = 1,
        PluralOnly = 2
    }
}
