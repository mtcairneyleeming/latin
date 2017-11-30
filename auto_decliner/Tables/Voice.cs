namespace LatinAutoDecline.Tables
{
    public class Voice<T>
    {
        public readonly T Present;
        public readonly T Imperfect;
        public readonly T Future;
        public readonly T Perfect;
        public readonly T PluPerfect;
        public readonly T FuturePerfect;

        public Voice(T present, T imperfect, T future, T perfect, T pluPerfect, T futurePerfect)
        {
            Present = present;
            Imperfect = imperfect;
            Future = future;
            Perfect = perfect;
            PluPerfect = pluPerfect;
            FuturePerfect = futurePerfect;
        }

        public override string ToString()
        {
            return
                $"{nameof(Present)}: {Present}, {nameof(Imperfect)}: {Imperfect}, {nameof(Future)}: {Future}, {nameof(Perfect)}: {Perfect}, {nameof(PluPerfect)}: {PluPerfect}, {nameof(FuturePerfect)}: {FuturePerfect}";
        }
    }
}