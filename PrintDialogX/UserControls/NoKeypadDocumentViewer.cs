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

            // DocumentViewer has as internal property IsSelectionEnabled. It aggressively sets
            // it to true every time the document changes. But, when selection is enabled,
            // Ctrl+MouseWheel zooming has a highly-undesirable side-effect of resetting the
            // viewport to the top/left of the page every time the zoom changes. This behaviour
            // only happens when selection is enabled (and thus it has a visible insertion point).
            // So, we always set it to false in order to keep the viewport offset when zooming.
            DisableSelection();
        }

        static PropertyInfo s_IsSelectionEnabled_property = typeof(DocumentViewer).GetProperty("IsSelectionEnabled", BindingFlags.Instance | BindingFlags.NonPublic);

        void DisableSelection()
        {
            s_IsSelectionEnabled_property?.SetValue(this, false);
        }
    }
}
