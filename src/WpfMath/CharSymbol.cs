namespace WpfMath
{
    // Atom representing single character that can be marked as text symbol.
    internal abstract class CharSymbol : Atom
    {
        protected CharSymbol(TexAtomType type, SourceSpan source) : base(type, source)
        {
            this.IsTextSymbol = false;
        }

        protected CharSymbol(SourceSpan source) : this(TexAtomType.Ordinary, source)
        {
        }

        public bool IsTextSymbol { get; }

        /// <summary>Returns the preferred font to render this character.</summary>
        public virtual ITeXFont GetStyledFont(TexEnvironment environment) => environment.MathFont;

        public abstract CharFont GetCharFont(ITeXFont texFont);

        public abstract Box CreateBox(TexEnvironment environment, bool isTextSymbol);

        public sealed override Box CreateBox(TexEnvironment environment) =>
            CreateBox(environment, this.IsTextSymbol);
    }
}
