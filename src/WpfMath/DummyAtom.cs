namespace WpfMath
{
    // Dummy atom representing atom whose type can change or which can be replaced by a ligature.
    // TODO[F]: It looks like this atom is always temporary so the constructors shouldn't accept SourceSpan
    internal class DummyAtom : Atom
    {
        public DummyAtom(TexAtomType type, SourceSpan source, Atom atom, bool isTextSymbol) : base(type, source)
        {
            this.Atom = atom;
            this.IsTextSymbol = isTextSymbol;
        }

        public DummyAtom(SourceSpan source, Atom atom) : this(TexAtomType.None, source, atom, false)
        {
        }

        public Atom Atom { get; }

        public bool IsTextSymbol { get; }

        public bool IsCharSymbol
        {
            get { return this.Atom is CharSymbol; }
        }

        public bool IsKern
        {
            get { return this.Atom is SpaceAtom; }
        }

        public DummyAtom WithLigature(FixedCharAtom ligatureAtom) =>
            new DummyAtom(TexAtomType.None, this.Source, ligatureAtom, false);

        public CharFont GetCharFont(ITeXFont texFont)
        {
            return ((CharSymbol)this.Atom).GetCharFont(texFont);
        }

        public override Box CreateBox(TexEnvironment environment) =>
            ((CharSymbol) this.Atom).CreateBox(environment, true);

        public override TexAtomType GetLeftType()
        {
            return this.Type == TexAtomType.None ? this.Atom.GetLeftType() : this.Type;
        }

        public override TexAtomType GetRightType()
        {
            return this.Type == TexAtomType.None ? this.Atom.GetRightType() : this.Type;
        }
    }
}
