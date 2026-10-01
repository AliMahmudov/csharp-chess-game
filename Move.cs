class Move
{
    public int FromRow { get; private set; }
    public int FromCol { get; private set; }
    public int ToRow { get; private set; }
    public int ToCol { get; private set; }

    public Move(int fromRow, int fromCol, int toRow, int toCol)
    {
        FromRow = fromRow;
        FromCol = fromCol;
        ToRow = toRow;
        ToCol = toCol;
    }
}