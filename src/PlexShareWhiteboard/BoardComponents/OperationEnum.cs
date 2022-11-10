namespace PlexShareWhiteboard.BoardComponents
{
    /// <summary>
    ///         Various operations that can be performed on the ShapeItems.
    /// </summary>
    public enum Operation
    {
        Creation,
        Deletion,
        ModifyShape,
        Clear,
        UndoClear,
        RestoreSnapshot,
        CreateSnapshot
    }
}