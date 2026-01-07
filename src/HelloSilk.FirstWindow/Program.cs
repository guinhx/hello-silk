using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

var options = WindowOptions.Default with
{
    Size = new Vector2D<int>(800, 600),
    Title = "My first Silk.NET application!"
};
var window = Window.Create(options);

window.Load += () =>
{
    Console.WriteLine("Window loaded!");

    var input = window.CreateInput();
    for (var i = 0; i < input.Keyboards.Count; i++) {
        var keyboard = input.Keyboards[i];
        keyboard.KeyDown += (IKeyboard keyboard, Key key, int keyCode) =>
        {
            Console.WriteLine($"Key {key} pressed!");
        };
    }
};

window.Update += (double deltaTime) =>
{
    // change window title
    window.Title = $"My first Silk.NET application! Dt. {deltaTime:F2}";
};

window.Render += (double deltaTime) =>
{
    // Rendering goes here
};

window.Run();
