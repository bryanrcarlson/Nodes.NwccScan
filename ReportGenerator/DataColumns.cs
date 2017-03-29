using System;
using System.Collections.Generic;
using System.Text;

namespace Nsar.Nodes.NwccScan.ReportGenerator
{
    /// <summary>
    /// Data columns that correspond to URL parameters
    /// <note>This is an incomplete list</note>
    /// </summary>
    public enum DataColumns : int
    {
        TAVG,   // avg temp
        TMAX,   // max temp
        TMIN,   // min temp
        TOBS,   // observed temp
        PRES,
        BATT,
        DPTP,
        PRCP,
        PREC,
        RHUM,
        RHUMX,
        RHUMN,
        SRADV,
        PVPV,
        SVPV,
        WDIRV,
        WSPDV,
        WSPDX
    }
}
