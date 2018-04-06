using System;

namespace WpfMath
{
    /// <summary>Atom representing base atom with accent above it.</summary>
    internal readonly struct AccentedAtom : IAtom
    {
        public TexAtomType Type => TexAtomType.Ordinary;
        public TexAtomType GetLeftType() => Type;
        public TexAtomType GetRightType() => Type;

        public SourceSpan Source { get; }

        public AccentedAtom(IAtom baseAtom, string accentName, SourceSpan source)
        {
            this.BaseAtom = baseAtom;
            this.AccentAtom = SymbolAtom.GetAtom(accentName, source);
            this.Source = source;

            if (this.AccentAtom.Type != TexAtomType.Accent)
                throw new ArgumentException("The specified symbol name is not an accent.", "accent");
        }

        public AccentedAtom(IAtom baseAtom, TexFormula accent)
        {
            var rootSymbol = accent.RootAtom as SymbolAtom?;
            if (rootSymbol == null)
                throw new ArgumentException("The formula for the accent is not a single symbol.", "accent");
            this.AccentAtom = rootSymbol.Value;

            if (this.AccentAtom.Type != TexAtomType.Accent)
                throw new ArgumentException("The specified symbol name is not an accent.", "accent");

            this.BaseAtom = null;
            this.Source = null;
        }

        /// <summary>Atom over which accent symbol is placed.</summary>
        public IAtom BaseAtom { get; }

        // Atom representing accent symbol to place over base atom.
        public SymbolAtom AccentAtom { get; }

        public Box CreateBox(TexEnvironment environment)
        {
            ICharSymbol GetBaseChar(IAtom baseAtom)
            {
                while (baseAtom is AccentedAtom a)
                {
                    baseAtom = a.BaseAtom;
                }

                return baseAtom as ICharSymbol;
            }

            var texFont = environment.MathFont;
            var style = environment.Style;

            // Create box for base atom.
            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment.GetCrampedStyle());
            var baseCharFont = GetBaseChar(BaseAtom)?.GetCharFont(texFont);
            var skew = baseCharFont == null ? 0.0 : texFont.GetSkew(baseCharFont, style);

            // Find character of best scale for accent symbol.
            var accentChar = texFont.GetCharInfo(AccentAtom.Name, style);
            while (texFont.HasNextLarger(accentChar))
            {
                var nextLargerChar = texFont.GetNextLargerCharInfo(accentChar, style);
                if (nextLargerChar.Metrics.Width > baseBox.Width)
                    break;
                accentChar = nextLargerChar;
            }

            var resultBox = new VerticalBox();

            // Create and add box for accent symbol.
            Box accentBox;
            var accentItalicWidth = accentChar.Metrics.Italic;
            if (accentItalicWidth > TexUtilities.FloatPrecision)
            {
                accentBox = new HorizontalBox(new CharBox(environment, accentChar));
                accentBox.Add(new StrutBox(accentItalicWidth, 0, 0, 0));
            }
            else
            {
                accentBox = new CharBox(environment, accentChar);
            }
            resultBox.Add(accentBox);

            var delta = Math.Min(baseBox.Height, texFont.GetXHeight(style, accentChar.FontId));
            resultBox.Add(new StrutBox(0, -delta, 0, 0));

            // Centre and add box for base atom. Centre base box and accent box with respect to each other.
            var boxWidthsDiff = (baseBox.Width - accentBox.Width) / 2;
            accentBox.Shift = skew + Math.Max(boxWidthsDiff, 0);
            if (boxWidthsDiff < 0)
                baseBox = new HorizontalBox(baseBox, accentBox.Width, TexAlignment.Center);
            resultBox.Add(baseBox);

            // Adjust height and depth of result box.
            var depth = baseBox.Depth;
            var totalHeight = resultBox.Height + resultBox.Depth;
            resultBox.Depth = depth;
            resultBox.Height = totalHeight - depth;

            return resultBox;
        }
    }
}
