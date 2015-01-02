using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DesktopToast
{
    /// <summary>
    /// Toast request container
    /// </summary>
    [DataContract]
    public class ToastRequest
    {
        #region Property (Public)

        /// <summary>
        /// Toast headline (optional)
        /// </summary>
        [DataMember]
        public string ToastHeadline { get; set; }

        /// <summary>
        /// Whether toast headline wraps across two lines (optional) 
        /// </summary>
        [DataMember]
        public bool ToastHeadlineWrapsTwoLines { get; set; }

        /// <summary>
        /// Toast body (required for toast)
        /// </summary>
        [DataMember]
        public string ToastBody { get; set; }

        /// <summary>
        /// Toast body extra section (optional)
        /// </summary>
        /// <remarks>This section will be reflected only when toast headline is specified and it does not
        /// wraps across two lines.</remarks>
        [DataMember]
        public string ToastBodyExtra { get; set; }

        /// <summary>
        /// Toast image file path (optional)
        /// </summary>
        /// <remarks>
        /// The image file path must be in the following form.
        /// "file:///" + full file path
        /// </remarks>
        [DataMember]
        public string ToastImageFilePath { get; set; }

        /// <summary>
        /// Toast audio (optional)
        /// </summary>
        [DataMember]
        public ToastAudio ToastAudio { get; set; }

        /// <summary>
        /// Shortcut file name to be installed in Windows startup (required for shortcut)
        /// </summary>
        [DataMember]
        public string ShortcutFileName { get; set; }

        /// <summary>
        /// Target file path of shortcut (required for shortcut)
        /// </summary>
        [DataMember]
        public string ShortcutTargetFilePath { get; set; }

        /// <summary>
        /// Icon file path of shortcut (optional) 
        /// </summary>
        /// <remarks>If not specified, target file path will be used.</remarks>
        [DataMember]
        public string ShortcutIconFilePath { get; set; }

        /// <summary>
        /// Arguments of shortcut (optional)
        /// </summary>
        [DataMember]
        public string ShortcutArguments { get; set; }

        /// <summary>
        /// AppUserModelID of application (required)
        /// </summary>
        /// <renarks>
        /// The AppUserModelID must be in the following form. 
        /// CompanyName.ProductName.SubProduct.VersionInformation 
        /// It can have no more than 128 characters and cannot contain spaces.
        /// </renarks>
        [DataMember]
        public string AppId { get; set; }

        /// <summary>
        /// Waiting time length before showing a toast after the shortcut file is installed (optional)
        /// </summary>
        [DataMember]
        public TimeSpan WaitingTime { get; set; }

        #endregion


        #region Property (Internal)

        internal bool IsShortcutValid
        {
            get
            {
                return !String.IsNullOrWhiteSpace(ShortcutFileName) &&
                    !String.IsNullOrWhiteSpace(ShortcutTargetFilePath) &&
                    !String.IsNullOrWhiteSpace(AppId);
            }
        }

        internal bool IsToastValid
        {
            get
            {
                return !String.IsNullOrWhiteSpace(ToastBody) &&
                    !String.IsNullOrWhiteSpace(AppId);
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ToastRequest()
        { }

        internal ToastRequest(string requestString)
            : this()
        {
            Import(requestString);
        }

        #endregion


        #region Import/Export

        /// <summary>
        /// Import from a request in JSON format.
        /// </summary>
        /// <param name="requestJson">Request in JSON format</param>
        internal void Import(string requestJson)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(requestJson)))
            {
                var serializer = new DataContractJsonSerializer(typeof(ToastRequest));
                var buff = (ToastRequest)serializer.ReadObject(stream);

                typeof(ToastRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(x => x.CanWrite)
                    .ToList()
                    .ForEach(x => x.SetValue(this, x.GetValue(buff)));
            }
        }

        /// <summary>
        /// Export a request in JSON format.
        /// </summary>
        /// <returns>Request in JSON format</returns>
        internal string Export()
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(ToastRequest));
                serializer.WriteObject(stream, this);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        #endregion
    }
}