using Windows.Data.Xml.Dom;

namespace DesktopToast.Helper
{
    internal class ToastDocument
    {
        public ToastDocument(XmlDocument xmlDocument)
        {
            XmlDocument = xmlDocument;
        }

        public XmlDocument XmlDocument { get; }
    }
}
