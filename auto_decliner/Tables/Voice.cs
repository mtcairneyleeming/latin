namespace decliner.Tables
{
    public class Voice<T> where T : new()
    {
        public T Future{ get; set; }
        public T FuturePerfect{ get; set; }
        public T Imperfect{ get; set; }
        public T Perfect{ get; set; }
        public T PluPerfect{ get; set; }
        public T Present{ get; set; }

        public Voice()
        {
            Future = new T();
            FuturePerfect = new T();
            Imperfect = new T();
            Imperfect = new T();
            Perfect = new T();
            PluPerfect = new T();
            Present = new T();
        }

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