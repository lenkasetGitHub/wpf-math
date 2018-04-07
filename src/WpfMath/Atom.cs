namespace WpfMath
{
    // Atom (smallest unit) of TexFormula.
    internal abstract class Atom
    {
        protected Atom(TexAtomType type, SourceSpan source)
        {
            this.Type = type;
            this.Source = source;
        }

        protected Atom(SourceSpan source) : this(TexAtomType.Ordinary, source)
        {
        }

        public TexAtomType Type { get; }
        public SourceSpan Source { get; }

        public abstract Box CreateBox(TexEnvironment environment);

        // Gets type of leftmost child item.
        public virtual TexAtomType GetLeftType()
        {
            return this.Type;
        }

        // Gets type of leftmost child item.
        public virtual TexAtomType GetRightType()
        {
            return this.Type;
        }
    }
}
