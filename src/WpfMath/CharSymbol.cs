namespace WpfMath
{
    /// <summary>Atom representing single character that can be marked as text symbol.</summary>
    internal interface ICharSymbol : IAtom
    {
        bool IsTextSymbol { get; }

        /// <summary>Returns the preferred font to render this character.</summary>
        ITeXFont GetStyledFont(TexEnvironment environment) => environment.MathFont;

        CharFont GetCharFont(ITeXFont texFont);
    }
}
