using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Reflection;

namespace PrintDialogX.Internal.UserControls
{
    internal class NoKeypadDocumentViewer : DocumentViewer
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }

        protected override void OnDocumentChanged()
        {
            base.OnDocumentChanged();
            SetIsSelectionEnabled();
        }

        bool _isSelectionEnabled;

        public bool IsSelectionEnabled
        {
            get => _isSelectionEnabled;
            set
            {
                _isSelectionEnabled = value;
                SetIsSelectionEnabled();
            }
        }

        static PropertyInfo s_IsSelectionEnabled_property = typeof(DocumentViewer).GetProperty("IsSelectionEnabled", BindingFlags.Instance | BindingFlags.NonPublic);

        void SetIsSelectionEnabled()
        {
            s_IsSelectionEnabled_property?.SetValue(this, _isSelectionEnabled);
        }
    }
}
