namespace SeaWolfAggr
{
    public class Pos
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        public Pos(int col, int row)
        {
            Col = col;
            Row = row;
        }

        public static bool operator ==(Pos pos1, Pos pos2)
        {
            if (pos1 is null || pos2 is null) return false;
            return pos1.Col == pos2.Col && pos1.Row == pos2.Row;
        }

        public static bool operator !=(Pos pos1, Pos pos2)
        {
            return !(pos1 == pos2);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Pos;
        }

        public override int GetHashCode()
        {
            return Col * 100 + Row;
        }
    }
}