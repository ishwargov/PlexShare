using System.Windows.Media.Imaging;

namespace PlexShareScreenshare.Client
{
    public class SharedClient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BitmapImage Image { get; set; }
        public SharedClient(int id, string name, BitmapImage image)
        {
            Id = id;
            Name = name;
            Image = image;
        }
    }
}
