using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

var options = WindowOptions.Default with
{
    Size = new Vector2D<int>(800, 600),
    Title = "My first Silk.NET application!"
};
var window = Window.Create(options);
window.Run();
