using Main.Models;

namespace Main
{
    class Core
    {
        public static JesterAndKingContext Context = new JesterAndKingContext();
        public static User CurrentUser { get; set; } = null;
    }
}
