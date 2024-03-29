﻿using System.Windows.Threading;

namespace PrintDialogX
{
    internal class Common
    {
        private static readonly DispatcherOperationCallback _exitFrameCallback = new DispatcherOperationCallback(ExitFrame);

        internal static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }
    }
}
