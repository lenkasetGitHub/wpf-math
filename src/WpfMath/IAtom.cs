namespace WpfMath
{
    /// <summary>Atom (smallest unit) of TexFormula.</summary>
    internal interface IAtom
    {
        TexAtomType Type { get; }

        /// <summary>Gets type of leftmost child item.</summary>
        TexAtomType GetLeftType();

        /// <summary>Gets type of leftmost child item.</summary>
        TexAtomType GetRightType();

        /// <summary>Source of this atom. May be <c>null</c>.</summary>
        SourceSpan Source { get; }

        Box CreateBox(TexEnvironment environment);
    }
}
