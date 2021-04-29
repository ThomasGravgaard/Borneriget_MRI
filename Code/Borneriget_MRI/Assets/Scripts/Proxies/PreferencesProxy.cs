using PureMVC.Patterns.Proxy;

namespace Borneriget.MRI
{
    public class PreferencesProxy : Proxy
    {
        public new static string NAME = "PreferencesProxy";

        public bool IsDanish { get; set; }
        public Avatars Avatar { get; set; }
        public bool UseVr { get; set; }

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
