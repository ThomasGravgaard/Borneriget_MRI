using PureMVC.Patterns.Proxy;

namespace Borneriget.MRI
{
    public class PreferencesProxy : Proxy
    {
        public new static string NAME = "PreferencesProxy";

        public string Language { get; set; }
        public string Avatar { get; set; }

        public enum Avatars
        {
            Theo,
            Thea
        }

        public PreferencesProxy() : base(NAME)
        {
        }
    }
}
