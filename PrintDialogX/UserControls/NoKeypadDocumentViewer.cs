using System.Windows.Controls;
using System.Windows.Automation.Peers;

namespace PrintDialogX.Internal.UserControls
{
    internal class NoKeypadDocumentViewer : DocumentViewer
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }
    }
}
