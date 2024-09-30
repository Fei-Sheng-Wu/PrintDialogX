using System.Windows.Controls;

namespace PrintDialogX.Internal.UserControls
{
    internal class NoKeypadDocumentViewer : DocumentViewer
    {
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new System.Windows.Automation.Peers.FrameworkElementAutomationPeer(this);
        }
    }
}
