namespace WpfMath
{
    // Atom representing big delimeter (e.g. brackets).
    internal readonly struct BigDelimeterAtom : IAtom
    {
        public TexAtomType Type => TexAtomType.Ordinary;
        public TexAtomType GetLeftType() => Type;
        public TexAtomType GetRightType() => Type;

        public SourceSpan Source { get; }

        public BigDelimeterAtom(IAtom delimeterAtom, int size)
        {
            this.DelimeterAtom = delimeterAtom;
            this.Size = size;
            this.Source = null;
        }

        public IAtom DelimeterAtom { get; }

        public int Size { get; }

        public Box CreateBox(TexEnvironment environment)
        {
            // TODO
            var resultBox = (Box)null; // DelimiterFactory.CreateBox(this.DelimeterAtom, this.Size, environment);
            resultBox.Shift = -(resultBox.Height + resultBox.Depth) / 2 -
                environment.MathFont.GetAxisHeight(environment.Style);
            return resultBox;
        }
    }
}
