using System.Text;

namespace com.game.utilities
{
    public static class Helpers
    {
        public static class Text
        {
            public static string Bold(string input)
            {
                return CoverXML(input, "b", "/b");
            }

            public static string Italic(string input)
            {
                return CoverXML(input, "i", "/i");
            }

            public static string Colorize(string input, string colorHexcode)
            {
                return CoverXML(input, $"color:{colorHexcode}", "/color");
            }

            public static string CoverXML(string input, string xmlStart, string xmlEnd)
            {
                StringBuilder sb = new();

                sb.Append($"<{xmlStart}>");
                sb.Append(input);
                sb.Append($"<{xmlEnd}>");

                return sb.ToString();
            }
        }
    }
}
