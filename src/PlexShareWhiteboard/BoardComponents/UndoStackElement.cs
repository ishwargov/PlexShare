namespace PlexShareWhiteboard.BoardComponents
{

    public class UndoStackElement
    {
        public ShapeItem PrvShape { get; set; }
        public ShapeItem NewShape { get; set; }
        public Operation Op { get; set; }
    }
}
