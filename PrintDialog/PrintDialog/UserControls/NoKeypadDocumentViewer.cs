using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;

namespace PrintDialogX.PrintControl.UserControls
{
    internal class NoKeypadDocumentViewer : DocumentViewer
    {
        public NoKeypadDocumentViewer()
        {
            TextBoxBase.SelectionBrushProperty.OverrideMetadata(typeof(DocumentViewer), new FrameworkPropertyMetadata(Brushes.Red));
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }
    }
}
