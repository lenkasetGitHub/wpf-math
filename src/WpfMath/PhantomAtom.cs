namespace WpfMath
{
    // Atom representing other atom that is not rendered.
    internal class PhantomAtom : Atom
    {
        private readonly bool useWidth;
        private readonly bool useHeight;
        private readonly bool useDepth;

        public PhantomAtom(SourceSpan source, Atom baseAtom)
            : this(source, baseAtom, true, true, true)
        {
        }

        public PhantomAtom(SourceSpan source, Atom baseAtom, bool useWidth, bool useHeight, bool useDepth)
            : base(source)
        {
            this.RowAtom = baseAtom == null ? new RowAtom() : new RowAtom(baseAtom);
            this.useWidth = useWidth;
            this.useHeight = useHeight;
            this.useDepth = useDepth;
        }

        public DummyAtom PreviousAtom => this.RowAtom.PreviousAtom;

        public RowAtom RowAtom { get; }

        public override Box CreateBox(TexEnvironment environment)
        {
            var resultBox = this.RowAtom.CreateBox(environment);
            return new StrutBox((this.useWidth ? resultBox.Width : 0), (this.useHeight ? resultBox.Height : 0),
                (this.useDepth ? resultBox.Depth : 0), resultBox.Shift);
        }

        public override TexAtomType GetLeftType()
        {
            return this.RowAtom.GetLeftType();
        }

        public override TexAtomType GetRightType()
        {
            return this.RowAtom.GetRightType();
        }
    }
}
