namespace WpfMath
{
    // Dummy atom representing atom whose type can change or which can be replaced by a ligature.
    internal class DummyAtom : Atom
    {
        public DummyAtom(TexAtomType type, Atom atom, bool isTextSymbol) : base(type)
        {
            this.Atom = atom;
            this.IsTextSymbol = isTextSymbol;
        }

        public DummyAtom(Atom atom) : this(TexAtomType.None, atom, false)
        {
        }

        public Atom WithPreviousAtom(DummyAtom previousAtom)
        {
            if (this.Atom is IRow row)
            {
                return new DummyAtom(this.Type, row.WithPreviousAtom(previousAtom), this.IsTextSymbol);
            }

            return this;
        }

        public static DummyAtom CreateLigature(FixedCharAtom ligatureAtom) =>
            new DummyAtom(TexAtomType.None, ligatureAtom, false);

        public Atom Atom { get; }

        public bool IsTextSymbol { get; }

        public DummyAtom WithType(TexAtomType type) =>
            new DummyAtom(type, this.Atom, this.IsTextSymbol);

        public DummyAtom AsTextSymbol() =>
            this.IsTextSymbol ? this : new DummyAtom(this.Type, this.Atom, true);

        public bool IsCharSymbol
        {
            get { return this.Atom is CharSymbol; }
        }

        public bool IsKern
        {
            get { return this.Atom is SpaceAtom; }
        }

        public CharFont GetCharFont(ITeXFont texFont)
        {
            return ((CharSymbol)this.Atom).GetCharFont(texFont);
        }

        public override Box CreateBox(TexEnvironment environment) =>
            this.Atom.CreateBox(environment);

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
