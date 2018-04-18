using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfMath
{
    // Atom representing horizontal row of other atoms, separated by glue.
    internal class RowAtom : Atom, IRow
    {
        // Set of atom types that make previous atom of BinaryOperator type change to Ordinary type.
        private static BitArray binaryOperatorChangeSet;

        // Set of atom types that may need kern, or together with previous atom, be replaced by ligature.
        private static BitArray ligatureKernChangeSet;

        static RowAtom()
        {
            binaryOperatorChangeSet = new BitArray(16);
            binaryOperatorChangeSet.Set((int)TexAtomType.BinaryOperator, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.BigOperator, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.Relation, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.Opening, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.Punctuation, true);

            ligatureKernChangeSet = new BitArray(16);
            ligatureKernChangeSet.Set((int)TexAtomType.Ordinary, true);
            ligatureKernChangeSet.Set((int)TexAtomType.BigOperator, true);
            ligatureKernChangeSet.Set((int)TexAtomType.BinaryOperator, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Relation, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Opening, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Closing, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Punctuation, true);
        }

        public RowAtom(IList<TexFormula> formulaList)
            : this(
                formulaList
                    .Where(formula => formula.RootAtom != null)
                    .Select(formula => formula.RootAtom))
        {
        }

        public RowAtom(Atom baseAtom)
            : this(
                baseAtom is RowAtom
                    ? (IEnumerable<Atom>) ((RowAtom) baseAtom).Elements
                    : new[] { baseAtom })
        {
        }

        public RowAtom()
        {
            this.Elements = new List<Atom>().AsReadOnly();
        }

        private RowAtom(DummyAtom previousAtom, ReadOnlyCollection<Atom> elements)
        {
            this.PreviousAtom = previousAtom;
            this.Elements = elements;
        }

        private RowAtom(IEnumerable<Atom> elements) =>
            this.Elements = elements.ToList().AsReadOnly();

        public DummyAtom PreviousAtom { get; }

        public ReadOnlyCollection<Atom> Elements { get; }

        public Atom WithPreviousAtom(DummyAtom previousAtom) =>
            new RowAtom(previousAtom, this.Elements);

        public RowAtom Add(Atom atom)
        {
            var newElements = this.Elements.ToList();
            newElements.Add(atom);
            return new RowAtom(this.PreviousAtom, newElements.AsReadOnly());
        }

        private static DummyAtom ChangeAtomToOrdinary(DummyAtom currentAtom, DummyAtom previousAtom, Atom nextAtom)
        {
            var type = currentAtom.GetLeftType();
            if (type == TexAtomType.BinaryOperator && (previousAtom == null ||
                binaryOperatorChangeSet[(int)previousAtom.GetRightType()]))
            {
                currentAtom = currentAtom.WithType(TexAtomType.Ordinary);
            }
            else if (nextAtom != null && currentAtom.GetRightType() == TexAtomType.BinaryOperator)
            {
                var nextType = nextAtom.GetLeftType();
                if (nextType == TexAtomType.Relation || nextType == TexAtomType.Closing || nextType == TexAtomType.Punctuation)
                    currentAtom = currentAtom.WithType(TexAtomType.Ordinary);
            }

            return currentAtom;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            // Create result box.
            var resultBox = new HorizontalBox(environment.Foreground, environment.Background);

            var previousAtom = this.PreviousAtom;

            // Create and add box for each atom in row.
            for (int i = 0; i < this.Elements.Count; i++)
            {
                var curAtom = new DummyAtom(this.Elements[i]);

                // Change atom type to Ordinary, if required.
                var hasNextAtom = i < this.Elements.Count - 1;
                var nextAtom = hasNextAtom ? (Atom)this.Elements[i + 1] : null;
                curAtom = ChangeAtomToOrdinary(curAtom, previousAtom, nextAtom);

                // Check if atom is part of ligature or should be kerned.
                var kern = 0d;
                if ((hasNextAtom && curAtom.GetRightType() == TexAtomType.Ordinary && curAtom.IsCharSymbol) &&
                    !(this.Elements[i].GetType() == typeof(CharAtom) && ((CharAtom)this.Elements[i]).TextStyle == "text"))
                {
                    if (nextAtom is CharSymbol cs && ligatureKernChangeSet[(int)nextAtom.GetLeftType()])
                    {
                        var font = cs.GetStyledFont(environment);
                        curAtom = curAtom.AsTextSymbol();
                        if (font.SupportsMetrics)
                        {
                            var leftAtomCharFont = curAtom.GetCharFont(font);
                            var rightAtomCharFont = cs.GetCharFont(font);
                            var ligatureCharFont = font.GetLigature(leftAtomCharFont, rightAtomCharFont);
                            if (ligatureCharFont == null)
                            {
                                // Atom should be kerned.
                                kern = font.GetKern(leftAtomCharFont, rightAtomCharFont, environment.Style);
                            }
                            else
                            {
                                // Atom is part of ligature.
                                curAtom = DummyAtom.CreateLigature(new FixedCharAtom(ligatureCharFont));
                                i++;
                            }
                        }
                    }
                }

                // Create and add glue box, unless atom is first of row or previous/current atom is kern.
                if (i != 0 && previousAtom != null && !previousAtom.IsKern && !curAtom.IsKern)
                    resultBox.Add(Glue.CreateBox(previousAtom.GetRightType(), curAtom.GetLeftType(), environment));

                // Create and add box for atom.
                var curBox = curAtom.WithPreviousAtom(previousAtom).CreateBox(environment);
                resultBox.Add(curBox);
                environment.LastFontId = curBox.GetLastFontId();

                // Insert kern, if required.
                if (kern > TexUtilities.FloatPrecision)
                    resultBox.Add(new StrutBox(0, kern, 0, 0));

                if (!curAtom.IsKern)
                    previousAtom = curAtom;
            }

            return resultBox;
        }

        public override TexAtomType GetLeftType()
        {
            if (this.Elements.Count == 0)
                return this.Type;
            return this.Elements.First().GetLeftType();
        }

        public override TexAtomType GetRightType()
        {
            if (this.Elements.Count == 0)
                return this.Type;
            return this.Elements.Last().GetRightType();
        }
    }
}
