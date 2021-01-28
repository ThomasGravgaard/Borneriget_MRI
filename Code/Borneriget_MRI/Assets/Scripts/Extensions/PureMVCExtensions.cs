using PureMVC.Interfaces;
using System;

namespace Borneriget.MRI
{
    public static class PureMVCExtensions
    {
        public static T RetrieveProxy<T>(this IFacade facade, string name) where T : IProxy => (T)facade.RetrieveProxy(name);
        public static T RetrieveProxy<T>(this IFacade facade) where T : IProxy
        {
            var name = GetNameFromType(typeof(T));
            return facade.RetrieveProxy<T>(name);
        }

        public static T RetrieveMediator<T>(this IFacade facade, string name) where T : IMediator => (T)facade.RetrieveMediator(name);
        public static T RetrieveMediator<T>(this IFacade facade) where T : IMediator
        {
            var name = GetNameFromType(typeof(T));
            return facade.RetrieveMediator<T>(name);
        }

        private static string GetNameFromType(Type type)
        {
            var field = type.GetField("NAME");
            return field.GetValue(null).ToString();
        }
    }
}
