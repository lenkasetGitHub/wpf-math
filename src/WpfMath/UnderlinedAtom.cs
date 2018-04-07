namespace WpfMath
{
    // Atom representing other atom that is underlined.
    internal class UnderlinedAtom : Atom
    {
        public UnderlinedAtom(Atom baseAtom) : base(baseAtom.Source)
        {
            this.BaseAtom = baseAtom;
        }

        public Atom BaseAtom { get; }

        public override Atom Copy()
        {
            return CopyTo(new UnderlinedAtom(BaseAtom?.Copy()));
        }

        protected override Box CreateBox(TexEnvironment environment)
        {
            var defaultLineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);

            // Create box for base atom.
            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);

            // Create result box.
            var resultBox = new VerticalBox();
            resultBox.Add(baseBox);
            resultBox.Add(new StrutBox(0, 3 * defaultLineThickness, 0, 0));
            resultBox.Add(new HorizontalRule(environment, defaultLineThickness, baseBox.Width, 0));

            resultBox.Depth = baseBox.Depth + 5 * defaultLineThickness;
            resultBox.Height = baseBox.Height;

            return resultBox;
        }
    }
}
