﻿using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Build
{
    /// <summary>
    /// Running Object Table Access
    /// </summary>
    public class ROTAccess
    {
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable rot);
        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx bindContext);

        /// <summary>
        /// Get a snapshot of the running object table (ROT).
        /// </summary>
        /// <returns>A dictionary mapping the name of the object in the ROT to the corresponding object (COM Object)</returns>
        public static Dictionary<string, List<object>> GetRunningObjectTable()
        {
            Dictionary<string, List<object>> result = new Dictionary<string, List<object>>();

            IntPtr pFetched = new IntPtr();
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, pFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectDisplayName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectDisplayName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                List<object> x = null;
                if (!result.TryGetValue(runningObjectDisplayName, out x))
                {
                    x = new List<object>();
                    result.Add(runningObjectDisplayName, x);
                }
                x.Add(runningObjectVal);
            }
            return result;
        }


        /// <summary>
        /// Get a table of the currently running instances of the Visual Studio .NET IDE.
        /// </summary>
        /// <returns>A dictionary mapping common information about the DTE object to the EnvDte.DTE object itself</returns>
        public static Dictionary<ROTDteInfo, DTE2> GetRunningDTETable()
        {
            if (!MessageFilter.IsRegistered)
                MessageFilter.Register();

            Dictionary<string, DTE2> runningIDEInstances = new Dictionary<string, DTE2>();
            Dictionary<string, List<object>> runningObjects = GetRunningObjectTable();

            IEnumerable<KeyValuePair<string, List<object>>> objTable = runningObjects.Where<KeyValuePair<string, List<object>>>((entry) => { ROTDteInfo info = null; return ROTDteInfo.TryParse(entry.Key, out info); });

            Dictionary<ROTDteInfo, DTE2> ret = new Dictionary<ROTDteInfo, DTE2>();

            foreach (KeyValuePair<string, List<object>> entry in objTable)
            {
                string displayName = entry.Key;
                Debug.Assert(entry.Value.Count == 1);

                if (entry.Value.Count > 0)
                {
                    DTE2 dte = (DTE2)entry.Value[0];


                    string solutionPath = dte.Solution.FullName;

                    if (!string.IsNullOrEmpty(solutionPath))
                    {
                        ROTDteInfo info = ROTDteInfo.Parse(entry.Key);
                        info.SolutionPath = solutionPath;
                        ret.Add(info, dte);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets the DTE Object which has opened a solution with the specified name.
        /// </summary>
        /// <param name="solutionName">Name of the solution (without path, without extension)</param>
        /// <returns>The DTE Object</returns>
        public static DTE2 GetActiveDTE(string solutionName)
        {
            KeyValuePair<ROTDteInfo, DTE2> foundEntry = GetRunningDTETable().First<KeyValuePair<ROTDteInfo, DTE2>>((entry) => { return (StringComparer.OrdinalIgnoreCase.Compare(entry.Key.SolutionName, solutionName) == 0); });
            return foundEntry.Value;
        }
    }

    /// <summary>
    /// DTE Information Object
    /// </summary>
    public class ROTDteInfo
    {
        /// <summary>
        /// The version of the DTE
        /// </summary>
        public readonly Version Version;
        /// <summary>
        /// The process identifier of the devenv process
        /// </summary>
        public readonly int ProcessId;

        /// <summary>
        /// Gets the solution path of the Info object
        /// </summary>
        /// <value>
        /// The solution path.
        /// </value>
        public string SolutionPath { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ROTDteInfo"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="solutionPath">The solution path.</param>
        /// <param name="processId">The process identifier.</param>
        public ROTDteInfo(Version version, string solutionPath, int processId)
        {
            this.Version = version;
            this.SolutionPath = solutionPath;
            this.ProcessId = processId;
        }

        /// <summary>
        /// Gets the name of the solution.
        /// </summary>
        /// <value>
        /// The name of the solution.
        /// </value>
        public string SolutionName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.SolutionPath))
                    return Path.GetFileNameWithoutExtension(this.SolutionPath);
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("!VisualStudio.DTE.{0}:{1}", this.ProcessId, this.SolutionName);
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public static ROTDteInfo Parse(string str)
        {
            ROTDteInfo info = null;
            if (!TryParse(str, out info))
            {
                throw new FormatException();
            }
            return info;
        }

        /// <summary>
        /// Tries to parse the specified string to an <see cref="ROTDteInfo"/> instance.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <returns>true if succeeded, false if not.</returns>
        public static bool TryParse(string str, out ROTDteInfo obj)
        {
            string searchStr = "!VisualStudio.DTE.";
            int index = searchStr.Length;
            int index2 = -1;

            if (str.StartsWith(searchStr))
            {
                index2 = str.LastIndexOf(':');
                int processId = -1;

                if (index2 < 0)
                    index2 = str.Length;
                else
                {
                    processId = int.Parse(str.Substring(index2 + 1));
                }

                string versionStr = str.Substring(index, index2 - index);
                Version version = Version.Parse(versionStr);
                obj = new ROTDteInfo(version, string.Empty, processId);
                return true;
            }
            else
            {
                obj = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Message Handler demo implementation to preserve the E_REJECTED_XXX Errors (See Msdn documentation for Visual Studio)
    /// </summary>
    public class MessageFilter : IOleMessageFilter
    {
        private static bool _isRegistered = false;

        /// <summary>
        /// Gets a value indicating whether the <see cref="MessageFilter"/> is registered already.
        /// </summary>
        /// <value>
        /// <c>true</c> if is registered; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRegistered
        {
            get { return _isRegistered; }
        }

        // Class containing the IOleMessageFilter
        // thread error-handling functions.
        // 

        /// <summary>
        /// Start the filter
        /// </summary>
        public static void Register()
        {
            IOleMessageFilter newFilter = new MessageFilter();
            IOleMessageFilter oldFilter = null;
            int test = CoRegisterMessageFilter(newFilter, out oldFilter);

            //if (test != 0)
            //    Debug.Fail("CoRegisterMessageFilter failed!");
            //else
                _isRegistered = true;
        }

        /// <summary>
        /// Done with the filter, close it.
        /// </summary>
        public static void Revoke()
        {
            IOleMessageFilter oldFilter = null;
            int test = CoRegisterMessageFilter(null, out oldFilter);
            _isRegistered = false;
        }

        // IOleMessageFilter functions.

        /// <summary>
        /// Handles the in coming thread requests.
        /// </summary>
        /// <param name="dwCallType">Type of the dw call.</param>
        /// <param name="hTaskCaller">The h task caller.</param>
        /// <param name="dwTickCount">The dw tick count.</param>
        /// <param name="lpInterfaceInfo">The lp interface info.</param>
        /// <returns></returns>
        int IOleMessageFilter.HandleInComingCall(int dwCallType,
          System.IntPtr hTaskCaller, int dwTickCount, System.IntPtr
          lpInterfaceInfo)
        {
            //Return the flag SERVERCALL_ISHANDLED.
            return 0;
        }

        /// <summary>
        /// Retries the rejected call.
        /// </summary>
        /// <param name="hTaskCallee">The h task callee.</param>
        /// <param name="dwTickCount">The dw tick count.</param>
        /// <param name="dwRejectType">Type of the dw reject.</param>
        /// <returns></returns>
        int IOleMessageFilter.RetryRejectedCall(System.IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        {
            // Thread call was rejected, so try again.

            if (dwRejectType == 2)
            // flag = SERVERCALL_RETRYLATER.
            {
                // Retry the thread call immediately if return >=0 & 
                // <100.
                return 99;
            }
            // Too busy; cancel call.
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hTaskCallee">The h task callee.</param>
        /// <param name="dwTickCount">The dw tick count.</param>
        /// <param name="dwPendingType">Type of the dw pending.</param>
        /// <returns></returns>
        int IOleMessageFilter.MessagePending(System.IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        {
            //Return the flag PENDINGMSG_WAITDEFPROCESS.
            return 2;
        }

        // Implement the IOleMessageFilter interface.
        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
    }

    /// <summary>
    /// Definition of the IOleMessageFilter interface
    /// </summary>
    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleMessageFilter
    {
        /// <summary>
        /// Handles the in coming call.
        /// </summary>
        /// <param name="dwCallType">Type of the dw call.</param>
        /// <param name="hTaskCaller">The h task caller.</param>
        /// <param name="dwTickCount">The dw tick count.</param>
        /// <param name="lpInterfaceInfo">The lp interface info.</param>
        /// <returns></returns>
        [PreserveSig]
        int HandleInComingCall(
            int dwCallType,
            IntPtr hTaskCaller,
            int dwTickCount,
            IntPtr lpInterfaceInfo);

        /// <summary>
        /// Retries the rejected call.
        /// </summary>
        /// <param name="hTaskCallee">The h task callee.</param>
        /// <param name="dwTickCount">The dw tick count.</param>
        /// <param name="dwRejectType">Type of the dw reject.</param>
        /// <returns></returns>
        [PreserveSig]
        int RetryRejectedCall(
            IntPtr hTaskCallee,
            int dwTickCount,
            int dwRejectType);

        /// <summary>
        /// Messages the pending.
        /// </summary>
        /// <param name="hTaskCallee">The h task callee.</param>
        /// <param name="dwTickCount">The dw tick count.</param>
        /// <param name="dwPendingType">Type of the dw pending.</param>
        /// <returns></returns>
        [PreserveSig]
        int MessagePending(
            IntPtr hTaskCallee,
            int dwTickCount,
            int dwPendingType);
    }
}
