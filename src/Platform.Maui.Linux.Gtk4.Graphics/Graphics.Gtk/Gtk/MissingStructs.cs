namespace Cairo {
    public record struct Color (double R, double G, double B, double A = 1);
}

namespace Gdk {
    public record struct Point (int X, int Y);

    public record struct Size (int Width, int Height);
}