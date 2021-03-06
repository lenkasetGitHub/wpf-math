namespace WpfMath
{
    // Atom representing big delimeter (e.g. brackets).
    internal class BigDelimeterAtom : Atom
    {
        public BigDelimeterAtom(Atom delimeterAtom, int size)
        {
            this.DelimeterAtom = delimeterAtom;
            this.Size = size;
        }

        public Atom DelimeterAtom { get; }

        public int Size { get; }

        public override Box CreateBox(TexEnvironment environment)
        {
            // TODO
            var resultBox = (Box)null; // DelimiterFactory.CreateBox(this.DelimeterAtom, this.Size, environment);
            resultBox.Shift = -(resultBox.Height + resultBox.Depth) / 2 -
                environment.MathFont.GetAxisHeight(environment.Style);
            return resultBox;
        }
    }
}
