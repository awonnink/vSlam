namespace SlmControls
{
    public interface IWebSceneProps
    {
        string ImageUrl { get; set; }
        string Name { get; set; }
        Target Target { get; set; }
        string Url { get; set; }
        int Visitors { get; set; }
        int Favorite { get; set; }
        int History { get; set; }
    }
}